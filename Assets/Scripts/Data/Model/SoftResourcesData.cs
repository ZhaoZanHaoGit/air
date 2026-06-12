using System;

/// <summary>
/// 所有软件教师上传学习资源
/// </summary>
public class SoftResourcesData 
{
    /// <summary>
    /// 资源ID
    /// </summary>
    public int ID { get; set; }
    /// <summary>
    /// 软件ID
    /// </summary>
    public int SoftID { get; set; }
    public string SoftName { get; set; }
    public string SchoolName { get; set; }
    /// <summary>
    /// 上传人账号
    /// </summary>
    public string Account { get; set; }
    /// <summary>
    /// 资源类型
    /// </summary>
    public int FileType { get; set; }
    /// <summary>
    /// 资源分组
    /// </summary>
    public int ResourcesType { get; set; }
    /// <summary>
    /// 资源名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 资源路径 非图片
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// 缩列图路径 （图片）
    /// </summary>
    public string ThumbnailsPath { get; set; }
    /// <summary>
    /// 原始图像路径（图片）
    /// </summary>
    public string OriginalImagesPath { get; set; }
    public DateTime CreatTime { get; set; }
}
