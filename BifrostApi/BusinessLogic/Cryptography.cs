using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BifrostApi.BusinessLogic
{
    public class Cryptography
    {
        private const int KEY_SIZE = 128;
        private const int ITERATIONS = 100000;

        // Encryption used AES256
        public static string AESEncrypt(string plaintext, string password)
        {
            var saltbytes = Generate256RandomBits();
            var IVBytes = Generate256RandomBits(); // Initialization Vector
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            using (var cipher = new Rfc2898DeriveBytes(password, saltbytes, ITERATIONS))
            {
                var keybytes = cipher.GetBytes(KEY_SIZE / 8);

                using (var symkey = new RijndaelManaged())
                {
                    // AES with a blocksize of 256
                    symkey.BlockSize = 128;
                    symkey.Mode = CipherMode.CBC;
                    symkey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symkey.CreateEncryptor(keybytes, IVBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptstream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptstream.Write(plaintextBytes, 0, plaintextBytes.Length);
                                cryptstream.FlushFinalBlock();

                                // Concat key in following format: SALT+IV+ENCRYPTEDPASSWORD
                                var cipherpass = saltbytes;
                                cipherpass = cipherpass.Concat(IVBytes).ToArray();
                                cipherpass = cipherpass.Concat(memoryStream.ToArray()).ToArray();

                                return HttpUtility.UrlEncode(Convert.ToBase64String(cipherpass));
                            }
                        }
                    }
                }
            }
        }

        public static string AESDecrypt(string ciphertext, string password)
        {
            var rawCipher = Convert.FromBase64String(ciphertext);
            var saltBytes = rawCipher.Take(KEY_SIZE / 8).ToArray();
            var IVBytes = rawCipher.Skip(KEY_SIZE / 8).Take(rawCipher.Length - ((KEY_SIZE / 8) * 2)).ToArray();
            var encryptedPassword = rawCipher.Skip(KEY_SIZE / 8 * 2).Take(rawCipher.Length - ((KEY_SIZE / 8) * 2)).ToArray();

            using (var cipher = new Rfc2898DeriveBytes(password, saltBytes, ITERATIONS))
            {
                var keybytes = cipher.GetBytes(KEY_SIZE / 8);

                using (var symkey = new RijndaelManaged())
                {
                    // AES with a blocksize of 256
                    symkey.BlockSize = 128;
                    symkey.Mode = CipherMode.CBC;
                    symkey.Padding = PaddingMode.PKCS7;

                    using (var decryptor = symkey.CreateDecryptor(keybytes, IVBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptstream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plaintextBytes = new byte[encryptedPassword.Length];
                                var decryptedByteCount = cryptstream.Read(plaintextBytes, 0, plaintextBytes.Length);
                                memoryStream.Close();
                                cryptstream.Close();


                                return Encoding.UTF8.GetString(plaintextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        public static string GenerateSalt()
        {
            return Convert.ToBase64String(Generate256RandomBits());
        }

        private static byte[] Generate256RandomBits()
        {
            var bytes = new byte[16]; // 256 bits

            using (var cipher = new RNGCryptoServiceProvider())
            {
                cipher.GetBytes(bytes);
            }

            return bytes;

        }

        public static string HashPassword(string password, string salt)
        {
            byte[] saltbytes = Encoding.UTF8.GetBytes(salt);
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltbytes,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: 512 / 8));

            return hash;
        }

        public static bool IsAuthenticated(string plaintextPassword, string encryptedPassword, string salt)
        {
            string givenPassword = HashPassword(plaintextPassword, salt);

            if (encryptedPassword == givenPassword)
                return true;
            return false;
        }
    }
}
