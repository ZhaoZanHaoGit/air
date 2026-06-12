using DG.Tweening;
using UnityEngine;

public class FaceCameraUI : MonoBehaviour
{
    private Transform mainCamera;
    private RectTransform originImage, lineImage, textImage;
    private void Awake()
    {
        originImage = transform.Find("Canvas/Origin").GetComponent<RectTransform>();
        lineImage = transform.Find("Canvas/Origin/Line").GetComponent<RectTransform>();
        textImage = transform.Find("Canvas/Origin/Line/Image").GetComponent<RectTransform>();
    }
    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward,
                        mainCamera.rotation * Vector3.up);
    }
    public void Playback(bool b)
    {
        if (b)
        {
            originImage.DOScale(Vector3.one, 0.25f).OnComplete(() =>
            {
                lineImage.DOScaleX(1, 0.25f).OnComplete(() =>
                {
                    textImage.DOScaleX(1, 0.25f);
                });
            });
        }
        else
        {
            textImage.DOScaleX(0, 0.25f).OnComplete(() =>
            {
                lineImage.DOScaleX(0, 0.25f).OnComplete(() =>
                {
                    originImage.DOScale(Vector3.zero, 0.25f);
                });
            });
        }
    }

}
