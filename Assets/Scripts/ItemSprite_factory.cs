using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSprite_factory : MonoBehaviour
{
    public Item item;
    public int count;
    Button button;
    public ItemManager manager;
    public new TextMeshProUGUI name;
    public TextMeshProUGUI model, num;
    void Start()
    {
        button = GetComponent<Button>();
       /* button.onClick.AddListener(() =>
        {
            if (item != null)
            {
                manager.RemoveItem(item, 1);
                if (count > 0)
                {
                    count--;

                    if (count == 0)
                    {
                        item = null;
                        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";

                        transform.GetChild(0).GetComponent<Image>().sprite = null;
                        transform.GetChild(0).gameObject.SetActive(false);

                    }
                    else
                    {
                        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
                    }

                }

            }



        });*/
    }

    public void setItem(string itemName, string itemModel, int itemNum)
    {
        name.text = itemName;
        model.text = itemModel;
        num.text = itemNum.ToString();
    }

    public void UpdateCount()
    {
        num.text = count.ToString();
    }
}
