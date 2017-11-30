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

            MarketType marketType = (MarketType)fSetting.ApMartketType;
            this.Text = string.Format( "{0} {1} Ver:{2}", marketType, Application.ProductName, Application.ProductVersion );

            //服務原始IDKey
            string originKey = string.Format( "{0}:{1}", GeneralTools.GetServiceOriginalKey( fSetting.DpscPort, Assembly.GetExecutingAssembly() ), marketType );

            TextBox_DpscKeyId.Text = originKey;

            // dataGridView 錯誤處理 (不然會跳exception)
            DataGridView_Clients.DataError += DataGridView_Clients_DataError;

            fProxyServer = new ProxyServer( fSetting.MulticastPort, fSetting.IsKeepNewConnectionWhenOverLimit, OutMessages );
            fProxyServer.OnStatus.OnFireEvent += fProxyServer_OnStatusMessage;

            // 心跳封包樣本
            fHeartbeat = new Heartbeat( new TimeSpan( 0, 0, fSetting.HeartBeatFrequency ), fProxyServer.Broadcasting );

            //發送模組
            fForwardModule = new ForwardModule( originKey, 
                                                fSetting.DpscIp, fSetting.DpscPort, fSetting.DpscChk,
                                                OutMessages,
                                                fHeartbeat.BlockingHeartbeat,
                                                fSetting.RawDataChannel
                                               );

            // datagrid綁資料結構
            fBindingSource = new BindingSource();
            fBindingSource.DataSource = fProxyServer.GetAvailableClinets();
            DataGridView_Clients.DataSource = fBindingSource;
        }

        #region 變數

        /// <summary>
        /// 設定
        /// </summary>
        private Settings fSetting;

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
        /// 有效連線
        /// </summary>
        private List<ProxyClient> fAvailableClients;

        #endregion

        #region 列舉

        /// <summary>
        /// 市場別
        /// </summary>
        public enum MarketType : byte
        {
            /// <summary>
            /// 上市
            /// </summary>
            TSE = 2,

            /// <summary>
            /// 上櫃
            /// </summary>
            OTC = 3,

            /// <summary>
            /// 期貨
            /// </summary>
            TMX = 4,

            /// <summary>
            /// 選擇權
            /// </summary>
            OPT = 5,

            /// <summary>
            /// 興櫃
            /// </summary>
            EMERGING = 6,

            /// <summary>
            /// 期貨盤後
            /// </summary>
            TMX_AfterHours = 7,

            /// <summary>
            /// 選擇權盤後
            /// </summary>
            OPT_AfterHours = 8
        }

        #endregion 列舉

        #region 方法

        /// <summary>
        /// 處理狀態訊息事件
        /// </summary>
        /// <param name="statusMessage"></param>
        private void fProxyServer_OnStatusMessage( string statusMessage )
        {
            TextBox_Status.AppendText( string.Format( "[{0}]{1}{2}", DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" ), statusMessage, Environment.NewLine ) );
        }

        /// <summary>
        /// 委派用的訊息輸出
        /// </summary>
        /// <param name="message"></param>
        private void OutMessages( string message )
        {
            if ( InvokeRequired )
            {
                this.Invoke( new Action<string>( OutMessages ), new object[] { message } );
                return;
            }
            else
            {
                TextBox_Status.AppendText( string.Format( "[{0}] : {1}{2}", DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" ), message, Environment.NewLine ) );
            }           
        }

        /// <summary>
        /// 刷畫面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Repaint_Tick( object sender, EventArgs e )
        {
            if ( tabControl1.SelectedTab == tabPage_Statu )
            {
                Label_ClientCount.Text = fProxyServer.GetAvailableClinetCount().ToString();
                label_MaxSendMs.Text = fProxyServer.GetMaxSendMs().ToString();
                label_AvgSendMs.Text = fProxyServer.GetAvgSendMs().ToString();
                label_SendCount.Text = fProxyServer.GetSendCount().ToString();

                Label_ForwardingCount.Text = fForwardModule.GetForwardingCount().ToString();
                Label_HeartCountDown.Text = fHeartbeat.GetHeartbeatRemainingSec().ToString( "0" );
            }
            else if ( tabControl1.SelectedTab == tabPage2 )
            {
                fBindingSource.ResetBindings( true );
                DataGridView_Clients.AutoResizeColumns();
            }
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

        #endregion

        /// <summary>
        /// 取得目前所有有效連線
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_GetAllClients_Click( object sender, EventArgs e )
        {
            checkedListBox_Clients.Items.Clear();

            fAvailableClients = fProxyServer.GetAvailableClinets();

            foreach ( var client in fAvailableClients )
            {
                checkedListBox_Clients.Items.Add( client.Address, false );
            }
        }

        /// <summary>
        /// 剔除已勾選連線
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_RemoveClients_Click( object sender, EventArgs e )
        {
            // 已選擇的連線 Index
            var checkedIndex = checkedListBox_Clients.CheckedIndices.Cast<int>().ToList();

            // 欲剔除的連線
            var checkedClients = new List<ProxyClient>();
            foreach ( var index in checkedIndex )
            {
                checkedClients.Add( fAvailableClients[index] );
            }

            fProxyServer.RemoveClients( checkedClients );

            button_GetAllClients.PerformClick();
        }
    }

}
