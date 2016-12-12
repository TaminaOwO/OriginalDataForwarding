using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginalDataForwarding.POCO
{
    /// <summary>
    /// 模仿證交所的心跳
    /// </summary>
    public class TelegramHeartbeat
    {
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="inBytes"></param>
        public TelegramHeartbeat( byte[] inBytes )
        {

        }

        /// <summary>
        /// 開頭 (ASCII Code)
        /// </summary>
        private byte fESCCode = 27;

        /// <summary>
        /// 開頭 (ASCII Code)
        /// </summary>
        public byte ESCCode
        {
            get
            {
                return fESCCode;
            }

            set
            {
                fESCCode = value;
            }
        }

        /// <summary>
        /// 結束 (HEXA Code)
        /// </summary>
        private byte[] fTerminalCode = new byte[2] { 0x0D, 0x0A};
       
        /// <summary>
        /// 結束 (HEXA Code)
        /// </summary>
        public byte[] TerminalCode
        {
            get
            {
                return fTerminalCode;
            }

            set
            {
                fTerminalCode = value;
            }
        }

        /// <summary>
        /// 將屬性存入要發送的陣列
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] result = new byte[17];
            int index = 0;
            byte[] data;

            data = new byte[1] { fESCCode };
            Array.Copy( data, 0, result, index, data.Length );
            index += 1;

            //中間內容不填，因為只是個假的~
            index += 14;

            data = fTerminalCode;
            Array.Copy( data, 0, result, index, data.Length );
            index += 2;

            return result; 
                
        }
    }
}
