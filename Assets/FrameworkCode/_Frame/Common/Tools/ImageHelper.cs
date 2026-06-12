using System.IO;
using UnityEngine;

public class ImageHelper : MonoBehaviour
{
    // 从本地路径加载图片为 Texture2D
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath); // 读取文件字节
        Texture2D texture = new Texture2D(2, 2); // 创建一个空的 Texture2D 对象
        texture.LoadImage(fileData); // 使用 LoadImage 将字节数据加载到 Texture2D 中
        return texture;
    }

    // 生成缩略图
    public static Texture2D GenerateThumbnail(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        Texture2D thumbnail = new Texture2D(targetWidth, targetHeight, originalTexture.format, false);
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float u = (float)x / targetWidth;
                float v = (float)y / targetHeight;
                Color color = originalTexture.GetPixelBilinear(u, v);
                thumbnail.SetPixel(x, y, color);
            }
        }
        thumbnail.Apply();
        return thumbnail;
    }

    // 保存 Texture2D 为 PNG 文件
    public static string SaveTextureToTempFile(Texture2D texture, string fileName)
    {
        string path = Path.Combine(Application.temporaryCachePath, fileName);  // 使用临时文件夹路径
        string correctedFilePath = path.Replace("/", "\\");
        byte[] bytes;
        Debug.Log(fileName);
        // 判断文件扩展名
        if (fileName.EndsWith(".png"))
        {
            bytes = texture.EncodeToPNG();  // PNG 编码
            File.WriteAllBytes(correctedFilePath, bytes);  // 将图片保存到临时文件夹
            return correctedFilePath;
        }
        else if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg"))
        {
            bytes = texture.EncodeToJPG();  // JPG 编码
            File.WriteAllBytes(correctedFilePath, bytes);  // 将图片保存到临时文件夹
            return correctedFilePath;
        }
        else
        {
            Debug.Log("不支持的文件格式");
            return null;
        }   
    }

    // 删除临时文件
    public static void DeleteTempFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log("删除临时文件: " + filePath);
        }
    }
}
