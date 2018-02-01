using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OriginalDataForwarding.Modules
{
    /*

        TCP一共有11種連線狀態： 
        SYN_SENT：在傳送連線請求後等待匹配的連線請求（客戶端向伺服器端傳送SYN請求建立一個連線，之後將狀態置為SYN_SENT）

        FIN_WAIT_1：等待遠端TCP連線中斷請求，或先前的連線中斷請求的確認（主動關閉端傳送FIN請求主動關閉連線，之後將狀態置為FIN_WAIT_1） 

        FIN_WAIT_2：從遠端TCP等待連線中斷請求（主動關閉端接到ACK後，之後將狀態置為FIN_WAIT_2，此時是半關閉狀態，即主動關閉端還能夠接受資料，但是無法傳送資料 ） 

        CLOSING：等待遠端TCP對連線中斷的確認（如果兩端同時傳送FIN，則在傳送後兩端都進入FIN_WAIT_1狀態。在收到對端的FIN後回覆ACK報文，之後將狀態置為CLOSING，比較少見） 

        TIME_WAIT：等待足夠的時間以確保遠端TCP接收到連線中斷請求的確認（主動關閉端接收到FIN，TCP就傳送ACK包，之後將狀態置為TIME_WAIT）

        LISTEN：偵聽來自遠方的TCP埠的連線請求（伺服器端需開啟一個socket進行監聽，之後將狀態置為LISTEN） 

        SYN_RCVD：在收到和傳送一個連線請求後等待對方對連線請求的確認（當伺服器端收到客戶端的連線請求後，將標誌位ACK和SYN置為1傳送給客戶端，之後將狀態置為SYN_RCVD） 

        CLOSE_WAIT：等待從本地使用者發來的連線中斷請求（被動關閉端接到FIN後，就發出ACK以迴應FIN請求,之後將狀態置為CLOSE_WAIT） 

        LAST_ACK：等待原來的發向遠端TCP的連線中斷請求的確認（被動關閉端在狀態為CLOSE_WAIT一段時間後，將餘下資料傳送完後發回一個FIN請求關閉連線，之後將狀態置為LAST_ACK） 

        CLOSED：沒有任何連線狀態（被動關閉端在接受到主動關閉端ACK包，之後將狀態置為CLOSED，連線結束）

        ESTABLISHED：代表一個開啟的連線（成功建立連線，之後將狀態置為ESTABLISHED，開始傳輸資料）
        
        原文連結：https://read01.com/jgAByD.html

    */

    /// <summary>
    /// 檢查本機連線狀態
    /// </summary>
    public static class ConnectionCheck
    {

        // ===============================================
        // The Method That Parses The NetStat Output
        // And Returns A List Of Port Objects
        // ===============================================
        /// <summary>
        /// 取得主機指定連線狀態
        /// </summary>
        /// <param name="selectIPs">指定的IP,沒資料不過濾</param>
        /// <param name="selectPorts">指定的Ports,沒資料不過濾</param>
        /// <returns></returns>
        public static List<Port> GetNetStatPorts(List<string> selectIPs, List<string> selectPorts)
        {
            var ports = new List<Port>();

            try
            {
                using (Process p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;
                    ps.CreateNoWindow = true;

                    p.StartInfo = ps;
                    p.Start();


                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        // Command Errored. Handle Here If Need Be
                    }

                    //Get The Rows
                    string[] rows = Regex.Split(content, "\r\n");
                    foreach (string row in rows)
                    {
                        //Split it baby
                        string[] tokens = Regex.Split(row, "\\s+");
                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            string goalAddress = Regex.Replace(tokens[3], @"\[(.*?)\]", "1.1.1.1");

                            var localInfo = localAddress.Split(':');
                            var goalInfo = goalAddress.Split(':');

                            ports.Add(new Port
                            {
                                Protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                LocalIP = localInfo[0],
                                Port_number = localInfo[1],
                                Process_name = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5])),
                                State = tokens[1] == "UDP" ? string.Empty : tokens[4],

                                GoalIP = goalInfo[0],
                                GoalPort = goalInfo[1]

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (selectIPs != null && selectIPs.Count > 0)
            {
                ports = ports.Where(x => selectIPs.Contains(x.LocalIP)).ToList();
            }

            if (selectPorts != null && selectPorts.Count > 0)
            {
                ports = ports.Where(x => selectPorts.Contains(x.Port_number)).ToList();
            }

            return ports;
        }

        private static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }


    }


    /// <summary>
    /// ===============================================
    /// The Port Class We're Going To Create A List Of
    /// ===============================================
    /// </summary>
    public class Port
    {
        public string Name
        {
            get
            {
                return string.Format("{0} ({1} port {2})", this.Process_name, this.Protocol, this.Port_number);
            }
            set { }
        }

        public string LocalIP { get; set; }

        public string Port_number { get; set; }


        public string GoalIP { get; set; }

        public string GoalPort { get; set; }

        public string State { get; set; }

        public string Process_name { get; set; }
        public string Protocol { get; set; }
    }
}
