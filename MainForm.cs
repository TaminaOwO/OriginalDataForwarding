using CMoney.WebBackend.Tools;
using OriginalDataForwarding.Modules;
using OriginalDataForwarding.Modules.TCPListener;
using OriginalDataForwarding.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OriginalDataForwarding
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            fSetting = new Settings();
            
            uint channel = 0;
            int serverPort = 0;
            string market = fSetting.Market.ToUpper();

            //依市場別決定訂閱的通道與發送的通道
            switch ( market )
            {
                case "TSE":
                    channel = fSetting.TseRawChannel;
                    serverPort = fSetting.TseServerPort;
                    break;
                case "OTC":
                    channel = fSetting.OtcRawChannel;
                    serverPort = fSetting.OtcServerPort;
                    break;
                case "TMX":
                    channel = fSetting.TmxRawChannel;
                    serverPort = fSetting.TmxServerPort;
                    break;
                case "OPT":
                    channel = fSetting.OptRawChannel;
                    serverPort = fSetting.OptServerPort;
                    break;
                default:
                    OutMessages( "無法判別市場，程式終止" );
                    return;

            }

            this.Text = string.Format( "{0} {1} Ver:{2}", market, Application.ProductName, Application.ProductVersion );

            fOriginKey = string.Format( "{0}:{1}",
                                GeneralTools.GetServiceOriginalKey( fSetting.DpscPort, Assembly.GetExecutingAssembly() )
                                , market );

            TextBox_DpscKeyId.Text = fOriginKey;

            // dataGridView 錯誤處理 (不然會跳exception)
            DataGridView_Clients.DataError += DataGridView_Clients_DataError;

            fProxyServer = new ProxyServer( serverPort );
            fProxyServer.OnStatus.OnFireEvent += fProxyServer_OnStatusMessage;

            // 心跳封包樣本
            fHeartbeat = new Heartbeat( new TimeSpan( 0, 0, fSetting.HeartBeatFrequency ), fProxyServer.Broadcasting );

            //發送模組
            fForwardModule = new ForwardModule( fOriginKey, 
                                                fSetting.DpscIp, fSetting.DpscPort, fSetting.DpscChk,
                                                OutMessages,
                                                fHeartbeat.BlockingHeartbeat,
                                                channel 
                                               );

            // datagrid綁資料結構
            fBindingSource = new BindingSource();
            fBindingSource.DataSource = fProxyServer.GetAvailableClinets();
            DataGridView_Clients.DataSource = fBindingSource;
        }

        /// <summary>
        /// 設定
        /// </summary>
        private Settings fSetting;

        /// <summary>
        /// 服務原始IDKey
        /// </summary>
        private string fOriginKey;

        /// <summary>
        /// stocket轉送
        /// </summary>
        private ProxyServer fProxyServer;

        /// <summary>
        /// 心跳
        /// </summary>
        private Heartbeat fHeartbeat;

        /// <summary>
        /// DataGridView 來源結構
        /// </summary>
        private BindingSource fBindingSource;

        /// <summary>
        /// 發送模組
        /// </summary>
        private ForwardModule fForwardModule;


        /// <summary>
        /// 處理狀態訊息事件
        /// </summary>
        /// <param name="statusMessage"></param>
        private void fProxyServer_OnStatusMessage( string statusMessage )
        {
            TextBox_Status.AppendText( string.Format( "[{0}]", DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" ) ) );
            TextBox_Status.AppendText( statusMessage );
            TextBox_Status.AppendText( "\r\n" );
        }

        /// <summary>
        /// 委派用的訊息輸出
        /// </summary>
        /// <param name="message"></param>
        private void OutMessages( string message )
        {
            TextBox_Status.AppendText( string.Format( "[{0}] : {1}{2}", DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" ), message, Environment.NewLine ) );
        }

        /// <summary>
        /// 刷畫面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Repaint_Tick( object sender, EventArgs e )
        {
            Label_ClientCount.Text = fProxyServer.GetAvailableClinetCount().ToString();
            Label_ForwardingCount.Text = fForwardModule.GetForwardingCount().ToString();
            Label_HeartCountDown.Text = fHeartbeat.GetHeartbeatRemainingSec().ToString( "0" );
            fBindingSource.ResetBindings( true );
            DataGridView_Clients.AutoResizeColumns();
        }

        /// <summary>
        /// 阻擋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( MessageBox.Show( "真的要離開嗎? 通知不會回補!", "警告", MessageBoxButtons.YesNo ) == System.Windows.Forms.DialogResult.No )
            {
                e.Cancel = true;
            }
            else
            {
                File.AppendAllText( "appExit.log", string.Format( "[{0}]程式被關閉, reason = {1}\r\n", DateTime.Now, e.CloseReason ) );
            }
        }


        /// <summary>
        /// 錯誤視窗處理 這裡不需要顯示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_Clients_DataError( object sender, DataGridViewDataErrorEventArgs e )
        {
            e.Cancel = true;
        }


    }

}
