using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddAgentClassInfoItem : MonoBehaviour
{
    private Toggle thisT;
    private TMP_Text classnameText;
    public string classname { get; set; }

    public AddAgentPanel panel;
    private void Awake()
    {
        thisT = GetComponent<Toggle>();
        classnameText = transform.Find("classname").GetComponent<TMP_Text>();
    }
    private void Start()
    {
        thisT.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                if (!string.IsNullOrEmpty(classname))
                {
                    panel.AddClass(classname);
                }
            }
            else
            {
                panel.RemoveClass(classname);
            }
        });
    }
    public void Init(string _classname)
    {
        if (classnameText != null)
        {
            classname = _classname;
            classnameText.text = _classname;
        }
    }
    public void SetSelect(bool b)
    {
        thisT.isOn = b;
        if (b)
        {
            if (!string.IsNullOrEmpty(classname))
            {
                panel.AddClass(classname);
            }
        }
        else
        {
            panel.RemoveClass(classname);
        }
    }
}
