using Hypertonic.GridPlacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hypertonic.GridPlacement.Example.BasicDemo;
public class CreateItemBtn : MonoBehaviour
{
    public GameObject _gridObjectToSpawnPrefab;
    private Button btn;
    private GameObject objectToPlace;
    public static event System.Action<GameObject> OnOptionSelected;
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(UseItem);
        GridControlManager_sample.OnObjectPlacedOnGrid += HandleObjectPlacedOnGrid;
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
    private void HandleObjectPlacedOnGrid(GameObject @object)
    { SimulationLoop.Instance.gridInScene.Add(@object); }
    public void UseItem()
    {
        /*
        // 1. 检查库存：如果数量已经为 0，理论上按钮由于交互禁用无法点击，但逻辑检查作为双重保险
        if (num <= 0 || !ItemManager.Instance.inventory.ContainsKey(this.item))
        {
            Debug.LogWarning("物品数量不足，无法使用！");
            return;
        }
        */
        // 2. 预处理：清理并归还当前可能正在放置的任何物体
        ClearAndReturnCurrentPlacement();
        // 5. 生成并进入放置模式
        objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), Quaternion.identity);
        objectToPlace.name = _gridObjectToSpawnPrefab.name;

        if (!objectToPlace.TryGetComponent(out Item3D gridObject))
        {
            objectToPlace.AddComponent<Item3D>();
        }
        //objectToPlace.GetComponent<Item3D>().itemUI = this;
       // objectToPlace.GetComponent<Item3D>().itemSelf = this.item;

        OnOptionSelected?.Invoke(objectToPlace);
        GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
    }
}
