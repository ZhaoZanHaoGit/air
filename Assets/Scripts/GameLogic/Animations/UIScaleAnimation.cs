
/******************************************************************************
 * 
 *  Title:			
 *
 *  Version:		
 *
 *  Description:
 *  1.UI大小的动画
 *
 *  Author:				
 *       
 *  Date:				
 * 
 ******************************************************************************/
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScaleAnimation : UIAnimationBase
{
    private RectTransform _myRect;
    private RectTransform myRect => _myRect ??= GetComponent<RectTransform>();
    private float _animationTime = 0.25f;
    public Vector3 MaxScale = Vector3.one * 1.1f;
    public Vector3 MinScale = Vector3.one;
    public GameObject obj;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        myRect.DOScale(MaxScale, _animationTime);
        if (obj != null) {
            obj.SetActive(true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        myRect.DOScale(MinScale, _animationTime);
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }
}
