using System;
using System.IO;
using System.Security.Cryptography;

namespace Utils
{
    public static class Crypto
    {
        public static (string Hash, string Salt) Encrypt(string plainText, string password)
        {
            string e = null;
            string salt = null;
            RijndaelManaged aes = null;

            try
            {
                salt = genSalt();
                var key = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt));

                aes = new RijndaelManaged();
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = genIV();

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);

                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }

                    e = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            finally
            {
                aes?.Clear();
            }

            return (e, salt);
        }

        public static string Decrypt(string cipherText, string password, string salt)
        {
            string d = null;
            RijndaelManaged aes = null;

            try
            {
                var key = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt));

                var bytes = Convert.FromBase64String(cipherText);

                using (var memoryStream = new MemoryStream(bytes))
                {
                    aes = new RijndaelManaged();
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = readByteArray(memoryStream);

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            d = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                aes?.Clear();
            }

            return d;
        }

        #region Helper methods

        private static byte[] readByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new SystemException("Stream did not contain properly formatted byte array");

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException("Did not read byte array properly");

            return buffer;
        }

        private static string genSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[32];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        private static byte[] genIV()
        {
            var rng = new RNGCryptoServiceProvider();
            var IV = new byte[16];
            rng.GetBytes(IV);
            return IV;
        }

        #endregion
    }
}