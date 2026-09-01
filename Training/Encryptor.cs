using System;

using System.Security.Cryptography;
using System.IO;

public class Encryptor
{
    DESCryptoServiceProvider key = new DESCryptoServiceProvider();
    public static string PrivateKey = "LlU9JJLJAu8=";
    public Encryptor()
    {
       
    }
    public Encryptor(string Key)
    {
        key.IV = Convert.FromBase64String("x7hkVXoObeI=");
        key.Key = Convert.FromBase64String(Key);

    }
    public string Encrypt(string PlainText)
    {

        MemoryStream ms = new MemoryStream();
        CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write);
        StreamWriter sw = new StreamWriter(encStream);
        sw.WriteLine(PlainText);
        sw.Close();
        encStream.Close();
        byte[] buffer = ms.ToArray();
        ms.Close();

        return Convert.ToBase64String(buffer);
    }
    public string Decrypt(string CypherText)
    {
        byte[] bText = Convert.FromBase64String(CypherText);
        MemoryStream ms = new MemoryStream(bText);
        CryptoStream encStream = new CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(encStream);
        string val = sr.ReadLine();
        sr.Close();
        encStream.Close();
        ms.Close();
        return val;
    }

  
}
