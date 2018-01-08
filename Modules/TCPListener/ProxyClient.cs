using OriginalDataForwarding.Modules.VariableClass;
using OriginalDataForwarding.POCO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OriginalDataForwarding.Modules.TCPListener
{
    public class ProxyClient
    {
        #region 建構子

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="client"></param>
        /// <param name="debugMsgFun"></param>
        public ProxyClient( TcpClient client, Action<string> debugMsgFun)
        {
            fClientSocket = client;
            IPEndPoint ipep = client.Client.RemoteEndPoint as IPEndPoint;
            Address = ipep.Address.ToString();
            fClientStream = client.GetStream();
            ConnectedStamp = DateTime.Now;
            LastReceiveTime = DateTime.Now;
            IsErrorClient = false;

            fAddDebugFun = debugMsgFun;
        }

        ~ProxyClient()
        {
            Dispose();
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            //關閉串流
            this.fClientStream.Close();

            //關閉連線
            this.fClientSocket.Close();
        }

        #endregion

        #region 常數

        /// <summary>
        /// Tcp Client Buffer size 
        /// Buffer 開的大，多 Client 同時存在時記憶體就吃的多，但 Buffer 開不大，throughput 就拉不起來        
        /// </summary>
        public const int TCP_BUFFER_SIZE = 1024;

        #endregion

        #region 屬性

        /// <summary>
        /// 連線請求時間
        /// </summary>
        public DateTime ConnectedStamp
        {
            set; get;
        }

        /// <summary>
        /// 最後接收心跳時間
        /// </summary>
        public DateTime LastReceiveTime
        {
            set; get;
        }

        /// <summary>
        /// 對方連入位置
        /// </summary>
        public string Address
        {
            private set; get;
        }

        /// <summary>
        /// 是否是錯誤的Client 用這個來判斷是否移除
        /// </summary>
        public bool IsErrorClient
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// HTS端socket
        /// </summary>
        private TcpClient fClientSocket;

        /// <summary>
        /// 該socket的串流
        /// </summary>
        private NetworkStream fClientStream;
      
        /// <summary>
        /// 外部傳入 除錯訊息函式
        /// </summary>
        private Action<string> fAddDebugFun;
        

        /// <summary>
        /// TcpClient是否連線
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return fClientSocket.Connected;
        }


        /// <summary>
        /// 發送封包到客端
        /// </summary>
        /// <param name="message">轉發封包資訊</param>
        /// <param name="writeWatch">封包送出時間監視器</param>
        /// <param name="statistics">發送統計資訊</param>
        /// <param name="outputMessages">輸出訊息清單</param>
        /// <param name="tickMessage">發送的時間訊息</param>
        /// <returns>是否發送成功</returns>
        public bool SendBroadcastingToClient( AckTask message, Stopwatch writeWatch, SendStatistics statistics, List<string> outputMessages, out string tickMessage)
        {
            bool isSuccess = false;
            tickMessage = null;

            try
            {
                if (this.fClientSocket.Connected)
                {
                    //ACK
                    if (statistics != null)
                    {
                        statistics.TotalSendCount++;
                    }

                    writeWatch.Restart();

                    //發送資料
                    var stream = this.fClientStream;
                    this.fClientStream.BeginWrite(message.DataBytes, 0, message.DataBytes.Length, AsyncSendCallback, statistics);
                    isSuccess = true;
                    

                    writeWatch.Stop();

                    //處理讀取資料(下端心跳)
                    if (this.fClientStream.DataAvailable)
                    {           
                        //client端 只讀心跳包
                        byte[] buffer = new byte[TCP_BUFFER_SIZE];
                        this.fClientStream.BeginRead(buffer, 0, buffer.Length, AsyncReceiveCallback, buffer);
                    }

                    tickMessage = string.Format("W:{0}", writeWatch.ElapsedMilliseconds);
                }
            }
            catch (IOException ex)
            {
                IsErrorClient = true;

                SocketException se = ex.InnerException as SocketException;
                int errorCode = se == null ? -1 : se.ErrorCode;
                outputMessages.Add(string.Format("TcpEx：Send SocketErr={0} {1}", errorCode, this.Address));

                isSuccess = false;
                if (statistics != null)
                {
                    statistics.ExceptionSendCount++;
                }
            }
            catch (Exception ex)
            {
                IsErrorClient = true;

                outputMessages.Add(string.Format("TcpEx：Send Err={0} {1}", ex.Message, this.Address));

                isSuccess = false;
                if (statistics != null)
                {
                    statistics.ExceptionSendCount++;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 非同步串流寫入回呼
        /// </summary>
        /// <param name="ar"></param>
        private void AsyncSendCallback(IAsyncResult ar)
        {
            SendStatistics statistics = (SendStatistics)ar.AsyncState;

            try
            {
                // Retrieve the socket from the state object.  
                statistics.SuccessSendCount++;

            }
            catch (IOException ex)
            {
                IsErrorClient = true;
                statistics.ExceptionSendCount++;

                SocketException se = ex.InnerException as SocketException;
                int errorCode = se == null ? -1 : se.ErrorCode;
                fAddDebugFun(string.Format("TcpEx：EndWrite SocketErr={0} {1}", errorCode, this.Address));

            }
            catch (Exception ex)
            {
                IsErrorClient = true;
                statistics.ExceptionSendCount++;

                fAddDebugFun(string.Format("TcpEx：EndWrite {0} {1}", ex.Message, this.Address));
            }
        }

        /// <summary>
        /// 非同步串流讀取回呼
        /// </summary>
        /// <param name="ar">回呼結果，這裡直接使用 byte[] 沒有再包裝成其它型別</param>
        private void AsyncReceiveCallback(IAsyncResult ar)
        {
            try
            {
                
                byte[] buffer = ar.AsyncState as byte[];

                //更新接收時戳
                this.LastReceiveTime = DateTime.Now;

            }
            catch (IOException ex)
            {
                IsErrorClient = true;
                SocketException se = ex.InnerException as SocketException;
                int errorCode = se == null ? -1 : se.ErrorCode;
                fAddDebugFun(string.Format("TcpEx：EndRead SocketErr={0} {1}", errorCode, this.Address));
            }
            catch (Exception ex)
            {
                IsErrorClient = true;
                fAddDebugFun(string.Format("TcpEx：EndRead {0} {1}", ex.Message, this.Address));
 
            }
        }
    }
}