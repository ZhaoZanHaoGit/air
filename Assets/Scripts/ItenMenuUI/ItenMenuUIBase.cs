using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItenMenuUIBase : MonoBehaviour
{
    protected Item3D item3D;
    public GameObject itemIns;
    public TMP_InputField tMP_InputField;

    virtual public void SetItem3D(Item3D item3D)
    {
        this.item3D = item3D;
    }
    virtual public Item3D GetItem3D()
    {
        return item3D;
    }
    virtual public void InitItenMenu()
    {
        tMP_InputField.text = itemIns.GetComponent<BaseValve>().valveName;
    }
    virtual public void OnConfirm()
    { itemIns.GetComponent<BaseValve>().valveName = tMP_InputField.text; }

}
