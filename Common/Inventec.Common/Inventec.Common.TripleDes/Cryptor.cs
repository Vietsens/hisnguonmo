using System;
using System.Security.Cryptography;
using System.Text;

namespace Inventec.Common.TripleDes
{
    public class Cryptor
    {
        public static string Encrypt(string toEncrypt, string key, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.Key = keyArray;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("00000000"); //Neu CipherMode la CBC thi IV giua encrypt & decrypt phai giong nhau --> FIX

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return BitConverter.ToString(resultArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string EncryptToBase64(string toEncrypt, string key)
        {
            try
            {
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.Key = keyArray;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("00000000"); //Neu CipherMode la CBC thi IV giua encrypt & decrypt phai giong nhau --> FIX

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return Convert.ToBase64String(resultArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Decrypt(string cipherString, string key, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                String[] arr = cipherString.Split('-');
                byte[] toEncryptArray = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) toEncryptArray[i] = Convert.ToByte(arr[i], 16);

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.Key = keyArray;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("00000000");

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return DecryptPrivate(cipherString, key, useHashing);
            }
        }

        private static string DecryptPrivate(string cipherString, string key, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                String[] arr = cipherString.Split('-');
                byte[] toEncryptArray = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) toEncryptArray[i] = Convert.ToByte(arr[i], 16);

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.None;
                tdes.Key = keyArray;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("00000000");

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        public static string DecryptFromBase64(string cipherString, string key)
        {
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.Key = keyArray;
                tdes.IV = UTF8Encoding.UTF8.GetBytes("00000000");

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

    }
}
