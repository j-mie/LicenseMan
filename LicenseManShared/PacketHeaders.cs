using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManShared
{
    public class PacketHeaders
    {
        public enum Headers
        {
            PublicKey,
            Login,
            Disconnect,
            Chunk,
            RequestAssembly,
            AssemblySettings
        }
    }
}
