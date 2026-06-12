//物品摆放场景放置物体按钮脚本


using Hypertonic.GridPlacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hypertonic.GridPlacement.Example.BasicDemo;

public class UI_TestPanelItemBTN : MonoBehaviour
{
    
    public Item itemins;
    [HideInInspector]
    public int itemnum;
    public int chazhi = 0;
    public Image itemImage;
    public TextMeshProUGUI numText, itemName;
    public static event System.Action<GameObject> OnOptionSelected;


    private Button btn;
    public GameObject _gridObjectToSpawnPrefab;
    private GameObject objectToPlace;

    // Start is called before the first frame update
    void Start()
    {
       
        btn = GetComponent<Button>();
        btn.onClick.AddListener(UseItem);
        btn.interactable=false;
        GridControlManager_sample.OnObjectPlacedOnGrid += HandleObjectPlacedOnGrid;
    }
    void OnDestroy()
    {
        GridControlManager_sample.OnObjectPlacedOnGrid -= HandleObjectPlacedOnGrid;
    }
    public void ClearData()
    {
        /*
        itemins = new Item("",0,null);
        num = 0;
        itemImage.sprite = null;
        numText.text = string.Empty;
        itemName.text = string.Empty;
        _gridObjectToSpawnPrefab = null;

        */
    }

    public void Init(Item itemperfab, int num)
    {
        itemins = itemperfab;
        itemnum = num - chazhi;
        itemImage.sprite = itemperfab.itemImage;
        numText.text = itemnum.ToString();
        itemName.text = itemperfab.itemName;
        _gridObjectToSpawnPrefab = itemperfab.prefab;
    }

    void Update()
    {
        UpdateUIState();
    }
    private void HandleObjectPlacedOnGrid(GameObject @object)
    {
        Debug.Log("@object.name");
        if (@object.GetComponent<Item3D>().itemUI == this)
        {
            if (!@object.GetComponent<Item3D>().hasCreate)
            {
                @object.GetComponent<Item3D>().hasCreate = true;
                // 3. 减少本地记录的数量并更新 UI
                itemnum--;
                chazhi++;
                UpdateUIState();
                // 4. 从全局背包系统移除
                // ItemManager.Instance.RemoveItem(this.item, 1);
                SimulationLoop.Instance.gridInScene.Add(@object);
            }

        }



    }

    public void UseItem()
    {
        // 1. 检查库存：如果数量已经为 0，理论上按钮由于交互禁用无法点击，但逻辑检查作为双重保险
        if (itemnum <= 0 || !ItemManager.Instance.inventory.ContainsKey(this.itemins))
        {
            Debug.LogWarning("物品数量不足，无法使用！");
            return;
        }

        // 2. 预处理：清理并归还当前可能正在放置的任何物体
        ClearAndReturnCurrentPlacement();
        // 5. 生成并进入放置模式
        objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), Quaternion.identity);
        objectToPlace.name = _gridObjectToSpawnPrefab.name;

        if (!objectToPlace.TryGetComponent(out Item3D gridObject))
        {
            objectToPlace.AddComponent<Item3D>();
        }
        objectToPlace.GetComponent<Item3D>().itemUI = this;
        objectToPlace.GetComponent<Item3D>().itemSelf = this.itemins;

        OnOptionSelected?.Invoke(objectToPlace);
        GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
    }



    public void ReturnItem()
    {
        // 调用封装函数处理清理工作
        ClearAndReturnCurrentPlacement();
    }

    /// <summary>
    /// 核心封装：取消当前放置模式，销毁场景物体，并处理库存归还及 UI 更新
    /// </summary>
    private void ClearAndReturnCurrentPlacement()
    {
        var gridManager = GridManagerAccessor.GridManager;

        if (gridManager.IsPlacingGridObject)
        {
            GameObject currentPlacingObject = gridManager.ObjectToPlace;

            // 1. 归还逻辑：本地数量增加，全局背包增加
            // num++;
            // ItemManager.Instance.AddItem(this.item, 1);

            // 2. 更新按钮交互性和文字
            // UpdateUIState();

            // 3. 取消放置并销毁
            gridManager.CancelPlacement(hideGrid: false);
            if (currentPlacingObject != null)
            {
                Destroy(currentPlacingObject);
            }
        }
    }

    /// <summary>
    /// 刷新当前按钮的文字显示和交互状态
    /// </summary>
    public void UpdateUIState()
    {

        // 更新数量文字
        if (numText != null)
        {
            numText.text = itemnum.ToString();
        }
        //Debug.Log(num.ToString());
        // 根据数量控制按钮是否可以点击
        if (btn != null)
        {
            btn.interactable = (itemnum > 0);
        }
    }
    private void HandleButtonClicked()
    {
        if (_gridObjectToSpawnPrefab == null)
        {
            Debug.LogError("Error. No prefab assigned to spawn on this selection option");
        }
        if (GridManagerAccessor.GridManager.IsPlacingGridObject)
        {

            //如果正在放置物体，点击按钮会取消放置模式取消正在放置的物体，放置当前脚本设置的物体
            // 1. 获取当前正在摆放但还未确认的物体
            GameObject currentPlacingObject = GridManagerAccessor.GridManager.ObjectToPlace;

            // 2. 取消当前的放置模式（这会重置 VisualManager 和内部状态）
            // 参数 hideGrid: false 表示切换物体时不需要隐藏再显示网格，保持视觉连贯
            GridManagerAccessor.GridManager.CancelPlacement(hideGrid: false);

            // 3. 销毁场景中那个未完成放置的物体
            if (currentPlacingObject != null)
            {
                Destroy(currentPlacingObject);
            }
        }

        GameObject objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());

        objectToPlace.name = _gridObjectToSpawnPrefab.name;

        if (!objectToPlace.TryGetComponent(out ExampleGridObject gridObject))
        {
            objectToPlace.AddComponent<ExampleGridObject>();
        }

        OnOptionSelected?.Invoke(objectToPlace);

        GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
    }



}
