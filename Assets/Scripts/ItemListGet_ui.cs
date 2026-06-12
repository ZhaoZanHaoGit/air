using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemListGet_ui : MonoBehaviour
{
    public Image showImage;
    public Button add, minus, Show;
    public int num = 0;
    public TextMeshProUGUI numText;
    public Item item;
    public TextMeshProUGUI itemNameText,itemModelText,ItemPriceText;

    private void Start()
    {
        Show.onClick.AddListener(ShowItemData);
        add.onClick.AddListener(() =>
        {
            num++;
            numText.text = num.ToString();

        });
        minus.onClick.AddListener(() =>
        {
            if (num > 0)
            {
                num--;
                numText.text = num.ToString();
            }
        });
        initItem();
    }
    void initItem()
    {
        itemNameText.text = item.itemName;
        itemModelText.text = item.model;
        ItemPriceText.text = item.price.ToString(); 


    }


    void ShowItemData()
    {
        showImage.sprite = item.describeTexture;

        showImage.transform.parent.gameObject.SetActive(true);


    }

}
