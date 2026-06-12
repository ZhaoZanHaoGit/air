using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.GridInput;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectMenuController : MonoBehaviour
{
    public GameObject menuPanel; // 你的操作菜单 Prefab 或 场景中的实例
    public RectTransform menuRectTransform; // 菜单的 RectTransform
    public GameObject currentOBJ;
    public GridControlManager_sample gridControlManager_Sample;
    public Button close, openGridControl, openItemSet, openDescribePanel;
    public TestingDescribePanel describePanel;
    public List<GameObject> itemsMenu = new List<GameObject>();

    // 引用你的蓝色配置文件
    public UniversalInputDefinition inputDef;
    UIManager_qidong uimanager;


    private void Start()
    {
        uimanager = GetComponent<UIManager_qidong>();

        close.onClick.AddListener(HideMenu);
        openGridControl.onClick.AddListener(() =>
        {
            HideMenu();
            uimanager.HandleExampleGridObjectSelected(currentOBJ, Vector2.one);
            GridManagerAccessor.GridManager.EnterPlacementMode(currentOBJ);
            gridControlManager_Sample.HandleExampleGridObjectSelected(currentOBJ);
        });

        openDescribePanel.onClick.AddListener(() =>
        {
            HideMenu();
            describePanel.gameObject.SetActive(true);
            describePanel.UpdateDescribeData(currentOBJ.GetComponent<Item3D>().itemSelf);
        });

        openItemSet.onClick.AddListener(OpenItemMenu);
    }

    private void OnEnable()
    {
        Item3D.OnObjectSelected += ShowMenu;
    }

    private void OnDisable()
    {
        Item3D.OnObjectSelected -= ShowMenu;
    }

    private void Update()
    {
        // 如果菜单没打开，就不执行后面的检测
        if (!menuPanel.activeSelf) return;

        // 1. 检测点击关闭（鼠标点击或手机单指点击）
        if (CheckGlobalClick())
        {
            // 判断点击的位置是否在菜单 UI 之外
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // 如果点的是空白处或者其他 3D 物体，关闭菜单
                HideMenu();
            }
        }

        // 2. 检测缩放关闭（滚轮或双指）
        if (Mathf.Abs(inputDef.GetZoomDelta()) > 0.001f)
        {
            HideMenu();
        }
    }

    private bool CheckGlobalClick()
    {
#if ENABLE_INPUT_SYSTEM
        // PC 端左键按下瞬间
        if (UnityEngine.InputSystem.Mouse.current != null &&
            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame) return true;

        // 手机端单指按下瞬间
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 1 &&
            UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began) return true;
#else
        // 旧版输入系统
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)) return true;
#endif
        return false;
    }
    void OpenItemMenu()
    {
        HideMenu();
        foreach (var item in itemsMenu)
        {
            item.SetActive(false);
        }
        Debug.Log($"当前物体的 Item 组件 ID: {currentOBJ.GetComponent<Item3D>().itemSelf.itemID}");
        if (itemsMenu.Count <= currentOBJ.GetComponent<Item3D>().itemSelf.itemID)
        {
            Debug.LogError("itemsMenu 列表中没有对应的菜单，请检查配置！");
            return;
        }
        itemsMenu[currentOBJ.GetComponent<Item3D>().itemSelf.itemID].SetActive(true);
        itemsMenu[currentOBJ.GetComponent<Item3D>().itemSelf.itemID].GetComponent<ItenMenuUIBase>().itemIns = currentOBJ;
        itemsMenu[currentOBJ.GetComponent<Item3D>().itemSelf.itemID].GetComponent<ItenMenuUIBase>().InitItenMenu();
    }
    void ClearPanel()
    {

        describePanel.gameObject.SetActive(false);
        foreach (var item in itemsMenu)
        {
            item.SetActive(false);
        }
        uimanager._gridControls.SetActive(false);


    }
    private void ShowMenu(GameObject targetObject, Vector2 screenPos)
    {
        ClearPanel();
        // 1. 激活菜单
        menuPanel.SetActive(true);

        // 2. 将屏幕坐标转换为 UI 局部坐标
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            menuRectTransform.parent as RectTransform,
            screenPos,
            null, // 如果是 Overlay 模式传 null，如果是 Camera 模式传 UI 相机
            out localPos
        );

        // 3. 设置位置
        menuRectTransform.localPosition = localPos;

        // 4. (可选) 将当前点击的物体信息传递给菜单中的按钮
        // 这样点击菜单的“归还”按钮时，知道是针对哪个 Item 进行操作
        currentOBJ = targetObject;
        var itemComponent = targetObject.GetComponent<Item3D>();
        if (itemComponent != null)
        {
            // 这里可以设置你的 Return 逻辑关联
            // 例如：menuReturnButton.itemBtnReference = itemComponent.itemUI;
        }
    }

    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }
}
