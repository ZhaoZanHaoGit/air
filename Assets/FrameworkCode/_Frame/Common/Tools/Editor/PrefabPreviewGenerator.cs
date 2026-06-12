using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabPreviewGenerator
{
    [MenuItem("Assets/生成预设透明预览图", true)]
    private static bool ValidateGeneratePreview()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/生成预设透明预览图")]
    private static void GeneratePreview()
    {
        GameObject prefab = Selection.activeObject as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning("请选中一个预设物体！");
            return;
        }

        // 获取预览图（异步）
        Texture2D preview = AssetPreview.GetAssetPreview(prefab);
        if (preview == null)
        {
            Debug.Log("预览图尚未生成，等待...");
            EditorApplication.delayCall += () => GeneratePreview(); // 递归尝试
            return;
        }

        // 创建透明背景图
        Texture2D transparentTex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        Color32[] clearPixels = new Color32[128 * 128];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = new Color32(0, 0, 0, 0);
        transparentTex.SetPixels32(clearPixels);

        // 将预览图贴到透明背景上（居中缩放）
        int width = Mathf.Min(preview.width, 128);
        int height = Mathf.Min(preview.height, 128);
        int x = (128 - width) / 2;
        int y = (128 - height) / 2;

        // 拷贝缩略图到目标纹理
        Color[] previewPixels = preview.GetPixels(0, 0, width, height);
        transparentTex.SetPixels(x, y, width, height, previewPixels);
        transparentTex.Apply();

        // 转换为 PNG 并保存
        byte[] pngData = transparentTex.EncodeToPNG();
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        string directory = Path.GetDirectoryName(assetPath);
        string fileName = Path.GetFileNameWithoutExtension(assetPath) + "_Preview.png";
        string savePath = Path.Combine(directory, fileName);
        File.WriteAllBytes(savePath, pngData);

        // 刷新资源
        AssetDatabase.Refresh();

        Debug.Log("生成成功：" + savePath);
    }
}