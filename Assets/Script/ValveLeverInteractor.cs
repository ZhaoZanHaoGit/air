using UnityEngine;
using DG.Tweening;

public class ValveLeverInteractor : MonoBehaviour
{
    public ManualValve34 targetValve;
    public Transform leverTransform; // 手柄旋转的轴向物体

    // 三个位置对应的三维旋转角度 (例如绕 Y 轴旋转)
    public Vector3 leftRotation = new Vector3(0, -30f, 0);
    public Vector3 centerRotation = new Vector3(0, 0f, 0);
    public Vector3 rightRotation = new Vector3(0, 30f, 0);

    private int currentPositionIndex = 0; // 0: 中, 1: 右, -1: 左

    void OnMouseDown() // 依赖 3D 碰撞体触发点击
    {
        if (targetValve == null || leverTransform == null) return;

        // 简易切换顺序逻辑：中 -> 右 -> 左 -> 中
        if (currentPositionIndex == 0) currentPositionIndex = 1;
        else if (currentPositionIndex == 1) currentPositionIndex = -1;
        else if (currentPositionIndex == -1) currentPositionIndex = 0;

        // 1. 驱动手柄旋转动画
        Vector3 targetRot = centerRotation;
        if (currentPositionIndex == 1) targetRot = rightRotation;
        else if (currentPositionIndex == -1) targetRot = leftRotation;

        leverTransform.DOLocalRotate(targetRot, 0.25f).SetEase(Ease.OutBack);

        // 2. 实时更新换向阀的物理气路状态
        targetValve.SetValveState(currentPositionIndex);
    }
}