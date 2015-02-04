using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LicenseManLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var cm = new Credential { Target = "LicenseMan", PersistanceType = PersistanceType.LocalComputer };

            if(args.Length >= 1)
            {
                if (args[0] == "reset")
                {
                    cm.Delete();
                }
            }

            cm.Load();

            if (String.IsNullOrWhiteSpace(cm.Username) || String.IsNullOrWhiteSpace(cm.Password))
            {
                LoginForm lf = new LoginForm();
                lf.ShowDialog();

                cm.Load();
            }

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            LicenseManLoader Loader = new LicenseManLoader(publicKey, privateKey, cm.Username, cm.Password);

            var Listener = new Thread(() =>
            {
                Loader.Load();
            });

            Listener.Start();

            Loader.SendPublicKey();

            Thread.Sleep(1000);

            Loader.SendUsernameAndPassword();
        }
    }
}
