using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class GlobalPathManager 
{
    //public static GlobalPathManager Instance { get; private set; }

    public string SelectedPath { get; private set; }

    public string studentname { get; private set; }
    public string studentnumber { get; private set; }

    static string keyString = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6";
    static string ivString = "Q7R8S9T0U1V2W3X4";

    // 将字符串密钥和IV转换为字节序列
    private static readonly byte[] keyBytes = Encoding.UTF8.GetBytes(keyString);
    private static readonly byte[] ivBytes = Encoding.UTF8.GetBytes(ivString);

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject); // 确保单例对象在加载新场景时不被销毁
    //    }
    //    else
    //    {
    //        Destroy(gameObject); // 如果已经存在实例，销毁新的对象
    //    }
    //    SelectedPath = Application.persistentDataPath;
    //}

    public void SetSelectedPath(string path)
    {
        SelectedPath = path;
        // 你可以在这里添加额外的逻辑，比如保存路径到 PlayerPrefs
    }
    public void SetStudentName(string path)
    {
        studentname = path;
        // 你可以在这里添加额外的逻辑，比如保存路径到 PlayerPrefs
    }
    public void SetStudentNum(string path)
    {
        studentnumber = path;
        // 你可以在这里添加额外的逻辑，比如保存路径到 PlayerPrefs
    }


    /// <summary>
    /// 生成加密文件到指定目录
    /// </summary>
    /// <param name="plaintext">要写入的内容</param>
    /// <param name="filePath">文件存放目录</param>
    public static void EncryptDataAndSaveToFile(string plaintext, string filePath)
    {
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                    }
                    string encryptedData = Convert.ToBase64String(msEncrypt.ToArray());
                    File.WriteAllText(filePath, encryptedData);
                }
            }
            Debug.Log("Data encrypted and saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred during encryption: {ex.Message}");
        }
    }
    /// <summary>
    /// 解码文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string DecryptDataFromFile(string filePath)
    {
        try
        {
            string encryptedData = File.ReadAllText(filePath);
            byte[] buffer = Convert.FromBase64String(encryptedData);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred during decryption: {ex.Message}");
        }
        return string.Empty;
    }


    /// <summary>
    /// 解码文件
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string DecryptDataFromText(string Text)
    {
        try
        { 
            byte[] buffer = Convert.FromBase64String(Text);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred during decryption: {ex.Message}");
        }
        return string.Empty;
    }


    // 例如，添加一个方法用于保存数据到选定的路径
    public void SaveDataToFile(string fileName, byte[] data)
    {
        if (!string.IsNullOrEmpty(SelectedPath))
        {
            string fullPath = System.IO.Path.Combine(SelectedPath, fileName);
            System.IO.File.WriteAllBytes(fullPath, data);
            Debug.Log($"Data saved to: {fullPath}");
        }
        else
        {
            Debug.LogError("Path not selected.");
        }
    }

    // 其他全局方法...
}
