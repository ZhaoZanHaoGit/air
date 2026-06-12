using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvDu_P : MonoBehaviour, IPaifengAnimSet
{

    public Image[] images;

    public float intervalSeconds = 2f;


    public int blinkCount = 2;
    [Range(0, 100)] public float minVPercent = 60f;
    [Range(0, 100)] public float maxVPercent = 100f;
    public float halfDuration = 0.15f;

    private bool _running = false;
    public bool doanim = false;
    public Animation anim, lineAnim;
    public GameObject arrowRoot, lineroot;

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

                    img.BlinkValue(blinkCount, minVPercent, maxVPercent, halfDuration);
                }
                if (i == 2)
                {
                    if (lineAnim != null)
                        lineAnim.Play();
                    // lineAnim3.Play();
                }

                if (i == 4)
                {
                    for (int j = 0; j < arrowRoot.transform.childCount; j++)
                    {
                        arrowRoot.transform.GetChild(j).gameObject.SetActive(true);
                    }
                    if (anim != null)
                        anim.Play();
                }

            }
        }
        /*
        yield return new WaitForSeconds(5);
        if (anim != null)
            anim.Stop();
        // lineAnim.Stop();
        Image[] lines = lineroot.GetComponentsInChildren<Image>();
        Debug.Log(lines.Length);
        foreach (var line in lines)
        {
            line.fillAmount = 0;
        }
        for (int j = 0; j < arrowRoot.transform.childCount; j++)
        {
            arrowRoot.transform.GetChild(j).gameObject.SetActive(false);
        }
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        _running = false;*/
    }

    public void endplay()
    {
        if (anim != null)
            anim.Stop();
        // lineAnim.Stop();
        Image[] lines = lineroot.GetComponentsInChildren<Image>();
        Debug.Log(lines.Length);
        foreach (var line in lines)
        {
            line.fillAmount = 0;
        }
        for (int j = 0; j < arrowRoot.transform.childCount; j++)
        {
            arrowRoot.transform.GetChild(j).gameObject.SetActive(false);
        }
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        _running = false;
    }
}