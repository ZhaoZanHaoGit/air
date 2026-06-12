using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuickMoveCamera : MonoBehaviour
{
    public CameraMove cameraMove;
    public List<MoveTransAndBut> moveTransAndBut=new List<MoveTransAndBut>();
    public UIScript1 uIScript1;
    //按照列表执行，主要出入口_0，次要出入口_1，发电机房_2，人防停车区_3
    void Start()
    {
        for (int i = 0; i < moveTransAndBut.Count; i++)
        {
            int tempI = i;
            moveTransAndBut[tempI].button.transform.DOScale(Vector3.one * 1.08f, 2.2f / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)           // 无限来回
            .SetUpdate(true);
            moveTransAndBut[tempI].button.onClick.AddListener(() => {
                cameraMove.SetCameraPositionAndRotation(moveTransAndBut[tempI].moveTran.position, moveTransAndBut[tempI].moveTran.rotation);
                //gameObject.SetActive(false);
                uIScript1.OnMapIconClicked();
            });
        }

        //foreach (var item in moveTransAndBut)
        //{
        //    item.button.onClick.AddListener(() => {
        //        cameraMove.SetCameraPositionAndRotation(item.moveTran.position, item.moveTran.rotation);
        //    });
        //}
    }

    
}
[Serializable]
public class MoveTransAndBut {
    public Button button;
    public Transform moveTran;
}