using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class TSsxcztj_Page : BasePage
{
    public override PageUIType GetUIType()
    {
        return PageUIType.TSsxcztj_Page;
    }
    private GameObject _learningNotesItem;
    private Transform _ItemRoot;
    private BarChart _sxDurationChart;
    private Serie _sxDurationSerie;
    private TeacherClassTj_Panel _teacherClassTj_Panel
    {
        get
        {
            return TeacherTjPanel.GetPanel((int)PanelUIType.TeacherClassTj_Panel) as TeacherClassTj_Panel;
        }
    }
    private TeacherStudentTj_Panel _teacherStudentTj_Panel
    {
        get
        {
            return TeacherTjPanel.GetPanel((int)PanelUIType.TeacherStudentTj_Panel) as TeacherStudentTj_Panel;
        }
    }
    public override void Awake()
    {
        base.Awake();
        _learningNotesItem = transform.Find("Scroll View/Viewport/Content/View1/PrefabT/LearningNotesItem").gameObject;
        _ItemRoot = transform.Find("Scroll View/Viewport/Content/View1/Scroll View/Viewport/Content").transform;
        _sxDurationChart = transform.Find("Scroll View/Viewport/Content/View2/top/BarChart").GetComponent<BarChart>();
    }

    public override void Init()
    {
        InitLearnDatas();
    }
    public override void OnRefresh()
    {
        UpdateLearnDatas();
    }
    private void UpdateLearnDatas()
    {
        _ = NetHelper.Instance.GetSoftLearnDatasByClasses(AppController.Instance.softData, (datas) =>
        {
            AppController.Instance.classesLearnDatas = datas;
            List<SoftwareLearningData> _datas = new List<SoftwareLearningData>();
            if (TeacherTjPanel.currentPanel.name == "TeacherClassTj_Panel")
            {
                _datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherClassTj_Panel._StartTime && a.Time <= _teacherClassTj_Panel._EndTime && a.ClassInfo == _teacherClassTj_Panel._currentSelectClass);
            }
            else if (TeacherTjPanel.currentPanel.name == "TeacherStudentTj_Panel")
            {
                _datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherStudentTj_Panel._StartTime && a.Time <= _teacherStudentTj_Panel._EndTime && a.Account == _teacherStudentTj_Panel._currentStudentAccount);
            }
            //List<SoftwareLearningData> _datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherClassTj_Panel._StartTime && a.Time <= _teacherClassTj_Panel._EndTime && a.ClassInfo == _teacherClassTj_Panel._currentSelectClass);
            if (_datas != null && _datas.Count > 0)
            {

                SpawnDurationItem(_datas);
                ChartHelper.UpdateChart_AllUserCount<POTSType>(_datas,_sxDurationSerie, _sxDurationChart, LearnType.LearnTime, POTSType.None);
            }
            else
            {
                OnResetView();
            }
        });
    }
    private void InitLearnDatas()
    {
        if (AppController.Instance.classesLearnDatas == null || AppController.Instance.classesLearnDatas.Count <= 0)
        {
            UpdateLearnDatas();
        }
        else
        {
            List<SoftwareLearningData> datas = new List<SoftwareLearningData>();
            if (TeacherTjPanel.currentPanel.name == "TeacherClassTj_Panel")
            {
                datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherClassTj_Panel._StartTime && a.Time <= _teacherClassTj_Panel._EndTime && a.ClassInfo == _teacherClassTj_Panel._currentSelectClass);
            }
            else if (TeacherTjPanel.currentPanel.name == "TeacherStudentTj_Panel")
            {
                datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherStudentTj_Panel._StartTime && a.Time <= _teacherStudentTj_Panel._EndTime && a.Account == _teacherStudentTj_Panel._currentStudentAccount);
            }
            //List<SoftwareLearningData> datas = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherClassTj_Panel._StartTime && a.Time <= _teacherClassTj_Panel._EndTime && a.ClassInfo == _teacherClassTj_Panel._currentSelectClass);
            if (datas != null && datas.Count > 0)
            {

                SpawnDurationItem(datas);
                ChartHelper.UpdateChart_AllUserCount<POTSType>(datas,_sxDurationSerie, _sxDurationChart, LearnType.LearnTime, POTSType.None);
            }
            else
            {
                OnResetView();
            }
        }
    }
    private void SpawnDurationItem(List<SoftwareLearningData> datas)
    {
        DeleteChildren(_ItemRoot);
        List<int> courseIds = Enum.GetValues(typeof(POTSType))
          .Cast<POTSType>()
          .Where(type => !type.Equals(POTSType.None))
          .Select(type => (int)Convert.ChangeType(type, typeof(int)))
          .ToList();
        int sequenceNumber = 1;
        for (int i = 0; i < datas.Count; i++)
        {
            for (int j = 0; j < courseIds.Count; j++)
            {
                if (datas[i].CourseID == courseIds[j])
                {
                    GameObject go = Instantiate(_learningNotesItem, _ItemRoot);
                    LearningNotesItem learningNotesItem = go.AddComponent<LearningNotesItem>();
                    learningNotesItem.Init(sequenceNumber, datas[i], Gamemode.ShiXun);
                    sequenceNumber++;
                }
            }
        }
    }
    public override void OnResetView()
    {
        DeleteChildren(_ItemRoot);
        ChartHelper.ResetChart<POTSType>(_sxDurationSerie, _sxDurationChart, POTSType.None);
    }
}
