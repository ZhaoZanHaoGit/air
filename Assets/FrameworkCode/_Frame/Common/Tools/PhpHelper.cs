using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PhpHelper : DDOLSingleton<PhpHelper>
{
    public override void Init()
    {

    }
    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="texture"></param>
    public void UpLoadTexture(string texturePath, Action<ResponseData> callback)
    {
        StartCoroutine(AsyncUploadTexture(texturePath, callback));
    }
    IEnumerator AsyncUploadTexture(string texturePath, Action<ResponseData> callback)
    {
        string uploadURL = "http://127.0.0.1/VRTMP/PhpLogic/UpLoadTexture.php";
        byte[] imageBytes = File.ReadAllBytes(texturePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("imageFile", imageBytes, "image.jpg", "image/jpeg");
        UnityWebRequest request = UnityWebRequest.Post(uploadURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            if (jsonResponse.Contains("true"))
            {
                ResponseData data = JsonConvert.DeserializeObject<ResponseData>(jsonResponse.TrimStart('\uFEFF', '\u200B'));
                data.path = GetTargetSubstring(uploadURL) + data.path;
                callback?.Invoke(data);
            }
            else
            {
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError($"Network Error: {request.error}");
        }
    }
    private Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }



    #region  下载图片
    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="image"></param>
    /// <param name="path"></param>
    public void LoadTexture(string path, Action<Texture2D> callback)
    {
        StartCoroutine(DownloadImage(path, callback));
    }
    IEnumerator DownloadImage(string path, Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            callback?.Invoke(texture);
        }
        else
        {
            Debug.LogError($"下载图片失败. Error: {request.error}");
            callback?.Invoke(null);
        }
    }
    #endregion 


    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void DeleteTexture(string path, Action<string> callback)
    {
        StartCoroutine(AsyncDeleteTexture(path, callback));
    }
    IEnumerator AsyncDeleteTexture(string path, Action<string> callback)
    {
        string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteTexture.php";
        UnityWebRequest request = UnityWebRequest.Get($"{serverURL}?imageUrl={path}");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            Debug.Log(response);
            callback?.Invoke(response);
        }
        else
        {
            Debug.LogError($"Failed to delete image. Error: {request.error}");
        }
    }
    /// <summary>
    /// 上传视频
    /// </summary>
    /// <param name="videoFilePath"></param>
    /// <param name="callback"></param>
    public void UploadVideo(string videoFilePath, Action callback)
    {
        StartCoroutine(AsyncUploadVideo(videoFilePath, callback));
    }
    IEnumerator AsyncUploadVideo(string videoFilePath, Action callback)
    {
        string uploadURL = "http://127.0.0.1/VRTMP/PhpLogic/UpLoadVideo.php";
        byte[] videoBytes = System.IO.File.ReadAllBytes(videoFilePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("videoFile", videoBytes, "video.mp4", "video/mp4");
        UnityWebRequest request = UnityWebRequest.Post(uploadURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            if (jsonResponse.Contains("true"))
            {
                ResponseData data = JsonConvert.DeserializeObject<ResponseData>(jsonResponse);
                data.path = GetTargetSubstring(uploadURL) + data.path;
                callback?.Invoke();
            }
            else
            {
                callback?.Invoke();
            }
        }
        else
        {
            Debug.LogError($"Network Error: {request.error}");
        }
    }
    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="videoFilePath"></param>
    /// <param name="callback"></param>
    public void DeleteVideo(string videoFilePath, Action<string> callback)
    {
        StartCoroutine(AsyncDeleteVideo(videoFilePath, callback));
    }
    IEnumerator AsyncDeleteVideo(string videoFilePath, Action<string> callback)
    {
        string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteVideo.php";
        UnityWebRequest request = UnityWebRequest.Get($"{serverURL}?videoUrl={videoFilePath}");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            Debug.Log(response);
            callback?.Invoke(response);
        }
        else
        {
            Debug.LogError($"Failed to delete image. Error: {request.error}");
        }
    }
    /// <summary>
    /// 上传.txt文件并返回文件存储路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void UpLoadTxt(string path, Action<ResponseData> callback)
    {
        StartCoroutine(AsyncUpLoadTxt(path, callback));
    }
    IEnumerator AsyncUpLoadTxt(string path, Action<ResponseData> callback)
    {
        string uploadURL = "http://127.0.0.1/VRTMP/PhpLogic/UpLoadTxt.php";
        byte[] fileData = System.IO.File.ReadAllBytes(path);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, "file.txt", "text/plain; charset=utf-8"); // 指定编码为 UTF-8

        UnityWebRequest www = UnityWebRequest.Post(uploadURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (www.downloadHandler.text.Contains("true"))
            {
                ResponseData data = JsonConvert.DeserializeObject<ResponseData>(www.downloadHandler.text.TrimStart('\uFEFF', '\u200B'));
                data.path = GetTargetSubstring(uploadURL) + data.path;
                callback?.Invoke(data);
            }
            else
            {
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError("Upload failed: " + www.error);
        }
    }
    #region 下载文本.txt
    /// <summary>
    /// 下载.txt
    /// </summary>
    /// <param name="text"></param>
    /// <param name="path"></param>
    public void LoadTxt(string path, Action<string> callback)
    {
        StartCoroutine(AsyncLoadTxt(path, callback));
    }
    IEnumerator AsyncLoadTxt(string path, Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            // 下载成功
            //string textContent = www.downloadHandler.text;
            // 手动指定编码进行解码
            byte[] resultBytes = www.downloadHandler.data;
            string decodedText = Encoding.UTF8.GetString(resultBytes);
            callback?.Invoke(decodedText);
        }
        else
        {
            // 下载失败
            Debug.LogError("Download failed: " + www.error);
        }
    }
    #endregion
    /// <summary>
    /// 删除.txt文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void DeleteTxt(string path, Action<string> callback)
    {
        StartCoroutine(AsyncDeleteTxt(path, callback));
    }
    IEnumerator AsyncDeleteTxt(string path, Action<string> callback)
    {
        string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteTxt.php";
        UnityWebRequest request = UnityWebRequest.Get($"{serverURL}?txtUrl={path}");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            Debug.Log(response);
            callback?.Invoke(response);
        }
        else
        {
            Debug.LogError($"Failed to delete image. Error: {request.error}");
        }
    }


    #region  压缩包上传
    /// <summary>
    /// 上传压缩包
    /// </summary>
    /// <param name="path"></param>
    public void UploadZip(string path, Action<ResponseData> callback)
    {
        if (string.IsNullOrEmpty(path)) return;
        StartCoroutine(AsyncUploadZip(path, callback));
    }
    IEnumerator AsyncUploadZip(string filePath, Action<ResponseData> callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/UpLoadZip.php"; // 请填写实际的服务器URL
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 4).Path;
        WWWForm form = new WWWForm();
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        // 添加文件数据和文件类型（根据实际情况修改文件名和Content-Type）
        form.AddBinaryData("file", fileData, "uploadedFile." + GetFileExtension(filePath), GetContentType(filePath));

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            // 使用SendWebRequest替代Send，以支持异步上传
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UpDownLoadMessage._instance.Hide();
                UIManager.Instance.OpenMessageBoxUI("错误提示", "上传文件时发生错误！", 0, EnumMessageBoxType.OK);
            }
            else
            {
                string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
                // Debug.Log("responseText: " + responseText);
                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(responseText);
                if (response.success)
                {
                    response.path = GetTargetSubstring(serverURL) + response.path;
                    callback?.Invoke(response);
                }
                else
                {
                    UpDownLoadMessage._instance.Hide();
                    UIManager.Instance.OpenMessageBoxUI("错误提示", response.message, 0, EnumMessageBoxType.OK);
                }
            }
        }
    }
    // 获取文件扩展名
    private string GetFileExtension(string filePath)
    {
        return System.IO.Path.GetExtension(filePath).TrimStart('.');
    }
    // 获取文件的Content-Type
    private string GetContentType(string filePath)
    {
        // 根据实际情况添加更多文件类型的映射
        switch (GetFileExtension(filePath))
        {
            case "zip":
                return "application/zip";
            case "rar":
                return "application/x-rar-compressed";
            default:
                return "application/octet-stream";
        }
    }
    #endregion
    #region 删除压缩包
    public void DeleteZipOnServer(string fileNameToDelete, Action callback)
    {
        StartCoroutine(DeleteFileCoroutine(fileNameToDelete, callback));
    }
    IEnumerator DeleteFileCoroutine(string fileNameToDelete, Action callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteZip.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 5).Path;
        WWWForm form = new WWWForm();
        form.AddField("filename", fileNameToDelete);

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Delete File Error: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B'); ;

                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(responseText);
                if (response.success)
                {
                    callback?.Invoke();
                }
                else
                {
                    UIManager.Instance.OpenMessageBoxUI("错误提示", response.message, 0, EnumMessageBoxType.OK);
                    callback?.Invoke();
                }
            }
        }
    }
    #endregion
   
    #region 批量上传文件
    
    public void OnUploadFiles(List<string> paths, string uploadDirectory, Action<string, string, FileType> callback, List<FileItem> fileItems = null, bool IsMySelfName = false)
    {
        StartCoroutine(UploadFiles(paths, uploadDirectory, callback, fileItems, IsMySelfName));
    }
    IEnumerator UploadFiles(List<string> paths, string uploadDirectory, Action<string, string, FileType> callback, List<FileItem> fileItems = null, bool IsMySelfName = false)
    {
        if (fileItems != null)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                yield return StartCoroutine(UploadFile(paths[i], uploadDirectory, callback, fileItems[i], IsMySelfName));
            }
        }
        else
        {
            for (int i = 0; i < paths.Count; i++)
            {
                yield return StartCoroutine(UploadFile(paths[i], uploadDirectory, callback, null, IsMySelfName));
            }
        }
    }
    IEnumerator UploadFile(string filePath, string uploadDirectory, Action<string, string, FileType> callback, FileItem item = null, bool IsMySelfName = false)
    {
        FileType fileType = FileType.None;
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/UploadFiles.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 7).Path;
        WWWForm form = new WWWForm();
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        string fileName = System.IO.Path.GetFileName(filePath);
        //Debug.Log(filePath);
        //Debug.Log(fileName);
        // 显式设置 boundary
        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        form.headers["Content-Type"] = "multipart/form-data; boundary=" + boundary;

        // 添加文件数据和文件类型
        form.AddBinaryData("file", fileData, "uploadedFile." + GetFileExtension(filePath), ContentType(fileName, ref fileType));
        // 添加上传目录信息
        form.AddField("uploadDirectory", uploadDirectory);

        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);

        // 设置上传处理程序使用字节数组
        www.uploadHandler = new UploadHandlerRaw(form.data);

        // 启动上传
        www.SendWebRequest();
        long totalBytes = fileData.Length;
        long uploadedBytes = 0;
        float progress = 0;
        while (!www.isDone)
        {
            uploadedBytes = (long)www.uploadedBytes;
            progress = (float)uploadedBytes / totalBytes;
            if (item != null && !IsMySelfName)
            {
               // item.UpdateUpLoadprogress(progress);
            }

            //UpDownLoadMessage._instance.ChanagedValue(progress);
            yield return null;
        }
        if (www.result != UnityWebRequest.Result.Success)
        {
            //UpDownLoadMessage._instance.Hide();
            UIManager.Instance.OpenMessageBoxUI("错误提示", "网络错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            if (progress < 1 || progress > 1)
            {
                progress = 1;
            }
            if (item != null && !IsMySelfName)
            {
                // item.UpdateUpLoadprogress(progress);
            }
            //UpDownLoadMessage._instance.Hide();
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(responseText);
            if (response.success)
            {
                response.path = GetTargetSubstring(serverURL) + response.path;
                if (item != null)
                {
                    //callback?.Invoke(item.Data.FileName, response.path, fileType);
                }
                else
                {
                    callback?.Invoke(response.name, response.path, fileType);
                }
                //Debug.Log($"{item.Data.FileName}--{response.path}--{fileType.ToString()}");
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "无法上传文件！", 0, EnumMessageBoxType.OK);
                callback?.Invoke("", "", fileType);
            }
        }
    }

    [Serializable]
    public class FileUploadResponse
    {
        public bool success;
        public string message;
        public string folderPath;
    }
    // 获取文件的Content-Type
    private string ContentType(string fileName, ref FileType fileType)
    {
        string extension = System.IO.Path.GetExtension(fileName).ToLower();

        switch (extension)
        {
            case ".zip":
                fileType = FileType.ZIP;
                return "application/zip";
            case ".rar":
                fileType = FileType.RAR;
                return "application/x-rar-compressed";
            case ".mp4":
                fileType = FileType.MP4;
                return "video/mp4";
            case ".jpg":
                fileType = FileType.JPG;
                return "image/jpg";
            case ".jpeg":
                fileType = FileType.JPEG;
                return "image/jpeg";
            case ".png":
                fileType = FileType.PNG;
                return "image/png";
            case ".txt":
                fileType = FileType.TXT;
                return "text/plain";
            case ".pdf":
                fileType = FileType.PDF;
                return "application/pdf";
            case ".doc":
                fileType = FileType.DOC;
                return "application/msword";
            case ".docx":
                fileType = FileType.DOC;
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case ".xls":
                fileType = FileType.XLS;
                return "application/vnd.ms-excel";
            case ".xlsx":
                fileType = FileType.XLS;
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            case ".wav":
                fileType = FileType.WAV;
                return "audio/wav";
            case ".mp3":
                fileType = FileType.MP3;
                return "audio/mpeg";
            case ".ogg":
                fileType = FileType.OGG;
                return "audio/ogg";
            // 添加更多文件类型...
            default:
                return "application/octet-stream"; // 默认类型
        }
    }
    
    #endregion
    
    #region 创建文件夹
    public void OnCreateFolder(Action<string, string> callback)
    {
        StartCoroutine(CreateFolder(callback));
    }
    IEnumerator CreateFolder(Action<string, string> callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/CreateFolder.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 7).Path;
        UnityWebRequest www = UnityWebRequest.Get(serverURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "上传文件时发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            FolderCreationResponse response = JsonUtility.FromJson<FolderCreationResponse>(responseText);

            if (response.success)
            {
                callback?.Invoke(response.folderName, response.folderPath);
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "无法创建文件！", 0, EnumMessageBoxType.OK);
            }
        }
    }
    [System.Serializable]
    public class FolderCreationResponse
    {
        public bool success;
        public string message;
        public string folderName;
        public string folderPath;

    }

    //创建课程资源文件夹
    public void OnCreateCourseResourceFolder(Action<string, string> callback)
    {
        StartCoroutine(CreateCourseResourceFolder(callback));
    }
    IEnumerator CreateCourseResourceFolder(Action<string, string> callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/CreateFolder.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 12).Path;
        UnityWebRequest www = UnityWebRequest.Get(serverURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "上传文件时发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            FolderCreationResponse response = JsonUtility.FromJson<FolderCreationResponse>(responseText);

            if (response.success)
            {
                callback?.Invoke(response.folderName, response.folderPath);
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "无法创建文件！", 0, EnumMessageBoxType.OK);
            }
        }
    }

    //创建软件缩略图文件夹
    public void OnCreateSoftWareTextureFolder(Action<string, string> callback)
    {
        StartCoroutine(CreateSoftWareTextureFolder(callback));
    }
    IEnumerator CreateSoftWareTextureFolder(Action<string, string> callback)
    {
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 10).Path;
        UnityWebRequest www = UnityWebRequest.Get(serverURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "上传文件时发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            FolderCreationResponse response = JsonUtility.FromJson<FolderCreationResponse>(responseText);

            if (response.success)
            {
                callback?.Invoke(response.folderName, response.folderPath);
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "无法创建文件！", 0, EnumMessageBoxType.OK);
            }
        }
    }

    #endregion
    #region 删除文件夹及文件夹中所有文件 directoryToDelete为删除的文件夹名称
    public void OnDeleteDirectory(string directoryToDelete, Action<bool> callback)
    {
        StartCoroutine(DeleteDirectory(directoryToDelete, callback));
    }
    IEnumerator DeleteDirectory(string directoryToDelete, Action<bool> callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteDirectory.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 8).Path;
        // 创建表单
        WWWForm form = new WWWForm();
        form.AddField("directoryToDelete", directoryToDelete);
        // 创建 UnityWebRequest 对象
        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        // 发送请求
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "删除文件夹发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            DeleteDirectoryResponse response = JsonConvert.DeserializeObject<DeleteDirectoryResponse>(responseText);
            callback?.Invoke(response.success);
        }
    }
    [System.Serializable]
    public class DeleteDirectoryResponse
    {
        public bool success;
        public string message;
    }
    #endregion
    static string GetTargetSubstring(string originalString)
    {
        // 找到最后一个 '/' 的索引
        int lastIndex = originalString.LastIndexOf('/');

        // 截取字符串，包括最后一个 '/'
        string targetSubstring = originalString.Substring(0, lastIndex + 1);

        return targetSubstring;
    }
    #region 创建资源文件夹
    //public void CreatResourceFolders(Action<ResourcesFolders> callback)
    //{
    //    StartCoroutine(AnyacCreatResourceFolders(callback));
    //}
    //IEnumerator AnyacCreatResourceFolders(Action<ResourcesFolders> callback)
    //{
    //    //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/CreateResourceFolders.php";
    //    string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 9).Path;
    //    UnityWebRequest www = UnityWebRequest.Get(serverURL);
    //    yield return www.SendWebRequest();

    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("Failed to send request. Error: " + www.error);
    //        UIManager.Instance.OpenMessageBoxUI("错误信息", "创建资源文件失败！" + www.error, 0, EnumMessageBoxType.OK);
    //    }
    //    else
    //    {
    //        string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
    //        ResourcesFolders response = JsonConvert.DeserializeObject<ResourcesFolders>(responseText);
    //        //Debug.Log(response.ModelFolder.success);
    //        //Debug.Log(response.ImageFolder.success);
    //        if (response.MainFolder.success && response.ModelFolder.success && response.ImageFolder.success && response.AudioFolder.success &&
    //            response.VideoFolder.success && response.CharacterFolder.success && response.SkyboxFolder.success)
    //        {
    //            callback?.Invoke(response);
    //        }
    //        else
    //        {
    //            callback?.Invoke(null);
    //        }
    //    }
    //}
    #endregion
    #region 删除资源文件
    public void DeleteRescourceFolder(string directoryToDelete, Action<bool> callback)
    {
        StartCoroutine(SyncDeleteRescourceFolder(directoryToDelete, callback));
    }
    IEnumerator SyncDeleteRescourceFolder(string directoryToDelete, Action<bool> callback)
    {
        //string serverURL = "http://127.0.0.1/VRTMP/PhpLogic/DeleteResourceFolder.php";
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 10).Path;
        // 创建表单
        WWWForm form = new WWWForm();
        form.AddField("directoryToDelete", directoryToDelete);
        // 创建 UnityWebRequest 对象
        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        // 发送请求
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "删除文件夹发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            DeleteDirectoryResponse response = JsonConvert.DeserializeObject<DeleteDirectoryResponse>(responseText);
            callback?.Invoke(response.success);
        }
    }
    #endregion
    #region 删除课程资源文件
    public void DeleteCourseRescourceFolder(string directoryToDelete, Action<bool> callback)
    {
        StartCoroutine(SyncDeleteCourseRescourceFolder(directoryToDelete, callback));
    }
    IEnumerator SyncDeleteCourseRescourceFolder(string directoryToDelete, Action<bool> callback)
    {
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 13).Path;
        // 创建表单
        WWWForm form = new WWWForm();
        form.AddField("directoryToDelete", directoryToDelete);
        // 创建 UnityWebRequest 对象
        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        // 发送请求
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "删除文件夹发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            DeleteDirectoryResponse response = JsonConvert.DeserializeObject<DeleteDirectoryResponse>(responseText);
            callback?.Invoke(response.success);
        }
    }
    #endregion

    #region 删除软件图片文件
    public void DeleteSoftWareTxtureFolder(string directoryToDelete, Action<bool> callback)
    {
        StartCoroutine(SyncDeleteSoftWareTxtureFolder(directoryToDelete, callback));
    }
    IEnumerator SyncDeleteSoftWareTxtureFolder(string directoryToDelete, Action<bool> callback)
    {
        string serverURL = AppController.Instance.pathDatas.Find(a => a.ID == 11).Path;
        // 创建表单
        WWWForm form = new WWWForm();
        form.AddField("directoryToDelete", directoryToDelete);
        // 创建 UnityWebRequest 对象
        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        // 发送请求
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "删除文件夹发生错误！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            string responseText = www.downloadHandler.text.TrimStart('\uFEFF', '\u200B');
            DeleteDirectoryResponse response = JsonConvert.DeserializeObject<DeleteDirectoryResponse>(responseText);
            callback?.Invoke(response.success);
        }
    }
    #endregion

    #region 删除文件（对外调用入口）
    public void DeleteFilesOnServer(List<string> filePathsToDelete, Action<DeleteResponse> callback)
    {
        StartCoroutine(DeleteFilesCoroutine(filePathsToDelete, callback));
    }

    public void DeleteFilesOnServer(string[] filePathsArray, Action<DeleteResponse> callback)
    {
        List<string> filePathsList = new List<string>(filePathsArray);
        DeleteFilesOnServer(filePathsList, callback);
    }
    #endregion

    #region 删除请求逻辑
    private IEnumerator DeleteFilesCoroutine(List<string> filePathsToDelete, Action<DeleteResponse> callback)
    {
        string phpDeletionURL = AppController.Instance.pathDatas.Find(a => a.ID == 8).Path;
        // 打印调试信息
        //foreach (var item in filePathsToDelete)
        //{
        //    Debug.Log("待删除文件路径: " + item);
        //}

        // 用容器类打包
        DeleteRequest requestData = new DeleteRequest { filesToDelete = filePathsToDelete };
        string jsonPayload = JsonUtility.ToJson(requestData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(phpDeletionURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(payloadBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

#if UNITY_2020_3_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError($"批量删除请求失败: {request.error}。响应: {request.downloadHandler?.text}");
                callback?.Invoke(null);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                //Debug.Log($"服务器响应: {jsonResponse}");

                // 反序列化 PHP 返回结果
                DeleteResponse response = JsonUtility.FromJson<DeleteResponse>(jsonResponse);
                callback?.Invoke(response);
            }
        }
    }
    #endregion
    #region 数据结构
    [Serializable]
    public class DeleteRequest
    {
        public List<string> filesToDelete;
    }

    [Serializable]
    public class DeleteResponse
    {
        public bool success;
        public string message;
        public int total_files;
        public int success_count;
        public int fail_count;
        public string timestamp;
        public Dictionary<string, DeleteResult> details; // 注意：Unity JsonUtility 不支持 Dictionary，需用其他库
    }

    [Serializable]
    public class DeleteResult
    {
        public string status;
        public string reason;
        public string server_path;
        public string permissions;
        public string error;
    }
    #endregion
}




