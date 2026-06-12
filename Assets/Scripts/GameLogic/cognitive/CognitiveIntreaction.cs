//认知模式交互脚本
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.InputSystem;
using HighlightPlus;

public class CognitiveIntreaction : MonoBehaviour
{

    private bool isVR;
    /// <summary>
    /// 判断是否在VR模式下
    /// </summary>  
    /*
    public DragState CurrentDragState { get => currentDragState; set => currentDragState = value; }

    [SerializeField]
    private DragState currentDragState;
    */
    // Start is called before the first frame update
    /// <summary>
    ///整体/局部交互状态
    /// </summary>
    [HideInInspector]
    public bool InteractionState;
    /// <summary>
    /// 组合/分解状态
    /// </summary>
    [HideInInspector]
    public bool combinedState = true;
    /// <summary>
    /// 旋转中心
    /// </summary>
    public GameObject objMain;
    /// <summary>
    /// 动画持续时间
    /// </summary>
    public float animTime = 1f;

    public Animation Animation;

    bool Animing;
    /// <summary>
    /// 爆炸分解子列表
    /// </summary>
    public List<GameObject> childOBJs = new List<GameObject>();


    public Dictionary<GameObject, TransformData> StartTransformData = new Dictionary<GameObject, TransformData>();

    private void Start()
    {
        Animing = false;

        foreach (var item in childOBJs)
        {
            StartTransformData.Add(item, new TransformData(item.transform.position, item.transform.rotation, item.transform.localScale));
            item.AddComponent<HighlightEffect>();
            //item.AddComponent<ShiXunHighLightControl>();
        }

        SetInteractionState(true);
    }

    /// <summary>
    /// 组合
    /// </summary>
    public void Combin()
    {
        // if (combinedState == false && Animing == false)
        {
            Animing = true;
            foreach (var entry in StartTransformData)
            {
                GameObject targetObject = entry.Key;
                TransformData transformData = entry.Value;

                // 动画：将位置、旋转、缩放变换到目标值，持续时间为1秒
                targetObject.transform.DOMove(transformData.position, animTime);
                targetObject.transform.DORotateQuaternion(transformData.rotation, animTime);
                targetObject.transform.DOScale(transformData.scale, animTime).OnComplete(() =>
                {
                    Animing = false;

                });
            }
        }
        combinedState = true;


    }
    /// <summary>
    /// 分解
    /// </summary>
    public void Disassemble()
    {

        if (combinedState == true && Animing == false)
        {
            Animation.Play();
            combinedState = false;
        }
    }
    /// <summary>
    /// 设置交互模式为整体/局部
    /// </summary>
    /// <param name="state"></param>
    public void SetInteractionState(bool state)
    {

        gameObject.GetComponent<Collider>().enabled = state;
        foreach (var item in childOBJs)
        { item.GetComponent<Collider>().enabled = !state; }


    }


   
}

public enum DragState
{
    // 整体拖拽状态
    FullDrag,

    // 局部拖拽状态
    PartialDrag,

    // 禁止拖拽状态
    NoDrag
}
[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}