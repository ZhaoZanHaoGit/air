using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitalHumanItem : MonoBehaviour
{
    private TMP_Text numberText, nameText, urlText, keyText, classnameText, isopenText;
    private Button editBtn, deleteBtn;
    public DigitalHumanData Data { get; set; }
    private AIDigitalHuman_Panel aIDigitalHuman_Panel { get { return TeacherTjPanel.GetPanel((int)PanelUIType.AIDigitalHuman_Panel) as AIDigitalHuman_Panel; } }
    private void Awake()
    {
        numberText=transform.Find("number").GetComponent<TMP_Text>();
        nameText=transform.Find("name").GetComponent <TMP_Text>();
        urlText = transform.Find("url").GetComponent<TMP_Text>();
        keyText = transform.Find("key").GetComponent<TMP_Text>();
        classnameText = transform.Find("classname").GetComponent<TMP_Text>();
        isopenText = transform.Find("isOpen").GetComponent<TMP_Text>();
        editBtn = transform.Find("czBtns/editButton").GetComponent<Button>();
        deleteBtn = transform.Find("czBtns/deleteButton").GetComponent<Button>();
    }
    private void Start()
    {
        editBtn.onClick.AddListener(() =>
        {
            aIDigitalHuman_Panel.EditData(Data);
        });
        deleteBtn.onClick.AddListener(() => {
            aIDigitalHuman_Panel.DeleteData(Data.ID.ToString());
        });
    }
    public void Init(int number, DigitalHumanData data) { 
        Data = data;
        numberText.text = number.ToString().Trim(); ;
        nameText.text = data.DigitalHumanName.Trim(); ;
        urlText.text = data.DigitalHumanURL.Trim(); ;
        keyText.text = data.DigitalHumanKey.Trim(); ;
        classnameText.text = data.Classinfo.Trim(); ;
        isopenText .text = ((DigitalHumanOnState)data.OnState).ToString().Trim();
        isopenText.color = data.OnState == 1 ? Color.blue : Color.red;
    }
}
