using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    [Serializable]
    class Client
    {
        internal string Username = "";
        internal string Password = "";

        internal bool OwnsCopy = false;

        internal string PublicKey;
    }
}
