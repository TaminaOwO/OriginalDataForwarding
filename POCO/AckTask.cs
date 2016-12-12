using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriginalDataForwarding.POCO
{
    public class AckTask
    {
        public byte[] DataBytes { set; get; }
        public ushort DataType { set; get; }
    }
}
