using OriginalDataForwarding.POCO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OriginalDataForwarding.Modules
{
    public class Heartbeat : IDisposable
    {
        /// <summary>
        /// 建構子 (心跳模組要把廣播方法帶入)
        /// </summary>
        /// <param name="cycleDealay"></param>
        /// <param name="broadcasting"></param>
        public Heartbeat(TimeSpan cycleDealay, Action<byte[], ushort> broadcasting )
        {
            fCycleDealay = cycleDealay;
            fBroadcasting = broadcasting;
            fToken = new CancellationTokenSource();
            fHeartbeat = new TelegramHeartbeat(null).GetBytes();

            fHeartbeater = Task.Factory.StartNew( ( tokenObj ) =>
             {
                 var token = (CancellationToken)tokenObj;

                 while ( !token.IsCancellationRequested )
                 {
                     var now = DateTime.Now;
                     if ( Timeout < now )
                     {
                         Timeout = now.Add( fCycleDealay );
                         fBroadcasting( fHeartbeat, 0 );
                     }

                     //休息一下
                     Thread.Sleep( 1000 );
                 }
             }, fToken.Token );           
        }

        ~Heartbeat()
        {
            Dispose();
        }

        public void Dispose()
        {
            fToken.Cancel();
            fHeartbeater.Wait();
        }

        #region 變數

        /// <summary>
        /// 廣播方法
        /// </summary>
        private Action<byte[], ushort> fBroadcasting;

        /// <summary>
        /// worker
        /// </summary>
        private Task fHeartbeater;

        /// <summary>
        /// 取消task
        /// </summary>
        private CancellationTokenSource fToken;

        /// <summary>
        /// 延遲多久
        /// </summary>
        private TimeSpan fCycleDealay;

        /// <summary>
        /// 心跳封包
        /// </summary>
        private byte[] fHeartbeat;

        /// <summary>
        /// 下一次的timeout
        /// </summary>
        private DateTime Timeout;

        #endregion

        /// <summary>
        /// 重置心跳延遲
        /// </summary>
        public void ResetPeriod()
        {
            Timeout = DateTime.Now.Add( fCycleDealay );
        }

        /// <summary>
        /// 發送訊息 並阻擋心跳
        /// </summary>
        /// <param name="pkg"></param>
        public void BlockingHeartbeat( byte[] pkg, ushort dataType )
        {
            fBroadcasting( pkg, dataType );
            ResetPeriod();
        }

        public double GetHeartbeatRemainingSec()
        {
            return ( Timeout - DateTime.Now ).TotalSeconds;
        }

    }
}