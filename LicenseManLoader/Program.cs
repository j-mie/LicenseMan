using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var cm = new Credential { Target = "LicenseMan" };

            if(args.Length >= 1)
            {
                if (args[0] == "-reset")
                {
                    cm.Delete();
                }
            }

            cm.Load();

            if(String.IsNullOrEmpty(cm.Username) || String.IsNullOrEmpty(cm.Password))
            {
                LoginForm lf = new LoginForm();
                lf.ShowDialog();

                cm.Load();
            }

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            LicenseManLoader Loader = new LicenseManLoader(publicKey, privateKey);

            new Thread(() => {
                Loader.Load();
            }).Start();


            Loader.SendPublicKey();
        }
    }
}
