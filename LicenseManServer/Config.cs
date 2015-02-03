using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    [Serializable]
    public class Config
    {
        public string ApplicationName = "";
        public int ListenPort = 4560;
        public bool EnableUPnP = false;

        public string FileName = "client.dll";
        public string NameSpaceClass = "ExampleApp.Program";
        public string Method = "Main";
        public bool ExitOnFinish = true;


        public string PrivateKey = "";
        public string PublicKey = "";
    }
}
