using HighlightPlus;
using Net.Common;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 该脚本用来核心控制展示模型
/// </summary>
public class ModelShowManager : SingletonMono<ModelShowManager>
{
    private Transform _rootTransform;
    private Vector3 modelRootPostion;
    private Quaternion modelRootRotation;
    private Transform showModel;
    public UnityEvent onShowModelChange;
    private RenderTexture cameraRenderT;
    void Start() {
        _rootTransform = transform.Find("ModelCenter");
        modelRootPostion = _rootTransform.position;
        modelRootRotation = _rootTransform.rotation;
        cameraRenderT = GetComponentInChildren<Camera>().targetTexture;
    }
    public void ModelShow(Transform model, Vector3 scale, CameraShowType sType, Vector3 angleV = default) {
        //Destory old gameobject
        if (showModel) {
            Destroy(showModel.gameObject);
        }

        //初始化对象控制  init model root postion and rotation
        _rootTransform.rotation = modelRootRotation;
        _rootTransform.position = modelRootPostion;
        _rootTransform.localScale = Vector3.one;
        //instantite show model
        showModel = Instantiate(model, _rootTransform);
        showModel.localScale = scale;
        showModel.localPosition = Vector3.zero;
        if (angleV != default) {
            showModel.eulerAngles = angleV;
        }
        //showModel.gameObject.layer = LayerMask.NameToLayer("ShowEntrench");
        SetLayerForObjectAndChildren(gameObject, "ShowEntrench");
        //Debug.Log("Instantiate model!!!!!");
        //关闭特效显示
        HighlightEffect tHE = showModel.GetComponent<HighlightEffect>();
        if (tHE != null) { tHE.enabled = false; }
        onShowModelChange.Invoke();
        switch (sType) {
            case CameraShowType.None:
                break;
            case CameraShowType.Object3D:
                GetComponentInChildren<Camera>().targetTexture = cameraRenderT;
                break;
            case CameraShowType.RawImage:
                GetComponentInChildren<Camera>().targetTexture = cameraRenderT;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 修改传入的物体及子物体的层级为layername
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    private void SetLayerForObjectAndChildren(GameObject obj, string layerName) {
        // 设置当前物体的层级
        obj.layer = LayerMask.NameToLayer(layerName);

        // 遍历所有子物体并递归设置层级
        foreach (Transform child in obj.transform) {
            SetLayerForObjectAndChildren(child.gameObject, layerName);
        }
    }


    /*
    public void ItemBoom() {
        showModel.gameObject.GetComponent<SimpleCognitiveControl>().BoomPlay(true);
    }

    public void ItemRecover() {
        showModel.gameObject.GetComponent<SimpleCognitiveControl>().BoomPlay(false);
    }*/
    /// <summary>
    /// 异常，关闭，应用按钮触发销毁展示物体
    /// </summary>
    public void DestoryBaseGo() {
        if (showModel) Destroy(showModel.gameObject, 1);
    }
}
public enum CameraShowType
{
    None,
    /// <summary>
    /// 3D物体展示
    /// </summary>
    Object3D,
    /// <summary>
    /// UI图像展示
    /// </summary>
    RawImage
}