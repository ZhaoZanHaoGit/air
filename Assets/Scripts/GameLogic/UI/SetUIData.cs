using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetUIData : MonoBehaviour
{
 public  TextMeshProUGUI part1, part2, part3, all, part21, part22;

    public void setPanelData(string part1Text, string part2Text, string part3Text, string allText,string part21Text,string part22Text)
    { 
        part1.text = part1Text;
        part2.text = part2Text;
        part3.text = part3Text;
        all.text = allText;
        part21.text = part21Text;
        part22.text = part22Text;
    }
}
