using System;
using System.Net;
using System.Net.Sockets;

namespace OriginalDataForwarding.Modules.TCPListener
{
    public class ProxyClient
    {
        public ProxyClient( TcpClient client )
        {
            ClientSocket = client;
            IPEndPoint ipep = client.Client.RemoteEndPoint as IPEndPoint;
            Address = ipep.Address.ToString();
            ClientStream = client.GetStream();
            ConnectedStamp = DateTime.Now;
            ReadBytes = new byte[BytesLength];
        }


        /// <summary>
        /// 連線請求時間
        /// </summary>
        public DateTime ConnectedStamp
        {
            private set; get;
        }

        /// <summary>
        /// 對方連入位置
        /// </summary>
        public string Address
        {
            private set; get;
        }

        /// <summary>
        /// HTS端socket
        /// </summary>
        public TcpClient ClientSocket
        {
            set; get;
        }

        /// <summary>
        /// 該socket的串流
        /// </summary>
        public NetworkStream ClientStream
        {
            set; get;
        }

        /// <summary>
        /// buffer reads
        /// </summary>
        public byte[] ReadBytes
        {
            set; get;
        }

        /// <summary>
        /// buffer length
        /// </summary>
        public static int BytesLength = 4096;
    }
}