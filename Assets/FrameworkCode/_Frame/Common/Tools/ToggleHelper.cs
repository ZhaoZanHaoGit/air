
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.toggle控制
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ToggleHelper : MonoBehaviour
{
    public GameObject[] ShowWhenIsOn;
    public GameObject[] HideWhenIsOn;
    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(onToggleValueChanged);
    }

    private void onToggleValueChanged(bool isOn)
    {
        foreach (var obj in ShowWhenIsOn) obj.SetActive(_toggle.isOn);
        foreach (var obj in HideWhenIsOn) obj.SetActive(!_toggle.isOn);
    }
}
