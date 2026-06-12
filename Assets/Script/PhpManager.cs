using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

// 数据结构
[System.Serializable]
public class ScoreData
{
    public int xh;
    public string name;
    public string idcard;
    public string danwei;
    public string phone;
    public string date;         // "2025-11-29"
    public int total_score;
}

[System.Serializable]
public class ScoreListResponse
{
    public int code;
    public string msg;
    public Data data;
    [System.Serializable]
    public class Data
    {
        public int total;
        public int page;
        public int size;
        public List<ScoreDataItem> list;
    }
}

[System.Serializable]
public class ScoreDataItem
{
    public int id;
    public int xh;
    public string name;
    public string idcard;
    public string danwei;
    public string phone;
    public string date;
    public int total_score;
    public string create_time;
}

public class PhpManager : MonoBehaviour
{
    private string baseUrl = "https://rf.kiwistudio.top/";  // 改成你自己的域名

    // 提交数据
    public IEnumerator SubmitScore(ScoreData data)
    {
        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "submit.php", json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("提交失败：" + www.error);
            }
            else
            {
                Debug.Log("提交成功：" + www.downloadHandler.text);
            }
        }
    }

    // 获取所有数据（默认第一页，每页20条）
    public IEnumerator GetAllScores(System.Action<List<ScoreDataItem>> callback = null)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "get_data.php?page=1&size=50"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("读取失败：" + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                ScoreListResponse resp = JsonUtility.FromJson<ScoreListResponse>(json);
                if (resp.code == 200)
                {
                    callback?.Invoke(resp.data.list);
                    Debug.Log($"成功读取 {resp.data.total} 条数据");
                }
                else
                {
                    Debug.LogError("服务器返回错误：" + resp.msg);
                }
            }
        }
    }
}