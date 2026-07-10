using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PneumaticEvaluator;

public class ItemManager : SingletonMono2<ItemManager>
{
    // Key: Item对象, Value: 拥有该物品的数量
    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    public List<ItemListGet_ui> itemListGet_scripts = new List<ItemListGet_ui>();
    public Transform iconRig;
    public UIManager_qidong uIManager_qidong;
    public PneumaticEvaluator Pneumatic;
    public Button cinfirm_receiptBtn;
    public GameObject toolPanel;
    bool part1Scored = false; // 标记零部件领取是否已经评分过，避免重复扣分
    private void Start()
    {
        cinfirm_receiptBtn.onClick.AddListener(OncinfrrmClick);
    }

    void OncinfrrmClick()
    {
        switch (SimulationLoop.Instance.currenttrainType)
        {
            case trainType.None:
                break;
            case trainType.free:
                Cinfirm_Rreceipt();
                uIManager_qidong.UpdateTestPanelItem();
                break;
            case trainType.test:
                Cinfirm_Rreceipt();
                uIManager_qidong.UpdateTestPanelItem();
                break;
            case trainType.exam:
                Dictionary<string, int> actualPicked = new Dictionary<string, int>();
                List<Dictionary<string, int>> expectedList = SimulationLoop.Instance.CurrentCase.ComponentList;
                foreach (var itemUI in itemListGet_scripts)
                {
                    if (itemUI.num > 0)
                    {
                        actualPicked.Add(itemUI.item.itemName, itemUI.num);


                    }
                }
                EvaluationResult result = Pneumatic.EvaluateComponents(expectedList, actualPicked);
                if (result.IsPerfect)
                {
                    Cinfirm_Rreceipt();
                    uIManager_qidong.UpdateTestPanelItem();
                }
                else
                {
                    Debug.Log(result.DetailReport);
                    UIManager.Instance.OpenMessageBoxUI(result.DetailReport, 0, EnumMessageBoxType.OK);
                }
                if (!part1Scored)
                {
                    Pneumatic.part1Score = result.FinalScore; // 记录零部件领取的得分
                    Pneumatic.part1Evaluated = true; // 标记已经过评分流程
                    part1Scored = true; // 标记已经评分过
                }
                break;
            default:
                break;
        }


    }

    private void OnEnable()
    {
        Cinfirm_Rreceipt();
    }

    /// <summary>
    ///添加到背包
    /// </summary>
    void Cinfirm_Rreceipt()
    {
        int count = 0;
        clearBagUI();
        foreach (var itemUI in itemListGet_scripts)
        {
            if (itemUI.num > 0)
            {
                AddItem(itemUI.item, itemUI.num);

                itemUI.num = 0; // 重置数量
                itemUI.numText.text = "0"; // 更新 UI 显示
                count++;
            }

        }
        int itemcinfirm = 0;
        foreach (var item in inventory)
        {
            updateBagIcon(itemcinfirm, item.Key.itemImage, item.Value, item.Key);
            itemcinfirm++;
        }
        toolPanel.SetActive(false);
    }

    void updateBagIcon(int count, Sprite sprite, int num, Item item)
    {
        iconRig.GetChild(count).GetComponent<ItemSprite_factory>().item = item;
        iconRig.GetChild(count).GetComponent<ItemSprite_factory>().count = num;
        iconRig.GetChild(count).GetComponent<ItemSprite_factory>().setItem(item.itemName, item.model, num);
        iconRig.GetChild(count).GetChild(0).gameObject.SetActive(true);
        //iconRig.GetChild(count).GetChild(0).GetComponent<Image>().sprite = sprite;
        //iconRig.GetChild(count).GetChild(1).GetComponent<TextMeshProUGUI>().text = num.ToString();


    }
    public void ClearAllItemData()
    {
        clearBagUI();
        inventory.Clear();
    }
    void clearBagUI()
    {
        if (iconRig.childCount > 0)
        {
            for (int i = 0; i < iconRig.childCount; i++)
            {
                iconRig.GetChild(i).GetChild(0).gameObject.SetActive(false);
                // iconRig.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
                iconRig.GetChild(i).GetComponent<ItemSprite_factory>().setItem("", "", 0);
            }
        }

    }

    public void UpdateBagUI()
    {
        for (int i = 0; i < iconRig.childCount; i++)
        {
            Item key = iconRig.GetChild(i).GetComponent<ItemSprite_factory>().item;
            if (inventory.ContainsKey(key))
            {
                iconRig.GetChild(i).GetComponent<ItemSprite_factory>().count = inventory[key];
                iconRig.GetChild(i).GetComponent<ItemSprite_factory>().UpdateCount();
            }

        }
    }


    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="item">物品对象</param>
    /// <param name="count">增加的数量</param>
    public void AddItem(Item item, int count = 1)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            inventory.Add(item, count);
        }
        Debug.Log($"增加了 {count} 个 {item.itemName}，当前总数: {inventory[item]}");
    }

    /// <summary>
    /// 移除指定数量的物品
    /// </summary>
    /// <returns>如果移除成功返回 true，数量不足返回 false</returns>
    public bool RemoveItem(Item item, int count = 1)
    {
        if (inventory.ContainsKey(item))
        {
            if (inventory[item] >= count)
            {
                inventory[item] -= count;
                if (inventory[item] <= 0)
                {
                    inventory.Remove(item);
                }
                return true;
            }
        }
        Debug.LogWarning("物品不足，无法移除");
        return false;
    }

    /// <summary>
    /// 彻底从背包丢弃某种物品
    /// </summary>
    public void ClearItem(Item item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory.Remove(item);
        }
    }

    public void debugString(string xxx)
    {

        Debug.Log(xxx);
    }

}
[System.Serializable] // 必须添加，否则无法在 Inspector 看到
public class Item
{
    public string itemName;
    public int itemID; // 建议用 ID 作为唯一标识
    public Sprite itemImage;
    public GameObject prefab;
    public Sprite describeTexture;
    public float price;
    public string model;

    [TextArea] public string itemDescription; // 将 Texture2D 改为 string 更常用

    public Item(string name, int id, Sprite image)
    {
        itemName = name;
        itemID = id;
        itemImage = image;
    }
}
// 对应 JSON 中的每一个元件对象
[Serializable]
public class ComponentItem
{
    public string name;
    public string type;
    public string iconName;
    public string category; // ✨ 新增：对应 JSON 中的分类标签
    public string describeText;
    public string describeImageName;
    public string describeVideoName;
    public string prefabName; // 注意：保持与你 JSON 里的拼写 "perfabName" 一致
    public string Specification;
}

// 对应 JSON 最外层的 {"components": [...]} 结构
[Serializable]
public class PneumaticComponentsData
{
    public List<ComponentItem> components;
}

