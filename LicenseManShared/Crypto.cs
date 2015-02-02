using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManShared
{
    public class Crypto
    {
        #region PasswordHashing
        //Thanks - https://github.com/Rohansi/RohBot
        public static byte[] HashPassword(string password, byte[] salt)
        {
            if (salt == null || salt.Length != 16)
                throw new Exception("bad salt");

            var h = new Rfc2898DeriveBytes(password, salt, 1000);
            return h.GetBytes(128);
        }

        private static RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();
        public static byte[] GenerateSalt()
        {
            var salt = new byte[16];
            _random.GetBytes(salt);
            return salt;
        }
        #endregion
        #region RSA Crypto
        // Thanks - http://stackoverflow.com/a/18850104
        private static string ByteToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private static byte[] StringToByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] EncryptToBytes(string publicKey, byte[] data)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

            byte[] encryptedBytes = rsaProvider.Encrypt(data, false);

            return encryptedBytes;
        }

        public static byte[] EncryptToBytes(string publicKey, string data)
        {
            return EncryptToBytes(publicKey, StringToByte(data));
        }

        public static string EncryptToString(string publicKey, string data)
        {
            return Convert.ToBase64String(EncryptToBytes(publicKey, data));
        }

        public static byte[] DecryptToBytes(string privateKey, byte[] encryptedBytes)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

            return plainBytes;
        }

        public static string DecryptToString(string privateKey, byte[] encryptedBytes)
        {
            return ByteToString(DecryptToBytes(privateKey, encryptedBytes));
        }

        public static string DecryptToString(string privateKey, string data)
        {
            byte[] base64 = Convert.FromBase64String(data);
            return ByteToString(DecryptToBytes(privateKey, base64));
        }
        #endregion
    }
}
