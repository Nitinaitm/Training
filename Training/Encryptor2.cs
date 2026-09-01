using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Training
{
    public class Encryptor2
    {

        //private DESCryptoServiceProvider key = new DESCryptoServiceProvider();
        //public static string PrivateKey = "BsphclAparP=";
        private DESCryptoServiceProvider key = new DESCryptoServiceProvider();
       // public static string PrivateKey = "BsphclA1";  // Use the first 8 characters of your desired key
        public static string PrivateKey = "BsphclT1";  // Use the first 8 characters of your desired key

        public Encryptor2()
        {
            key.Key = System.Text.Encoding.ASCII.GetBytes(PrivateKey);  // Convert to ASCII bytes (8 bytes)
                                                                        //key.IV = Convert.FromBase64String("MTIzNDU2Nzg=");  // Use a valid 8-byte IV in Base64
           // key.IV = Convert.FromBase64String("Cert@123");
            key.IV = Encoding.UTF8.GetBytes("Cert@123");
        }


        public string Encrypt(string PlainText)
        {
            key.GenerateIV(); // Generate a new IV for every encryption
            byte[] iv = key.IV; // Store the generated IV

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length); // Prepend IV to the ciphertext

                using (CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(encStream))
                {
                    sw.WriteLine(PlainText);
                }

                byte[] encryptedData = ms.ToArray();
                return Convert.ToBase64String(encryptedData); // Convert to Base64 for easy storage
            }
        }

        public string Decrypt(string CypherText)
        {
            byte[] encryptedData = Convert.FromBase64String(CypherText);

            using (MemoryStream ms = new MemoryStream(encryptedData))
            {
                byte[] iv = new byte[8]; // DES uses an 8-byte IV
                ms.Read(iv, 0, iv.Length); // Extract the IV from the beginning

                key.IV = iv; // Set the extracted IV

                using (CryptoStream encStream = new CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(encStream))
                {
                    return sr.ReadLine(); // Read the decrypted text
                }
            }
        }
    }
}