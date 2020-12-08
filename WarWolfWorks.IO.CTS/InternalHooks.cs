using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WarWolfWorks.IO.CTS
{
    internal static class InternalHooks
    {
        internal const int Keysize = 256;
        internal const int DerivationIterations = 1000;
        internal static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32]; // 32 Bytes => 256 bits.
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        internal static string EncryptingKey(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        internal static string Encrypt(string input, string password)
        {
            try
            {
                password = EncryptingKey(password);

                byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
                byte[] ivStringBytes = Generate256BitsOfRandomEntropy();
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
                using (Rfc2898DeriveBytes passwordIntern = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = passwordIntern.GetBytes(Keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = Keysize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    byte[] cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                return input;
            }
        }

        internal static string Decrypt(string input, string password)
        {
            try
            {
                password = EncryptingKey(password);

                byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(input);
                byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (Rfc2898DeriveBytes passwordInternal = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations))
                {
                    byte[] keyBytes = passwordInternal.GetBytes(Keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = Keysize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                return input;
            }
        }
    }
}
