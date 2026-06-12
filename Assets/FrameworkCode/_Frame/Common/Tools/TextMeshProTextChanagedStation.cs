using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMeshProTextChanagedStation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text text;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TextTip._instance.Show(text.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TextTip._instance.Hide();
    } 
}
