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


    public void UpdateCount()
    {
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
    }
}
