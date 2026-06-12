using DG.Tweening;
using System;
using System.IO;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    private RectTransform thisT;
    private float animationSpeed = 0.5f;

    //public PanelUIType ToBeOpenUI { get; set; } = PanelUIType.None;
    public abstract PanelUIType GetUIType();
    public virtual void Awake()
    {
        thisT = GetComponent<RectTransform>();
    }
    public virtual void Init()
    {
    }
    public virtual void OnShow()
    {
        thisT.DOLocalMoveX(0, animationSpeed);
    }
    public virtual void OnHide()
    {
        thisT.DOLocalMoveX(1700, animationSpeed);
    }
    public virtual void OnRefresh() { }
    public void DeleteChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        parent.DetachChildren();
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
    protected bool FileSize(string path,int _MB)
    {

        FileInfo fileInfo = new FileInfo(path);

        // 获取文件大小，以字节为单位
        long fileSizeInBytes = fileInfo.Length;

        // 转换为兆字节
        double fileSizeInMegabytes = fileSizeInBytes / (1024.0 * 1024.0);

        // 判断文件大小是否大于10兆
        if (fileSizeInMegabytes > _MB)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
