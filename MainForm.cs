using CMoney.Real.Dpsc;
using OriginalDataForwarding.Modules;
using OriginalDataForwarding.Modules.TCPListener;
using OriginalDataForwarding.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
            this.Text = string.Format( "{0} {1} Ver:{2} x{3}", marketType, Application.ProductName, Application.ProductVersion , IntPtr.Size * 8);

            //服務原始IDKey
            string originKey = string.Format( "{0}:{1}", Dpsc.GetServiceOriginalKey( fSetting.DpscPort, Assembly.GetExecutingAssembly() ), marketType);

            TextBox_DpscKeyId.Text = originKey;

            // dataGridView 錯誤處理 (不然會跳exception)
            DataGridView_Clients.DataError += DataGridView_Clients_DataError;

            fProxyServer = new ProxyServer( fSetting.MulticastPort, fSetting.IsKeepNewConnectionWhenOverLimit,fSetting.ClientHeartBeatFrequency, fSetting.ClientSameIPLimitCount, OutMessages );
            fProxyServer.OnStatus.OnFireEvent += fProxyServer_OnStatusMessage;

            // 心跳封包樣本
            fHeartbeat = new Heartbeat( new TimeSpan( 0, 0, fSetting.HeartBeatFrequency ), fProxyServer.Broadcasting );

            fProxyServer.SetHeartBeatPackage( fHeartbeat.HeartbeatBytes, Heartbeat.HEART_BEAT_DATA_TYPE );

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

        #region 常數

        /// <summary>
        /// 顯示文字最大行數
        /// </summary>
        private int SHOW_TEXT_LIMIT_LINES = 500;

        /// <summary>
        /// 早上6點重置統計
        /// </summary>
        private int RESET_CALCUATION_TIME = 6;

        #endregion

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
            if (TextBox_Status.Lines.Count() > SHOW_TEXT_LIMIT_LINES)
            {
                TextBox_Status.Clear();
            }

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
                if (TextBox_Status.Lines.Count() > SHOW_TEXT_LIMIT_LINES)
                {
                    TextBox_Status.Clear();
                }

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
            DateTime nowTime = DateTime.Now;

            if (nowTime.Hour == RESET_CALCUATION_TIME && nowTime.Minute == 0 && nowTime.Second < 5)
            {
                fProxyServer.ResetCalculation();
                fForwardModule.ResetCalculation();
            }

            if ( tabControl1.SelectedTab == tabPage_Statu )
            {
                Label_ClientCount.Text = fProxyServer.GetAvailableClinetCount().ToString();
                label_MaxSendMs.Text = fProxyServer.GetMaxSendMs().ToString();
                label_MaxSendTimeStamp.Text = fProxyServer.GetMaxSendMsTimeStamp();
                label_AvgSendMs.Text = fProxyServer.GetAvgSendMs().ToString();
                label_LastSendTime.Text = fProxyServer.GetLastSendMs().ToString();
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

            checkedListBox_Clients.Items.Clear();
        }

        /// <summary>
        /// 確認主機連線狀態
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ShowSystemConnection_Click(object sender, EventArgs e)
        {
            dataGridView_System.DataSource = null;
            dataGridView_System.DataSource = ConnectionCheck.GetNetStatPorts(null,new List<string>() { fSetting.MulticastPort.ToString() });
        }
    }

}
