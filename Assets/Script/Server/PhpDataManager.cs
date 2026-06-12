using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class PhpDataManager : MonoBehaviour
{
    private string configFilePath;
    public Config config;

    IEnumerator Start()
    {
        configFilePath = Path.Combine(Application.streamingAssetsPath, "config.json");

#if UNITY_WEBGL || UNITY_ANDROID
        yield return LoadConfigByRequest(configFilePath);
#else
        LoadConfigByFile(configFilePath);
        yield break;
#endif
    }

    void LoadConfigByFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("配置文件不存在：" + path);
            return;
        }

        string json = File.ReadAllText(path);
        Debug.Log("JSON Content: " + json);
        config = JsonUtility.FromJson<Config>(json);
        Debug.Log("服务器地址: " + config.serverAddress);
    }

    IEnumerator LoadConfigByRequest(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"读取配置失败: {www.error}, url={url}");
                yield break;
            }

            string json = www.downloadHandler.text;
            Debug.Log("JSON Content: " + json);
            config = JsonUtility.FromJson<Config>(json);
            Debug.Log("服务器地址: " + config.serverAddress);
        }
    }

    // 提交数据
    public IEnumerator SubmitData(string name, string idcard, string company,
                                  string phone, int totalScore, string date = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("idcard", idcard);
        form.AddField("company", company);
        form.AddField("phone", phone);
        form.AddField("total_score", totalScore);
        if (!string.IsNullOrEmpty(date)) form.AddField("date", date);

        using (UnityWebRequest www = UnityWebRequest.Post(config.serverAddress + "submit.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.LogError(www.error);
            else Debug.Log("提交结果: " + www.downloadHandler.text);
        }
    }

    public IEnumerator GetAllData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(config.serverAddress + "get_all.php"))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.LogError(www.error);
            else Debug.Log("所有数据: " + www.downloadHandler.text);
        }
    }
}

[System.Serializable]
public class Config
{
    public string serverAddress;
}
