/*

Captcha Asp.net control.
Developed by: Aref Karimi
Email: Arefkr@gmail.com
Last update: 20 October 2009

This code is free to use. 
 
*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// This class includes two methods to encrypt and decrypt texts with a symmetric algorithm.
/// </summary>
public static class SecurityHelper
{
    private static byte[] SymmetricKey
    {
        get
        {
            return Encoding.UTF8.GetBytes("2B3c3D4e5F6g5H81");
        }
    }
    public static byte[] EncryptString(string data)
    {
        byte[] ClearData = Encoding.UTF8.GetBytes(data);
        SymmetricAlgorithm Algorithm = SymmetricAlgorithm.Create();
        Algorithm.Key = SymmetricKey;

        MemoryStream Target = new MemoryStream();
        Algorithm.GenerateIV();
        Target.Write(Algorithm.IV, 0, Algorithm.IV.Length);
        CryptoStream cs = new CryptoStream(Target, Algorithm.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(ClearData, 0, ClearData.Length);
        cs.FlushFinalBlock();
        return Target.ToArray();
    }
    public static string DecryptString(byte[] data)
    {
        SymmetricAlgorithm Algorithm = SymmetricAlgorithm.Create();
        Algorithm.Key = SymmetricKey;
        MemoryStream Target = new MemoryStream();
        int ReadPos = 0;
        byte[] IV = new byte[Algorithm.IV.Length];
        Array.Copy(data, IV, IV.Length);
        Algorithm.IV = IV;
        ReadPos += Algorithm.IV.Length;
        CryptoStream cs = new CryptoStream(Target,
        Algorithm.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(data, ReadPos, data.Length - ReadPos);
        cs.FlushFinalBlock();
        return Encoding.UTF8.GetString(Target.ToArray());
    }
}

