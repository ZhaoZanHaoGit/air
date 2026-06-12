using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHPtest : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {


            StartCoroutine(TestSubmitAndRead(i + 100));
        }
    }

    IEnumerator TestSubmitAndRead(int id)
    {
        // 提交一条测试数据
        ScoreData d = new ScoreData
        {
            xh = id,
            name = "张三",
            idcard = "123456789012345678",
            danwei = "某单位",
            phone = "13800138000",
            date = System.DateTime.Now.ToString("yyyy-MM-dd"),
            total_score = 95
        };

        yield return StartCoroutine(GetComponent<PhpManager>().SubmitScore(d));

        // 读取所有数据
        yield return StartCoroutine(GetComponent<PhpManager>().GetAllScores((list) =>
        {
            foreach (var item in list)
            {
                Debug.Log($"{item.xh} {item.name} {item.total_score}分");
            }
        }));
    }
}
