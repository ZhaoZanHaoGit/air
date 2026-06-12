using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TongfengShow : MonoBehaviour,IPaifengAnimSet
{


    [Header("���� 8 �� UI Image")]
    public Image[] images;

    [Header("������룩")]
    public float intervalSeconds = 2f;

    // ���� BlinkValue �ļ򵥲�����������Ĭ�ϼ��ɣ�
    public int blinkCount = 2;
    [Range(0, 100)] public float minVPercent = 60f;
    [Range(0, 100)] public float maxVPercent = 100f;
    public float halfDuration = 0.15f;

    private bool _running = false;
    public bool doanim = false;
    public Animation anim, lineAnim, lineAnim2, lineAnim3,lastAnim;
    public GameObject arrowRoot;
    public Image[] Lines,Arrows;
    void Update()
    {
        if (!_running && doanim)
        {
            StartCoroutine(RunOnce());
            doanim = false;
        }
    }
    private IEnumerator RunOnce()
    {
        _running = true;

        if (images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                yield return new WaitForSeconds(intervalSeconds);
                var img = images[i];
                if (img != null)
                {
                    img.gameObject.SetActive(true);
                    // ֻ���𴥷�һ����˸��ʵ����˸�� BlinkValue �ڲ��� DOTween ִ��
                    img.BlinkValue(blinkCount, minVPercent, maxVPercent, halfDuration);
                }
                if (i == 4)
                {
                    lineAnim.Play();
                    lineAnim3.Play();
                }
                if (i == 5)
                { lineAnim2.Play(); }
                if (i == 6)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        arrowRoot.transform.GetChild(j).gameObject.SetActive(true);
                    }
                    anim.Play();
                }
                if (i == 7)
                {
                    for (int j = 8; j < 21; j++)
                    {
                        arrowRoot.transform.GetChild(j).gameObject.SetActive(true);
                    }
                    lastAnim.gameObject.SetActive(true);
                    lastAnim.Play();
                }

                // ���һ�Ŵ�����Ͳ��ٵȴ���ֱ�ӽ���




            }
        }
        /*
        yield return new WaitForSeconds(5);
        anim.Stop();
        lineAnim.Stop();
        lineAnim2.Stop();
        lineAnim3.Stop();
        lastAnim.Stop();
        lastAnim.gameObject.SetActive(false);
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in Lines)
        {
            image.fillAmount = 0;
        }
        foreach(var image in Arrows)
            { image.enabled = false; }
        for (int j = 0; j < arrowRoot.transform.childCount; j++)
        {
            arrowRoot.transform.GetChild(j).gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
        _running = false; // һ�ֽ����������ٴΰ� A ������һ��
        */
    }

    public void endplay()
    {
        anim.Stop();
        lineAnim.Stop();
        lineAnim2.Stop();
        lineAnim3.Stop();
        lastAnim.Stop();
        lastAnim.gameObject.SetActive(false);
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in Lines)
        {
            image.fillAmount = 0;
        }
        foreach (var image in Arrows)
        { image.enabled = false; }
        for (int j = 0; j < arrowRoot.transform.childCount; j++)
        {
            arrowRoot.transform.GetChild(j).gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
        _running = false;
    }
    /*
private IEnumerator RunOnce()
{
   _running = true;

   if (images != null)
   {
       for (int i = 0; i < images.Length; i++)
       {
           var img = images[i];
           if (img != null)
           {
               // ֻ���𴥷�һ����˸��ʵ����˸�� BlinkValue �ڲ��� DOTween ִ��
               img.BlinkValue(blinkCount, minVPercent, maxVPercent, halfDuration);
           }
           if (i == 3)
           { animator.SetTrigger("PlayArrowMove"); }

           // ���һ�Ŵ�����Ͳ��ٵȴ���ֱ�ӽ���
           if (i < images.Length - 1)
           {
               animator.SetTrigger("Stop");
               gameObject.SetActive(false);
               yield return new WaitForSeconds(intervalSeconds);
           }

       }
   }

   _running = false; // һ�ֽ����������ٴΰ� A ������һ��
}*/

}
