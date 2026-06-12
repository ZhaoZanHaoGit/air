using DG.Tweening;
using UnityEngine;

public abstract class BasePage : MonoBehaviour
{
    private RectTransform thisT;
    private float animationSpeed = 0.5f;
    public abstract PageUIType GetUIType();
    public virtual void Awake()
    {
        thisT = GetComponent<RectTransform>();
    }
    public virtual void Init()
    {
    }
    public virtual void InitData(SoftResourcesData data)
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
    public virtual void OnResetView() { }
}
