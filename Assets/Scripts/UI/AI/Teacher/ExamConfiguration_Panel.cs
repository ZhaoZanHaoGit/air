using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExamConfiguration_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.ExamConfiguration_Panel;
    }
    private Button resBtn, saveBtn;
    public TMP_InputField[] timerInputs;
    private Dictionary<int, TMP_InputField> dic = new Dictionary<int, TMP_InputField>();
    public override void Awake()
    {
        base.Awake();
        resBtn= transform.Find("scoreview/resButton").GetComponent<Button>();
        saveBtn = transform.Find("scoreview/saveButton").GetComponent<Button>();
        for (int i = 0; i < timerInputs.Length; i++)
        {
            dic.Add(300 + 1 + i, timerInputs[i]);
        }
    }
    private void Start()
    {
        resBtn.onClick.AddListener(() =>
        {
            foreach (var item in timerInputs)
            {
                item.text = "10";
            }
        });
        saveBtn.onClick.AddListener(() =>
        {
            bool allNotEmpty = timerInputs.All(inputField => !string.IsNullOrEmpty(inputField.text));
            if (!allNotEmpty)
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "数据不完整！", 0, EnumMessageBoxType.OK);
                return;
            }
            ExamSettingDatas examSettingDatas = new ExamSettingDatas();
            for (int i = 0; i < timerInputs.Length; i++)
            {
                ExamSettingData data = new ExamSettingData
                {
                    CourseType = 301 + i,
                    AllTimer = int.Parse(timerInputs[i].text.Trim())
                };
                examSettingDatas.datas.Add(data);
            }
            string datajson = JsonConvert.SerializeObject(examSettingDatas);
            SoftExamSettingDatas data1 = new SoftExamSettingDatas
            {
                SoftID = (int)Defines.softInfo,
                Schoolname = AppController.Instance.loginUser.School_name,
                DataJson = datajson
            };
            _ = NetHelper.Instance.AddSoftExamSettingData(data1, code => {

                if (code == 0)
                {
                    UIManager.Instance.OpenMessageBoxUI("提示信息", "保存成功！", 3, EnumMessageBoxType.OK);
                    AppController.Instance.examSettingDatas = examSettingDatas;
                }
                else {
                    UIManager.Instance.OpenMessageBoxUI("提示信息", "保存失败！", 3, EnumMessageBoxType.OK);
                }
            });
        });
    }
    public override void Init()
    {
        if (AppController.Instance.examSettingDatas != null)
        {
            ShowTimer();
        }
        else
        {
            OnRefresh();
        }
    }
    public override void OnRefresh()
    {
        SoftExamSettingDatas data = new SoftExamSettingDatas
        {
            SoftID = (int)Defines.softInfo,
            Schoolname = AppController.Instance.loginUser.School_name,
            DataJson = ""
        };
        _ = NetHelper.Instance.GetSoftExamSettingData(data, _data =>
        {
            //Debug.Log(_data.DataJson);
            if (_data != null && !string.IsNullOrEmpty(_data.DataJson))
            {

                AppController.Instance.examSettingDatas = JsonConvert.DeserializeObject<ExamSettingDatas>(_data.DataJson);
                ShowTimer();
            }
        });
    }
    void ShowTimer()
    {
        var dataList = AppController.Instance.examSettingDatas.datas;
        for (int i = 0; i < dataList.Count; i++)
        {
            var courseType = dataList[i].CourseType;
            if (dic.TryGetValue(courseType, out var field) && field != null)
            {
                field.text = dataList[i].AllTimer.ToString();
            }
            else
            {
                Debug.LogWarning($"[ExamSettingsPanel] 未找到 CourseType={courseType} 对应的输入框");
            }
        }
    }
}
