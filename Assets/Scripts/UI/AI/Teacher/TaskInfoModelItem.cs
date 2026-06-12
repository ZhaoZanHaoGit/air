using TMPro;
using UnityEngine;

public class TaskInfoModelItem : MonoBehaviour
{
    private TMP_Text tasknameText;
    private void Awake()
    {
        tasknameText = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
    }
    public void SetData(string _name)
    {
        tasknameText.text = _name;
    }
}
