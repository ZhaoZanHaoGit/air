using System.Collections;
using UnityEngine;
using SFB;

public class BasicSample : MonoBehaviour {
    private string _path;

    void OnGUI() {
        var guiScale = new Vector3(Screen.width / 800.0f, Screen.height / 600.0f, 1.0f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiScale);

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical();

        // Open File Samples

        if (GUILayout.Button("打开文件")) {
            WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false));
        }
        GUILayout.Space(5);
        if (GUILayout.Button("异步打开文件")) {
            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false, (string[] paths) => { WriteResult(paths); });
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开多个")) {
            WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", "", "", true));
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开文件扩展名")) {
            WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", true));
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开文件目录")) {
            WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", Application.dataPath, "", true));
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开文件过滤器")) {
            var extensions = new [] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Sound Files", "mp3", "wav" ),
                new ExtensionFilter("All Files", "*" ),
            };
            WriteResult(StandaloneFileBrowser.OpenFilePanel("打开文件", "", extensions, true));
        }

        GUILayout.Space(15);

        // Open Folder Samples

        if (GUILayout.Button("打开文件夹")) {
            var paths = StandaloneFileBrowser.OpenFolderPanel("选择文件夹", "", true);
            WriteResult(paths);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开文件夹异步")) {
            StandaloneFileBrowser.OpenFolderPanelAsync("选择文件夹", "", true, (string[] paths) => { WriteResult(paths); });
        }
        GUILayout.Space(5);
        if (GUILayout.Button("打开文件夹目录")) {
            var paths = StandaloneFileBrowser.OpenFolderPanel("选择文件夹", Application.dataPath, true);
            WriteResult(paths);
        }

        GUILayout.Space(15);

        // Save File Samples

        if (GUILayout.Button("保存文件")) {
            _path = StandaloneFileBrowser.SaveFilePanel("保存文件", "", "", "");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("异步保存文件")) {
            StandaloneFileBrowser.SaveFilePanelAsync("保存文件", "", "", "", (string path) => { WriteResult(path); });
        }
        GUILayout.Space(5);
        if (GUILayout.Button("保存文件默认名称")) {
            _path = StandaloneFileBrowser.SaveFilePanel("保存文件", "", "MySaveFile", "");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("保存文件默认名称Ext")) {
            _path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "MySaveFile", "dat");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("保存文件目录")) {
            _path = StandaloneFileBrowser.SaveFilePanel("Save File", Application.dataPath, "", "");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("保存文件过滤器")) {
            // Multiple save extension filters with more than one extension support.
            var extensionList = new [] {
                new ExtensionFilter("Binary", "bin"),
                new ExtensionFilter("Text", "txt"),
            };
            _path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "MySaveFile", extensionList);
        }

        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.Label(_path);
        GUILayout.EndHorizontal();
    }

    public void WriteResult(string[] paths) {
        if (paths.Length == 0) {
            return;
        }

        _path = "";
        foreach (var p in paths) {
            _path += p + "\n";
        }
    }

    public void WriteResult(string path) {
        _path = path;
    }
}
