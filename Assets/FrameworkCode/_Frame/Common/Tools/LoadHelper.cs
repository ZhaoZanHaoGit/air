using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadHelper : DDOLSingleton<LoadHelper>
{

    public override void Init()
    {

    }
    /// <summary>
    /// 下载文件到指定目录
    /// </summary>
    /// <param name="downloadUrl"></param>
    /// <param name="saveDrivePath"></param>
    /// <returns></returns>
    public void DownLoadFile(string downloadUrl, string saveDrivePath, string saveName, Action callback,bool isTishi)
    {
        Debug.Log(downloadUrl);
        Debug.Log(saveDrivePath);
        Debug.Log(saveName);
        UpDownLoadMessage._instance.Show("数据下载中......");
        StartCoroutine(DownloadFile(downloadUrl, saveDrivePath, saveName, callback, isTishi));
    }
    IEnumerator DownloadFile(string downloadUrl, string saveDrivePath, string saveName, Action callback, bool isTishi)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
        {
            // 发送请求
            UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();

            // 等待下载过程
            while (!webRequest.isDone)
            {           
                UpDownLoadMessage._instance.ChanagedValue(webRequest.downloadProgress);
                yield return null;
            }

            // 检查是否有错误发生
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                UpDownLoadMessage._instance.Hide();
                UIManager.Instance.OpenMessageBoxUI("提示", "下载失败！", 0, EnumMessageBoxType.OK);
            }
            else
            {
                UpDownLoadMessage._instance.Hide();
                  // 处理下载的数据
                 //Debug.Log("下载完成!");               
                if (!Directory.Exists(saveDrivePath))
                {
                    Directory.CreateDirectory(saveDrivePath);
                }
                // 获取文件名
                string fileName = saveName;
                // 保存下载的文件到指定路径
                string filePath = System.IO.Path.Combine(saveDrivePath, fileName);
                System.IO.File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
                // Debug.Log("下载成功！" + filePath);
                if (isTishi) {
                    UIManager.Instance.OpenMessageBoxUI("提示", "下载成功！", 0, EnumMessageBoxType.OK);
                }               
                callback?.Invoke();
            }
        }
    }
}
