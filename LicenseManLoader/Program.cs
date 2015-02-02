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
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            #if !DEBUG
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            #endif

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

            LicenseManLoader Loader = new LicenseManLoader(publicKey, privateKey, cm.Username, cm.Password);

            var Listener = new Thread(() =>
            {
                Loader.Load();
            });

            Listener.Start();

            Loader.SendPublicKey();

            Thread.Sleep(350);

            Loader.SendUsernameAndPassword();

            Thread.Sleep(350);

            Listener.Abort();
        }
    }
}
