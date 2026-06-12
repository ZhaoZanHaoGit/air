using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class BaseWindowPanel : MonoBehaviour
{
    public virtual void OnShow()
    {
        this.gameObject.SetActive(true);
    }
    public virtual void OnHide()
    {
        this.gameObject.SetActive(false);
    }
    protected string WriteResult(string[] paths)
    {
        string _path = "";
        if (paths.Length == 0)
        {
            return "";
        }
        else
        {
            foreach (var p in paths)
            {
                _path += p;
            }
            return _path;
        }
    }

    protected string GetFileInfo(string path)
    {
        string str = "";
        if (File.Exists(path))
        {
            FileInfo fileInfo = new FileInfo(path);

            // 获取文件名
            string fileName = fileInfo.Name;
            //Debug.Log("File Name: " + fileName);

            // 获取文件大小（字节数）
            long fileSizeBytes = fileInfo.Length;
            //Debug.Log("File Size (Bytes): " + fileSizeBytes);

            // 可以将字节数转换为其他单位（例如 KB、MB、GB）
            float fileSizeKB = fileSizeBytes / 1024f;
            //Debug.Log("File Size (KB): " + fileSizeKB);

            float fileSizeMB = fileSizeKB / 1024f;
            //Debug.Log("File Size (MB): " + fileSizeMB);

            float fileSizeGB = fileSizeMB / 1024f;
            //Debug.Log("File Size (GB): " + fileSizeGB);
            str = fileName + "(" + fileSizeKB.ToString("0.0") + "KB" + ")";           
        }
        else
        {
            Debug.LogError("File does not exist: " + path);
            str = "";
        }
        return str;
    }
    protected float GetFileInfoSize(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        long fileSizeBytes = fileInfo.Length;
        float fileSizeKB = fileSizeBytes / 1024f;
        float fileSizeMB = fileSizeKB / 1024f;
        return fileSizeMB;
    }
    protected string GetFileInfoFileName(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        string fileName = fileInfo.Name;
        return fileName;
    }

    protected bool AreRotationsEqual(string str1, string str2)
    {
        //Debug.Log(str1);
        //Debug.Log(str2);
        if (str1.Length != str2.Length)
        {
            return false;
        }
        char[] chars1 = str1.ToCharArray();
        char[] chars2 = str2.ToCharArray();
        Array.Sort(chars1);
        Array.Sort(chars2);

        // 比较排序后的字符数组是否相等
        return Enumerable.SequenceEqual(chars1, chars2);
    }
    protected List<T> GetRandomElements<T>(List<T> list, int numberOfElements)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < numberOfElements && list.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            result.Add(list[randomIndex]);
            list.RemoveAt(randomIndex);
        }
        return result;
    }
    protected void Reset(Transform root)
    {
        if (root.childCount > 0)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }
    }
    protected bool IsNetworkAddress(string address)
    {
        // 使用正则表达式匹配网络地址的特征
        string pattern = @"^(https?|ftp|file)://";
        if (Regex.IsMatch(address, pattern, RegexOptions.IgnoreCase))
        {
            return true;
        }

        // 检查是否包含域名或IP地址
        pattern = @"^(www\.|[a-zA-Z0-9\-]+\.[a-zA-Z]{2,})";
        if (Regex.IsMatch(address, pattern, RegexOptions.IgnoreCase))
        {
            return true;
        }

        // 检查是否包含IP地址
        pattern = @"^(\d{1,3}\.){3}\d{1,3}";
        if (Regex.IsMatch(address, pattern))
        {
            return true;
        }

        return false;
    }
}
