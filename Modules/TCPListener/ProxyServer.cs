using CMoney.WebBackend.ProjectsLayer.Components;
using OriginalDataForwarding.POCO;
using System;
using System.Collections.Generic;
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
        /// <param name="outputMessage">輸出訊息</param>
        public ProxyServer ( int socketPort, Action<string> outputMessage)
        {
            fOutputMessage = outputMessage;

            // Listener worker
            fTcpListener = new TcpListener( IPAddress.Any, socketPort );
            fToken = new CancellationTokenSource();

            //事件處理
            OnStatus = new ThreadEvent<string>();
            OnData = new ThreadEvent<string>();
            OnConnectionStatus = new ThreadEvent<ProxyClient>();

            // 平行結構
            fClientTmpQueue = new ThreadBridge<ProxyClient>();
            fCastingMessage = new ThreadBridge<AckTask>();  

            // connection pool
            fClientPool = new List<ProxyClient>();

            //connection worker
            fTaskListener = Task.Factory.StartNew( ListenerAsync, fToken.Token );

            // boradcastor
            fBroadcastor = Task.Factory.StartNew( BroadcastingAsync, fToken.Token );
        }
        ~ProxyServer()
        {
            Dispose();
        }

        /// <summary>
        /// 釋放資源，放開執行緒
        /// </summary>
        public void Dispose()
        {
            // 請求停止執行緒
            fToken.Cancel();

            // 停止listener
            fTcpListener.Stop();

            // 等待停止
            fTaskListener.Wait();

            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// 同 IP 的最大連線數
        /// </summary>
        private const int SAME_IP_LIMIT_COUNT = 2;

        #region 變數

        /// <summary>
        /// 監聽socket
        /// </summary>
        private TcpListener fTcpListener = null;

        /// <summary>
        /// 用來Abort Task的物件
        /// </summary>
        private CancellationTokenSource fToken;

        private byte[] fReadByte = new byte[4194304];

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
        private void ListenerAsync( object tokenObj )
        {
            CancellationToken token = (CancellationToken)tokenObj;

            //啟動接收 10個連線數
            fTcpListener.Start(10);

            //端點資訊
            var info = (IPEndPoint)fTcpListener.Server.LocalEndPoint;

            //開始工作囉!
            while ( !fToken.IsCancellationRequested )
            {
                try
                {
                    OnStatus.OnFireMessage( string.Format( "[Server] 等待Client連線...Port:{0}", info.Port ) );

                    //執行續會所在這，一直到有新的連線才會繼續走
                    TcpClient client = fTcpListener.AcceptTcpClient();

                    //丟到平行資料結構
                    //這個queue的進出要很快，才不會影響到連線效率
                    var currentClient = new ProxyClient( client );
                    fClientTmpQueue.Enqueue( currentClient );

                    OnStatus.OnFireMessage( string.Format( "[Clinet] 來自{0}的連線...Port:{1}", currentClient.Address, info.Port ) );

                    // log
                    OnConnectionStatus.OnFireMessage( currentClient );
                }
                catch ( SocketException se )
                {
                    OnStatus.OnFireMessage( string.Format( "SocketCode:{0}\r\n{1}", se.SocketErrorCode, se.ToString() ) );
                }
                catch ( Exception e )
                {
                    OnStatus.OnFireMessage( e.ToString() );
                }
            }

            //Request 取消
            token.ThrowIfCancellationRequested();
        }


        /// <summary>
        /// 處理廣播訊息
        /// </summary>
        /// <param name="tokenObj"></param>
        private void BroadcastingAsync( object tokenObj )
        {
            CancellationToken token = (CancellationToken)tokenObj;

            //開始工作囉!
            while ( !fToken.IsCancellationRequested )
            {
                try
                {
                    //管理連線池
                    DumpAndThrowDeadClient();

                    //拿出要處理的工作
                    var allPacketage = fCastingMessage.DequeueAll().ToArray();
                    if ( allPacketage.Length == 0 )
                    {
                        Thread.Sleep( 10 );
                    }
                    else
                    {
                        //廣播
                        SendBroadcasting( allPacketage );
                    }
                }
                catch ( Exception e )
                {
                    OnStatus.OnFireMessage( e.ToString() );
                }
            }

            token.ThrowIfCancellationRequested();
        }

        #endregion

        #region 公開方法

        /// <summary>
        /// 廣播訊息
        /// </summary>
        /// <param name="message"></param>
        public void Broadcasting( byte[] pkg, ushort dataType )
        {
            fCastingMessage.Enqueue( new AckTask()
            {
                DataBytes = pkg,
                DataType = dataType
            } );
        }


        /// <summary>
        /// 取得有效的連線數
        /// </summary>
        /// <returns></returns>
        public int GetAvailableClinetCount()
        {
            return fClientPool.Count;
        }

        /// <summary>
        /// 取得有效的連線數
        /// </summary>
        /// <returns></returns>
        public List<ProxyClient> GetAvailableClinets()
        {
            return fClientPool;
        }

        /// <summary>
        /// 踢出指定連線
        /// </summary>
        /// <param name="willRemoveClients">欲剔除的連線</param>
        public void RemoveClients( List<ProxyClient> willRemoveClients )
        {
            foreach ( var item in willRemoveClients )
            {
                // 紀錄斷線 LOG
                OnConnectionStatus.OnFireMessage( item );

                fClientPool.Remove( item );

                OnStatus.OnFireMessage( string.Format( "剔除 Client [{0}]", item.Address ) );
            }
        }

        #endregion

        /// <summary>
        /// 丟掉斷開連線的Client
        /// </summary>
        private void DumpAndThrowDeadClient()
        {
            // 拿出連線的
            var allConnection = fClientTmpQueue.DequeueAll();
            if ( allConnection.Count != 0 )
            {
                fClientPool.AddRange( allConnection );
            }

            // 超過限制數量的連線
            var overLimitClients = fClientPool.Where( x => x.ClientSocket.Connected)
                                    .GroupBy( x => x.Address ).ToDictionary( x => x.Key, x => x.ToList() )
                                    .Where( x => x.Value.Count > SAME_IP_LIMIT_COUNT )
                                    .Select( x => x.Value.LastOrDefault() ).ToList();

            // 已斷線的連線
            var disconnected = fClientPool.Where( x => !x.ClientSocket.Connected ).ToList();

            // 丟掉不要的連線
            foreach ( var item in disconnected.Concat(overLimitClients).Distinct() )
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
        /// <param name="successCount"></param>
        private void SendBroadcasting( params AckTask[] messages )
        {
            var taskStartTick = Environment.TickCount;
            var usedTicks = new List<string>();

            //打出通知
            int total = 0;
            int success = 0;
            int exceptionCount = 0;
            foreach ( var data in messages )
            {
                foreach ( var item in fClientPool )
                {
                    var sendBroadcastingTick = Environment.TickCount;
                    string tickMessage = null;

                    try
                    {
                        if ( item.ClientSocket.Connected )
                        {
                            //ACK
                            total++;

                            int writeTick = Environment.TickCount;
                            
                            //發送資料
                            if ( item.ClientStream.CanWrite )
                            {
                                item.ClientStream.Write( data.DataBytes, 0, data.DataBytes.Length );

                                //ACK
                                success++;
                            }

                            writeTick = Environment.TickCount - writeTick;
                            int readTick = Environment.TickCount;

                            //處理讀取資料(略過不做)
                            if ( item.ClientStream.CanRead && item.ClientStream.DataAvailable )
                            {
                                //丟掉回傳資料
                                item.ClientStream.Read( fReadByte, 0, fReadByte.Length );
                            }

                            readTick = Environment.TickCount - readTick;
                            tickMessage = string.Format( "W:{0},R:{1}", writeTick, readTick );
                        }
                    }
                    catch ( Exception e )
                    {
                        OnStatus.OnFireMessage( e.ToString() );
                        exceptionCount++;
                    }

                    sendBroadcastingTick = Environment.TickCount - sendBroadcastingTick;
                    usedTicks.Add( string.Format( "{0}[{1}]", sendBroadcastingTick, tickMessage ?? "Exception" ) );
                }
            }

            taskStartTick = Environment.TickCount - taskStartTick;

            //如果單次轉發超過300ms就應該注意
            if ( taskStartTick > 300 )
            {
                fOutputMessage( string.Format( "SendBroadcasting TaskTick : {0}", taskStartTick ) );
                fOutputMessage( string.Format( "TotalCount : {0} SuccessCount : {1} ExceptionCount : {2}", total, success, exceptionCount ) );
                fOutputMessage( string.Format( "UsedTicksList : {0}", string.Join( " , ", usedTicks ) ) );
            }
        }

    }
}
