using CMoney.WebBackend.ProjectsLayer.Components;
using OriginalDataForwarding.Modules.VariableClass;
using OriginalDataForwarding.POCO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OriginalDataForwarding.Modules.TCPListener
{
    public class ProxyServer : IDisposable
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="socketPort"></param>
        /// <param name="isKeepNewConnectionWhenOverLimit">是否保留新連線</param>
        /// <param name="clientHeartBeatFrequency">客端心跳間隔</param>
        /// <param name="outputMessage">輸出訊息</param>
        public ProxyServer ( int socketPort,bool isKeepNewConnectionWhenOverLimit,int clientHeartBeatFrequency, Action<string> outputMessage )
        {
            fOutputMessage = outputMessage;
            fIsKeepNewConnectionWhenOverLimit = isKeepNewConnectionWhenOverLimit;
            fClientHeartBeatFrequency = clientHeartBeatFrequency;

            // Listener worker
            fTcpListener = new TcpListener ( IPAddress.Any , socketPort );
            fToken = new CancellationTokenSource ( );

            //事件處理
            OnStatus = new ThreadEvent<string> ( );
            OnData = new ThreadEvent<string> ( );
            OnConnectionStatus = new ThreadEvent<ProxyClient> ( );

            // 平行結構
            fClientTmpQueue = new ThreadBridge<ProxyClient> ( );
            fCastingMessage = new ThreadBridge<AckTask> ( );

            // connection pool
            fClientPool = new List<ProxyClient> ( );

            //connection worker
            fTaskListener = Task.Factory.StartNew ( ListenerAsync , fToken.Token );

            // boradcastor
            fBroadcastor = Task.Factory.StartNew ( BroadcastingAsync , fToken.Token );
        }

        ~ProxyServer ( )
        {
            Dispose ( );
        }

        /// <summary>
        /// 釋放資源，放開執行緒
        /// </summary>
        public void Dispose ( )
        {
            // 請求停止執行緒
            fToken.Cancel ( );

            // 停止listener
            fTcpListener.Stop ( );

            // 等待停止
            fTaskListener.Wait ( );

            GC.SuppressFinalize ( this );
        }

        /// <summary>
        /// 同 IP 的最大連線數
        /// </summary>
        private const int SAME_IP_LIMIT_COUNT = 2;

        /// <summary>
        /// 合理的時間間格
        /// </summary>
        private const long REASONABLE_TIME_GRID = 300;

        /// <summary>
        /// 合理的單一工作時間間格
        /// </summary>
        private const long REASONABLE_TIME_GRID_BY_TASK = 10;

        #region 變數

        /// <summary>
        /// 監聽socket
        /// </summary>
        private TcpListener fTcpListener = null;

        /// <summary>
        /// 用來Abort Task的物件
        /// </summary>
        private CancellationTokenSource fToken;

        /// <summary>
        /// 讀取用暫存陣列
        /// </summary>
        private byte[ ] fReadByte = new byte[ 4 * 1024 * 1024 ];

        /// <summary>
        /// listener
        /// </summary>
        private Task fTaskListener;

        /// <summary>
        /// listener
        /// </summary>
        private Task fBroadcastor;

        /// <summary>
        /// 連線池
        /// </summary>
        private List<ProxyClient> fClientPool;

        /// <summary>
        /// 存放新連線的暫存體
        /// </summary>
        private ThreadBridge<ProxyClient> fClientTmpQueue;

        /// <summary>
        /// 存放新連線的暫存體
        /// </summary>
        private ThreadBridge<AckTask> fCastingMessage;

        /// <summary>
        /// 輸出訊息
        /// </summary>
        private Action<string> fOutputMessage;

        /// <summary>
        /// 轉發數
        /// </summary>
        private int fSendCount = 0;

        /// <summary>
        /// 轉發時間總和
        /// </summary>
        private double fTotalSendMs = 0;

        /// <summary>
        /// 最大轉發時間
        /// </summary>
        private double fMaxSendMs = 0;

        /// <summary>
        /// 最後轉發時間
        /// </summary>
        private double fLastSendMs = 0;

        /// <summary>
        /// 最大轉發時間時戳
        /// </summary>
        private DateTime fMaxSendMsTimeStamp = new DateTime();

        /// <summary>
        /// 是否保留新連線
        /// </summary>
        private bool fIsKeepNewConnectionWhenOverLimit;

        /// <summary>
        /// 客端心跳間隔
        /// </summary>
        private int fClientHeartBeatFrequency;

        /// <summary>
        /// 心跳封包
        /// </summary>
        private byte[] fHeartbeatBytes;

        /// <summary>
        /// 心跳封包類型
        /// </summary>
        private ushort fHeartbeatDataType;

        #endregion

        #region 事件

        /// <summary>
        /// 狀態事件
        /// </summary>
        public ThreadEvent<string> OnStatus;

        /// <summary>
        /// 資料事件
        /// </summary>
        public ThreadEvent<string> OnData;

        /// <summary>
        /// 連線/斷線事件
        /// </summary>
        public ThreadEvent<ProxyClient> OnConnectionStatus;

        #endregion

        #region Worker方法

        /// <summary>
        /// 接收client 連線
        /// </summary>
        /// <param name="tokenObj"></param>
        private void ListenerAsync ( object tokenObj )
        {
            CancellationToken token = ( CancellationToken ) tokenObj;

            //啟動接收 10個連線數
            fTcpListener.Start ( 10 );

            //端點資訊
            var info = ( IPEndPoint ) fTcpListener.Server.LocalEndPoint;

            //開始工作囉!
            while ( !fToken.IsCancellationRequested )
            {
                try
                {
                    OnStatus.OnFireMessage ( string.Format ( "[Server] 等待Client連線...Port:{0}" , info.Port ) );

                    //執行續會所在這，一直到有新的連線才會繼續走
                    TcpClient client = fTcpListener.AcceptTcpClient ( );
                    client.NoDelay = true;
                    client.SendBufferSize = 1024 * 1024 ;
                    client.ReceiveBufferSize = 1024 * 1024;

                    //丟到平行資料結構
                    //這個queue的進出要很快，才不會影響到連線效率
                    var currentClient = new ProxyClient ( client );
                    fClientTmpQueue.Enqueue ( currentClient );

                    OnStatus.OnFireMessage ( string.Format ( "[Clinet] 來自{0}的連線...Port:{1}" , currentClient.Address , info.Port ) );

                    // log
                    OnConnectionStatus.OnFireMessage ( currentClient );
                }
                catch ( SocketException se )
                {
                    OnStatus.OnFireMessage ( string.Format ( "SocketCode:{0}\r\n{1}" , se.SocketErrorCode , se.ToString ( ) ) );
                }
                catch ( Exception e )
                {
                    OnStatus.OnFireMessage ( e.ToString ( ) );
                }
            }

            //Request 取消
            token.ThrowIfCancellationRequested ( );
        }


        /// <summary>
        /// 處理廣播訊息
        /// </summary>
        /// <param name="tokenObj"></param>
        private void BroadcastingAsync ( object tokenObj )
        {
            CancellationToken token = ( CancellationToken ) tokenObj;

            //開始工作囉!
            while ( !fToken.IsCancellationRequested )
            {
                try
                {
                    //管理連線池
                    DumpAndThrowDeadClient ( );

                    //改成一次取一包就打
                    AckTask packetage;
                    List<string> outputMessages;
                    List<string> allOutputMessages = new List<string>();
                    while ( fCastingMessage.TryDequeue( out packetage ) )
                    {
                        //廣播
                        SendBroadcasting( packetage ,out outputMessages );

                        if ( outputMessages.Count > 0 && packetage.DataType != 0)
                        {
                            allOutputMessages.AddRange( outputMessages );
                        }
                    }

                    //工作都做完了再輸出訊息
                    foreach ( string outputMessage in allOutputMessages )
                    {
                        fOutputMessage( outputMessage );
                    }

                    //訊息打完後清除訊息清單
                    allOutputMessages.Clear();

                    Thread.Sleep( 10 );                    
                }
                catch ( Exception e )
                {
                    OnStatus.OnFireMessage ( e.ToString ( ) );
                }
            }

            token.ThrowIfCancellationRequested ( );
        }

        #endregion

        #region 公開方法

        /// <summary>
        /// 廣播訊息
        /// </summary>
        /// <param name="message"></param>
        public void Broadcasting ( byte[ ] pkg , ushort dataType )
        {
            var newTask = new AckTask()
            {
                DataBytes = pkg,
                DataType = dataType,
                StartTime = DateTime.Now           
            };

            fCastingMessage.Enqueue ( newTask );
        }

        /// <summary>
        /// 設定心跳封包
        /// </summary>
        /// <param name="heartbeatBytes">心跳封包</param>
        /// <param name="heartbeatDataType">心跳封包類型</param>
        public void SetHeartBeatPackage ( byte[] heartbeatBytes, ushort heartbeatDataType )
        {
            fHeartbeatBytes = heartbeatBytes;
            fHeartbeatDataType = heartbeatDataType;
        }

        /// <summary>
        /// 取得有效的連線數
        /// </summary>
        /// <returns></returns>
        public int GetAvailableClinetCount ( )
        {
            return fClientPool.Count;
        }

        /// <summary>
        /// 取得最大轉發時間
        /// </summary>
        /// <returns></returns>
        public double GetMaxSendMs ()
        {
            return Math.Round( fMaxSendMs, 2, MidpointRounding.AwayFromZero );
        }

        /// <summary>
        /// 取得平均轉發時間
        /// </summary>
        /// <returns></returns>
        public double GetAvgSendMs ()
        {
            return fSendCount == 0 ? 0 : Math.Round( fTotalSendMs / fSendCount, 2, MidpointRounding.AwayFromZero );
        }

        /// <summary>
        /// 取得轉發數
        /// </summary>
        /// <returns></returns>
        public int GetSendCount ()
        {
            return fSendCount;
        }

        /// <summary>
        /// 取得最大轉發時間時戳
        /// </summary>
        /// <returns></returns>
        public string GetMaxSendMsTimeStamp ()
        {
            return fMaxSendMsTimeStamp.ToString("MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 取得最後轉發時間
        /// </summary>
        /// <returns></returns>
        public double GetLastSendMs ()
        {
            return Math.Round( fLastSendMs, 2, MidpointRounding.AwayFromZero );
        }

        /// <summary>
        /// 取得有效的連線數
        /// </summary>
        /// <returns></returns>
        public List<ProxyClient> GetAvailableClinets ( )
        {
            return fClientPool;
        }
        
        /// <summary>
        /// 踢出指定連線
        /// </summary>
        /// <param name="willRemoveClients">欲剔除的連線</param>
        public void RemoveClients ( List<ProxyClient> willRemoveClients )
        {
            foreach ( var item in willRemoveClients )
            {
                // 紀錄斷線 LOG
                OnConnectionStatus.OnFireMessage ( item );

                fClientPool.Remove ( item );

                OnStatus.OnFireMessage ( string.Format ( "剔除 Client [{0}]" , item.Address ) );
            }
        }

        #endregion

        /// <summary>
        /// 丟掉斷開連線的Client
        /// </summary>
        private void DumpAndThrowDeadClient ( )
        {
            // 拿出連線的
            var allNewConnections = fClientTmpQueue.DequeueAll ( );
            if ( allNewConnections.Count != 0 )
            {
                fClientPool.AddRange ( allNewConnections );
            }

            // 已斷線的連線
            var disconnectedClients = fClientPool.Where( x => !x.ClientSocket.Connected ).ToList();

            //取出有效連線超過指定數量的Ip連線群組
            var overLimitAddressGroupClients = fClientPool
                .Where( x => x.ClientSocket.Connected ) //看起來還活著的連線
                .GroupBy( x => x.Address ) //根據Ip分組
                .Where( x => x.Count() > SAME_IP_LIMIT_COUNT ) //取出超過連線數的
                .ToDictionary( x => x.Key, x => x.ToList() );

            #region 對所有超過連線數量的Ip檢測有效連線，保留最新(or最舊)的指定連線數

            //還沒設定心跳包的話就不打
            List<ProxyClient> overLimitClients = new List<ProxyClient>();
            if ( fHeartbeatBytes != null )
            {
                //心跳包
                var packetage = new AckTask()
                {
                    DataBytes = fHeartbeatBytes,
                    DataType = fHeartbeatDataType,
                    StartTime = DateTime.Now
                };

                string tickMessage; //這裡用不到
                Stopwatch writeWatch = new Stopwatch(); //這裡用不到
                List<string> outputMessages = new List<string>(); //這裡用不到
                foreach ( var addressGroupClients in overLimitAddressGroupClients )
                {
                    var clients = addressGroupClients.Value;
                    var needRemoveClientCount = clients.Count - SAME_IP_LIMIT_COUNT;

                    #region 對所有的Client檢測連線是否有效

                    var failClients = new List<ProxyClient>();
                    var successClients = new List<ProxyClient>();
                    foreach ( var client in clients )
                    {
                        //打心跳封包檢測連線是否有效
                        bool isWriteSuccess = sendBroadcastingToClient( client, packetage, writeWatch, null, outputMessages, out tickMessage );
                        if ( isWriteSuccess )
                        {
                            successClients.Add( client );
                        }
                        else
                        {
                            failClients.Add( client );
                        }
                    }

                    #endregion

                    #region 先踢掉無效的連線

                    var needRemoveClients = new List<ProxyClient>();
                    if ( failClients.Count > 0 )
                    {
                        needRemoveClients.AddRange( failClients );
                    }

                    #endregion

                    #region 如果還不夠就取有效連線來踢

                    if ( needRemoveClients.Count < needRemoveClientCount )
                    {
                        //還需要補踢的連線數
                        var needRemoveSuccessClientCount = needRemoveClientCount - needRemoveClients.Count;
                        
                        //超過時間沒接到心跳的優先踢
                        var now = DateTime.Now;
                        var needRemoves = successClients.OrderByDescending( x => ( now - x.LastReceiveTime ).TotalSeconds > fClientHeartBeatFrequency );
                        if ( fIsKeepNewConnectionWhenOverLimit )
                        {
                            //從舊的開始踢
                            needRemoves = needRemoves.ThenBy( x => x.ConnectedStamp );
                        }
                        else
                        {
                            //從新的開始踢
                            needRemoves = needRemoves.ThenByDescending( x => x.ConnectedStamp );
                        }

                        needRemoveClients.AddRange( needRemoves.Take( needRemoveSuccessClientCount ) );
                    }

                    #endregion

                    //加總到要踢的連線集合
                    overLimitClients.AddRange( needRemoveClients );
                }
            }

            #endregion

            // 丟掉不要的連線
            var allNeedRemoveClients = disconnectedClients.Union( overLimitClients ).ToList();
            if ( allNeedRemoveClients.Count > 0 )
            {
                removeConnections( allNeedRemoveClients );
            }
        }

        /// <summary>
        /// 移除連線
        /// </summary>
        /// <param name="connections">要移除的連線清單</param>
        private void removeConnections (List<ProxyClient> connections)
        {
            foreach ( var item in connections )
            {
                // 紀錄斷線LOG
                OnConnectionStatus.OnFireMessage( item );

                fClientPool.Remove( item );
                OnStatus.OnFireMessage( string.Format( "Clinet斷線[{0}]", item.Address ) );
            }
        }

        /// <summary>
        /// 送出通知工作
        /// </summary>
        /// <param name="message">封包資料</param>
        /// <param name="outputMessages">回傳輸出訊息</param>
        private void SendBroadcasting ( AckTask message, out List<string> outputMessages )
        {
            outputMessages = new List<string>();

            Stopwatch watchTime = new Stopwatch();
            watchTime.Start();
            var usedTicks = new List<string>();

            //打出通知
            SendStatistics statistics = new SendStatistics();
            Stopwatch taskWatch = new Stopwatch();
            Stopwatch writeWatch = new Stopwatch();
            var needRemoveClients = new List<ProxyClient>();
            foreach ( var client in fClientPool )
            {
                taskWatch.Restart();

                string tickMessage;
                bool isWriteSuccess = sendBroadcastingToClient( client, message, writeWatch, statistics, outputMessages, out tickMessage );
                if ( !isWriteSuccess )
                {
                    needRemoveClients.Add( client );
                }

                taskWatch.Stop();
                if ( taskWatch.ElapsedMilliseconds > REASONABLE_TIME_GRID_BY_TASK )
                {
                    usedTicks.Add( string.Format( "{0}[{1}]", taskWatch.ElapsedMilliseconds, tickMessage ?? "Exception" ) );
                }
            }

            //取得工作建立到完成的時間間格
            var taskTimeSpan = DateTime.Now - message.StartTime;
            if ( message.DataType != 0 )
            {
                fSendCount++;

                var taskTime = taskTimeSpan.TotalMilliseconds;
                if ( taskTime > fMaxSendMs )
                {
                    fMaxSendMs = taskTime;
                    fMaxSendMsTimeStamp = DateTime.Now;
                }
                
                fTotalSendMs += taskTime;
                fLastSendMs = taskTime;
            }

            //如果單次轉發超過300ms就應該注意            
            watchTime.Stop();
            if ( watchTime.ElapsedMilliseconds > REASONABLE_TIME_GRID )
            {
                outputMessages.Add( string.Format( "SendBroadcasting Milliseconds : {0}", watchTime.ElapsedMilliseconds ) );
                outputMessages.Add( string.Format( "TotalCount : {0} SuccessCount : {1} ExceptionCount : {2}", statistics.TotalSendCount, statistics.SuccessSendCount, statistics.ExceptionSendCount ) );
                outputMessages.Add( string.Format( "UsedTicksList : {0}", string.Join( " , ", usedTicks ) ) );
            }

            //移除無效連線
            if ( needRemoveClients.Count > 0 )
            {
                removeConnections( needRemoveClients );
            }
        }

        /// <summary>
        /// 發送封包到客端
        /// </summary>
        /// <param name="client">客端資訊</param>
        /// <param name="message">轉發封包資訊</param>
        /// <param name="writeWatch">封包送出時間監視器</param>
        /// <param name="statistics">發送統計資訊</param>
        /// <param name="outputMessages">輸出訊息清單</param>
        /// <param name="tickMessage">發送的時間訊息</param>
        /// <returns>是否發送成功</returns>
        private bool sendBroadcastingToClient ( ProxyClient client, AckTask message, Stopwatch writeWatch, SendStatistics statistics, List<string> outputMessages, out string tickMessage )
        {
            bool isSuccess = false;
            tickMessage = null;

            try
            {
                if ( client.ClientSocket.Connected )
                {
                    //ACK
                    if ( statistics!= null )
                    {
                        statistics.TotalSendCount++;
                    }

                    writeWatch.Restart();

                    //發送資料
                    var stream = client.ClientStream;
                    if ( stream.CanWrite )
                    {
                        //是心跳包就直接打
                        if ( message.DataType == fHeartbeatDataType )
                        {
                            stream.Write( message.DataBytes, 0, message.DataBytes.Length );
                        }
                        else
                        {
                            stream.BeginWrite( message.DataBytes, 0, message.DataBytes.Length, null, null );
                        }

                        isSuccess = true;

                        //ACK
                        if ( statistics != null )
                        {
                            statistics.SuccessSendCount++;
                        }
                    }

                    writeWatch.Stop();

                    //處理讀取資料(略過不做)
                    if ( stream.CanRead && stream.DataAvailable )
                    {
                        //先接起來之後再處理
                        //是心跳包就直接打
                        if ( message.DataType == fHeartbeatDataType )
                        {
                            stream.Read( fReadByte, 0, fReadByte.Length );
                        }
                        else
                        {
                            stream.BeginRead( fReadByte, 0, fReadByte.Length, null, null );
                        }

                        //更新接收時戳
                        client.LastReceiveTime = DateTime.Now;
                    }

                    tickMessage = string.Format( "W:{0}", writeWatch.ElapsedMilliseconds );
                }
            }
            catch ( IOException ex )
            {
                SocketException se = ex.InnerException as SocketException;
                int errorCode = se == null ? -1 : se.ErrorCode;
                outputMessages.Add( string.Format( "TcpEx：Send SocketErr={0} {1}", errorCode, client.Address ) );
                OnStatus.OnFireMessage( ex.ToString() );

                isSuccess = false;
                if ( statistics != null )
                {
                    statistics.ExceptionSendCount++;
                }
            }
            catch ( Exception e )
            {
                OnStatus.OnFireMessage( e.ToString() );

                isSuccess = false;
                if ( statistics != null )
                {
                    statistics.ExceptionSendCount++;
                }
            }

            return isSuccess;
        }
        
    }
}
