using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTip :MonoBehaviour
{
    private TMP_Text showText;
    private RectTransform thisT;
    private HorizontalLayoutGroup group;
    private bool isfollow=false;
    public static TextTip _instance;
    private void Awake()
    {
        _instance = this;
        group =transform.Find("BG").GetComponent<HorizontalLayoutGroup>();
        thisT = GetComponent<RectTransform>();
        showText = transform.Find("BG/message").GetComponent<TMP_Text>();
        this.gameObject.SetActive(false);
    }
    public void Show(string message)
    {
        //if (UIHelper.Instance.IsPointerOverUI()) return;
        this.gameObject.SetActive(true);
        thisT.position = Input.mousePosition;
        showText.text = message;
        group.enabled = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)group.transform);
        group.enabled = true;
        isfollow = true;
    }
    private void Update()
    {
        if (isfollow) {
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            thisT.transform.position = new Vector3(mousePosition.x+30, mousePosition.y+10, 0);
        }
    }

    public void Hide()
    {
        isfollow=false;
        showText.text = "";
        if (this.gameObject.activeSelf) {
            this.gameObject.SetActive(false);
        }
    }
    bool ScreenMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenCenterX = screenWidth / 2f;
        if (mousePos.x < screenCenterX)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    void SetAnchorToTopLeft(RectTransform rectTransform)
    {
        // 设置锚点
        rectTransform.anchorMin = new Vector2(-0.5f, 1.2f);
        rectTransform.anchorMax = new Vector2(-0.5f, 1.2f);

        // 设置锚点位置（Pivot）
        rectTransform.pivot = new Vector2(-0.5f, 1.2f);
    }

    void SetAnchorToTopRight(RectTransform rectTransform)
    {
        // 设置锚点
        rectTransform.anchorMin = new Vector2(1.5f, 1.2f);
        rectTransform.anchorMax = new Vector2(1.5f, 1.2f);

        // 设置锚点位置（Pivot）
        rectTransform.pivot = new Vector2(1.5f, 1.2f);
    }
}
    