using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    class LicenseManServer
    {
        private Config Config;
        private Logger Logger;
        private Server Server;

        public LicenseManServer()
        {
            Logger = LogManager.GetLogger("LicenseManServer");

            if (File.Exists("LicenseMan.config"))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("LicenseMan.config"));
            }
            else
            {
                Logger.Warn("Writing out config file 'LicenseMan.config' - please fill it in");
                Config = new Config();

                CspParameters cspParams = new CspParameters { ProviderType = 1 };
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(4096, cspParams);

                Config.PublicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
                Config.PrivateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

                var Json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText("LicenseMan.config", Json);
                Environment.Exit(10);
            }
        }

        public void Start()
        {
            Logger.Info("Starting LicenseManServer for: {0}", Config.ApplicationName);

            Server = new Server(Config);
            Server.Listen();
        }
    }
}
