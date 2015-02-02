using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    // Thanks to http://stackoverflow.com/a/18850104
    internal class Crypto
    {
        public static byte[] Encrypt(string publicKey, string data)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

            return encryptedBytes;
        }

        public static string Decrypt(string privateKey, byte[] encryptedBytes)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

            string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

            return plainText;
        }
    }
}
