using CMoney.Real.Dpsc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginalDataForwarding.Modules
{
    public class ForwardModule : IDisposable
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="dpscKey"></param>
        /// <param name="dpscIp"></param>
        /// <param name="dpscPort"></param>
        /// <param name="chkSum"></param>
        /// <param name="outputMessage"></param>
        /// <param name="broadcasting"></param>
        /// <param name="channel"></param>
        public ForwardModule(string dpscKey, string dpscIp, ushort dpscPort, uint chkSum,
                            Action<string> outputMessage,
                            Action<byte[], ushort> broadcasting, uint channel )
        {
            fOutputMessage = outputMessage;
            fBroadcasting = broadcasting;
            fChannel = channel;

            fDpsc = new Dpsc( dpscKey );
            fDpsc.ServerHost = dpscIp;
            fDpsc.ServerPort = dpscPort;
            fDpsc.SysCheckSum = chkSum;
            fDpsc.OnStatus += fDpsc_OnStatus;
            fDpsc.OnRcvData += fDpsc_OnRcvData;
            fDpsc.Connect();

        }

        ~ForwardModule()
        {
            Dispose();
        }

        /// <summary>
        /// 釋放資源
        /// </summary>
        public void Dispose()
        {
            fDpsc.Disconnect();
            fDpsc.Dispose();

            GC.SuppressFinalize( this );
        }

        #region 變數

        /// <summary>
        /// Dpsc
        /// </summary>
        private Dpsc fDpsc;

        /// <summary>
        /// 訊號頻道
        /// </summary>
        uint fChannel;

        /// <summary>
        /// 輸出訊息
        /// </summary>
        private Action<string> fOutputMessage;

        /// <summary>
        /// 轉發方法
        /// </summary>
        private Action<byte[], ushort> fBroadcasting;

        /// <summary>
        /// 轉發數
        /// </summary>
        private int fForwardingCount = 0;

        #endregion

        #region Dpsc

        /// <summary>
        /// Dpsc連線事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        private void fDpsc_OnStatus( Dpsc sender, Dpsc.DpscStatusList status, string message )
        {
            switch ( status )
            {
                case Dpsc.DpscStatusList.DebugError:
                    break;
                case Dpsc.DpscStatusList.Connecting:
                    {
                        fOutputMessage( "Dpsc連線中" );
                    }
                    break;
                case Dpsc.DpscStatusList.Connected:
                    {
                        fOutputMessage( "Dpsc連線成功" );

                        //訂閱即時條件頻道
                        fDpsc.SubChannel( fChannel );

                    }
                    break;
                case Dpsc.DpscStatusList.KeepAlive:
                    break;
                case Dpsc.DpscStatusList.SubChannel:
                    {
                        fOutputMessage( string.Format( "訂閱{0}", message ) );
                    }
                    break;
                case Dpsc.DpscStatusList.DisSubChannel:
                    break;
                case Dpsc.DpscStatusList.ServerTime:
                    break;
                case Dpsc.DpscStatusList.DisConnect:
                    {
                        fOutputMessage( "Dpsc斷線了" );
                        fDpsc.Connect();
                    }
                    break;
                case Dpsc.DpscStatusList.ConnectTimeout:
                    {
                        fOutputMessage( "Dpsc連線逾時" );
                        fDpsc.Connect();
                    }
                    break;
                case Dpsc.DpscStatusList.UserDisConnect:
                    {
                        fOutputMessage( "使用者將Dpsc斷開" );
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Dpsc收到資料事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        private void fDpsc_OnRcvData( Dpsc sender, ushort dataType, byte[] data )
        {
            switch ( dataType )
            {
                case 9951:
                case 9952:
                case 9953:
                case 9954:

                    fForwardingCount++;

                    //直接把收到封包轉發出去
                    fBroadcasting( data, dataType );
                    break;
                default:
                    break;
            }
        }

        #endregion

        public int GetForwardingCount()
        {
            return fForwardingCount;
        }
    }
}
