
/******************************************************************************
 * 
 *  Title:				
 *
 *  Version:			
 *
 *  Description:
 *  1.UI¶¯»­»ùÀà
 *
 *  Author:				
 *       
 *  Date:			
 * 
 ******************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIAnimationBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public abstract void OnPointerEnter(PointerEventData eventData);

    public abstract void OnPointerExit(PointerEventData eventData);
}
