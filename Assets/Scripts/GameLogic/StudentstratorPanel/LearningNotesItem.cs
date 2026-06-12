using TMPro;
using UnityEngine;

public class LearningNotesItem : MonoBehaviour
{
    private TMP_Text _infoText;
    private void Awake()
    {
        _infoText=transform.Find("Text (TMP)").GetComponent<TMP_Text>();
    }
    public void Init(int index, SoftwareLearningData data, Gamemode gamemode)
    {
        if (AppController.Instance.userType == UserType.学生)
        {
            switch (gamemode)
            {
                case Gamemode.RenZhi:
                    _infoText.text = $"{index}.{AppController.Instance.loginUser.Username}同学在{data.Time}时，学习了{data.CourseName}科目";
                    break;
                case Gamemode.ShiXun:
                    _infoText.text = $"{index}.{AppController.Instance.loginUser.Username}同学在{data.Time}时，进行了{data.CourseName}科目的实训操作";
                    break;
                case Gamemode.KaoHe:
                    _infoText.text = $"{index}.{AppController.Instance.loginUser.Username}同学在{data.Time}时，进行了{data.CourseName}科目的实训考核操作，成绩为{data.Score}分";
                    break;
            }
        }
        else {
            switch (gamemode)
            {
                case Gamemode.RenZhi:
                    _infoText.text = $"{index}.{data.UserName}同学在{data.Time}时，学习了{data.CourseName}科目";
                    break;
                case Gamemode.ShiXun:
                    _infoText.text = $"{index}.{data.UserName}同学在{data.Time}时，进行了{data.CourseName}科目的实训操作";
                    break;
                case Gamemode.KaoHe:
                    _infoText.text = $"{index}.{data.UserName}同学在{data.Time}时，进行了{data.CourseName}科目的实训考核操作，成绩为{data.Score}分";
                    break;
            }
        }
    }
}
