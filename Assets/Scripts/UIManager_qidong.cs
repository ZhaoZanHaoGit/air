//ui管理器，负责管理UI界面和UI事件

using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine.TextCore.LowLevel;

public class UIManager_qidong : MonoBehaviour
{
    public GameObject workSpace, factory;
    public GameObject TestItemPerfab;
    public RectTransform TestItemUIRoot;
    public RectTransform mainPanelRoot;
    Dictionary<Item, UI_TestPanelItemBTN> uI_TestPanelItems = new Dictionary<Item, UI_TestPanelItemBTN>();
    public List<GameObject> ItemBtns = new List<GameObject>();
    //[SerializeField]
    // private Button _addAnObjectButton;

    //[SerializeField]
    //private GameObject _objectSelectionUI;

    [SerializeField]
    private GameObject _gridObjectAlignmentControls;

    [SerializeField]
    public GameObject _gridControls;

    private void Start()
    {
        // _addAnObjectButton.onClick.AddListener(HandleAddAnObjectButtonPressed);
        // mainPanelRoot.gameObject.SetActive(false);
         UI_TestPanelItemBTN.OnOptionSelected += HandleGridObjectOptionSelected;
        CreateItemBtn.OnOptionSelected += HandleGridObjectOptionSelected;
        //Button_CloseObjectSelection.OnCloseButtonPressed += HandleCloseObjectSelectionPressed;
        Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent += HandleOpenChangeAlignmentPressed;
        Button_ChangeAlignment.OnChangeAlignmentPressed += HandleChangeAlignmentPressed;
        Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacementPressed;
        GridControlManager_sample.OnObjectPlacedOnGrid += HandleObjectPlacedOnGrid;
        for (int i = 0; i < TestItemUIRoot.childCount; i++)
        {
            TestItemUIRoot.GetChild(i).gameObject.SetActive(false);
            ItemBtns.Add(TestItemUIRoot.GetChild(i).gameObject);


        }

        //Item3D.OnObjectSelected += HandleExampleGridObjectSelected;
    }





    private void OnDestroy()
    {
        // _addAnObjectButton.onClick.RemoveListener(HandleAddAnObjectButtonPressed);

        Button_GridObjectSelectionOption.OnOptionSelected -= HandleGridObjectOptionSelected;
        UI_TestPanelItemBTN.OnOptionSelected -= HandleGridObjectOptionSelected;
        CreateItemBtn.OnOptionSelected -= HandleGridObjectOptionSelected;
        //Button_CloseObjectSelection.OnCloseButtonPressed -= HandleCloseObjectSelectionPressed;
        Button_OpenChangeAlignmentOptions.OnOpenChangeAlignmentOptionEvent -= HandleOpenChangeAlignmentPressed;
        Button_ChangeAlignment.OnChangeAlignmentPressed -= HandleChangeAlignmentPressed;
        Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacementPressed;
        GridControlManager_sample.OnObjectPlacedOnGrid -= HandleObjectPlacedOnGrid;
        //Item3D.OnObjectSelected -= HandleExampleGridObjectSelected;
    }

    private void HandleGridObjectOptionSelected(GameObject gridObject)
    {
        Debug.Log("aaaaaaaaaaaaaaaa");
        // _objectSelectionUI.SetActive(false);
        _gridControls.SetActive(true);
    }

    private void HandleAddAnObjectButtonPressed()
    {
        //_objectSelectionUI.SetActive(true);
    }

    private void HandleCloseObjectSelectionPressed()
    {
        // _objectSelectionUI.SetActive(false);
    }

    private void HandleOpenChangeAlignmentPressed()
    {
        _gridObjectAlignmentControls.SetActive(true);
    }

    public void HandleCancelPlacementPressed()
    {
        _gridControls.SetActive(false);
    }

    private void HandleChangeAlignmentPressed(ObjectAlignment objectAlignment)
    {
        _gridObjectAlignmentControls.SetActive(false);
    }

    private void HandleObjectPlacedOnGrid(GameObject @object)
    {
        GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

        _gridControls.SetActive(false);
    }
    /// <summary>
    /// 打开移动物体界面
    /// </summary>
    /// <param name="gridObject"></param>
    /// <param name="vector2"></param>
    public void HandleExampleGridObjectSelected(GameObject gridObject, Vector2 vector2)
    {
        GridManagerAccessor.GridManager.OnGridObjectDeleted += HandleGridObjectDeleted;

        _gridControls.SetActive(true);
    }

    private void HandleGridObjectDeleted()
    {
        GridManagerAccessor.GridManager.OnGridObjectDeleted -= HandleGridObjectDeleted;

        _gridControls.SetActive(false);
    }

    public void TogglePanel(bool isfactory)
    {
        if (isfactory)
        {
            factory.SetActive(true);
            workSpace.SetActive(false);
            ItemManager.Instance.UpdateBagUI();
        }
        else
        {
            factory.SetActive(false);
            workSpace.SetActive(true);
            UpdateTestPanelItem();
        }
        ItemManager.Instance.ClearAllItemData();

    }
    public void clearButtonData()
    {
        foreach (var item in ItemBtns)
        {
            item.GetComponent<UI_TestPanelItemBTN>().ClearData();
            item.SetActive(false);
        }
        uI_TestPanelItems.Clear();
    }
    public void UpdateTestPanelItem()
    {

        foreach (var item in ItemBtns)
        {
            item.SetActive(false);
        }
        int tempi = 0;
        // 这里可以根据需要更新测试面板上的物品信息
        // 例如，获取 ItemManager 中的物品数据并显示在 UI 上
        foreach (var item in ItemManager.Instance.inventory)
        {


            GameObject uiItem = ItemBtns[tempi];
            uiItem.GetComponent<UI_TestPanelItemBTN>().Init(item.Key, item.Value);
            uI_TestPanelItems[item.Key]=uiItem.GetComponent<UI_TestPanelItemBTN>();

            ItemBtns[tempi].SetActive(true);
            tempi++;
        }

    }


}
