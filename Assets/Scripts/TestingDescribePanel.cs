//物品摆放界面描述脚本

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestingDescribePanel : MonoBehaviour
{
   public Image image;
   public TextMeshProUGUI text;
   

  public  void UpdateDescribeData(Item item)
    {
        image.sprite = item.describeTexture;
        /*
        if (item.itemDescription != null)
        {
            text.transform.parent.gameObject.SetActive(true);

            text.text = item.itemDescription;
        }
        else { text.transform.parent.gameObject.SetActive(false); }*/





    }
}
