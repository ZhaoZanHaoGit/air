using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentClassinfoItem : MonoBehaviour
{
    private Toggle thisT;
    private TMP_Text classnameText;

    public string classname { get; private set; }

    public EditorAgentPanel editorAgentPanel;

    private void Awake()
    {
        thisT = GetComponent<Toggle>();
        classnameText = transform.Find("classname").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        thisT.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (string.IsNullOrEmpty(classname)) return;

        if (isOn)
            editorAgentPanel.AddClass(classname);
        else
            editorAgentPanel.RemoveClass(classname);
    }

    public void Init(string _classname)
    {
        classname = _classname;
        classnameText.text = _classname;
    }

    // 
    public void SetSelect(bool value)
    {
        // 不触发事件
        thisT.SetIsOnWithoutNotify(value);
    }
}
