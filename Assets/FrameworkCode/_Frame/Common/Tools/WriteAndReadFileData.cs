using Newtonsoft_X.Json;
using System;
using System.IO;

public class WriteAndReadFileData 
{
    public static void WriteToFileExample(Action action) {
        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        string fileName = "TMPUserInfo.tmp";
        string filePath = Path.Combine(myDocumentsPath, fileName);
        PathData data = AppController.Instance.pathDatas.Find(a => a.ID == 11);
        AppController.Instance.loginUser.ServerIP = data.Path;
        string fileContent = JsonConvert.SerializeObject(AppController.Instance.loginUser);
        File.WriteAllText(filePath, fileContent);
        if (File.Exists(filePath))
        {
            string readContent = File.ReadAllText(filePath);
            if (readContent == fileContent)
            {
                action?.Invoke();
            }
            else
            {
                UnityEngine.Debug.Log("文件内容不匹配！");
            }
        }
        else
        {
            UnityEngine.Debug.Log("文件写入失败！");
        }
    }
    public static User ReadFromFileExample() {    
        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        string fileName = "TMPUserInfo.tmp";
        string filePath = Path.Combine(myDocumentsPath, fileName);
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            User user = JsonConvert.DeserializeObject<User>(fileContent);
            File.Delete(filePath);
            return user;
        }
        else
        {
            //UnityEngine.Debug.Log("文件不存在：" + filePath);
            return null;
        }
    }
    public static void WriteToSoftFileExample(string softname,string softpath, Action action)
    {
        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        string fileName = softname+".tmp";
        string filePath = Path.Combine(myDocumentsPath, fileName);
        SoftAttachToPath data = new SoftAttachToPath
        {
            SoftName = softname,
            SoftPath= softpath
        };   
        string fileContent = JsonConvert.SerializeObject(data);
        File.WriteAllText(filePath, fileContent);
        if (File.Exists(filePath))
        {
            string readContent = File.ReadAllText(filePath);
            if (readContent == fileContent)
            {
                action?.Invoke();
            }
            else
            {
                UnityEngine.Debug.Log("文件内容不匹配！");
            }
        }
        else
        {
            UnityEngine.Debug.Log("文件写入失败！");
        }
    }
    public static SoftAttachToPath ReadSoftFromFileExample(string softname)
    {
        string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        string fileName = softname+".tmp";
        string filePath = Path.Combine(myDocumentsPath, fileName);
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            SoftAttachToPath data = JsonConvert.DeserializeObject<SoftAttachToPath>(fileContent);          
            return data;
        }
        else
        {
            UnityEngine.Debug.Log("文件不存在：" + filePath);
            return null;
        }
    }
}
public class SoftAttachToPath { 
    public string SoftName { get; set; }
    public string SoftPath { get; set; }
}