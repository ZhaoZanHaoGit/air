using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    public float value0= 0.5f, value1= 0.1f, value2= 20f;
    public int value3=2;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShakeP();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ShakeR();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            domoveease();
        }
    }

    public void ShakeP()
    {
        Vector3 littleMoveValue = new Vector3 (0, 1, 0);
        transform.DOShakePosition(
            value0,    // 动画持续时间（1秒）
            littleMoveValue* value1,    // 颤动强度（Vector3.one * 0.5f）
            value3,      // 抖动次数
            value2       // 随机性（默认值90，一般不用改）
        );
    }

    public void ShakeR()
    {
        transform.DOShakeRotation(
            1.0f,    // 持续时间
            30f,     // 旋转强度（表示最多偏转30度）
            10,      // 抖动次数
            90f      // 随机性
        );
    }

    void domoveease()
    {
        transform.DOLocalMoveY(1, 1).SetEase(Ease.OutBack,0.02f).OnComplete(() => {Debug.Log("完成"); });
    }
}
