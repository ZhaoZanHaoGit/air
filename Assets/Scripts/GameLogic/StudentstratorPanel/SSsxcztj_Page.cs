using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts.Runtime;

public class SSsxcztj_Page : BasePage
{
    public override PageUIType GetUIType()
    {
        return PageUIType.SSsxcztj_Page;
    }
    private GameObject _learningNotesItem;
    private Transform _ItemRoot;
    private BarChart _sxDurationChart;
    private Serie _sxDurationSerie;
    private StudentZHTj_Panel _studentZHTJPanel
    {
        get
        {
            StudentZHTj_Panel panel = StudentTjPanel.GetPanel((int)PanelUIType.StudentZHTj_Panel) as StudentZHTj_Panel;
            return panel;
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
        _ = NetHelper.Instance.GetSoftLearnDatasByAccount(AppController.Instance.loginUser.Account, (int)Defines.softInfo, (datas) =>
        {
            AppController.Instance.currentUserLearnDatas = datas;
            List<SoftwareLearningData> _datas = AppController.Instance.currentUserLearnDatas.FindAll(a => a.Time >= _studentZHTJPanel._StartTime && a.Time <= _studentZHTJPanel._EndTime);
            if (datas != null && datas.Count > 0)
            {
                SpawnDurationItem(datas);
                ChartHelper.UpdateChart<POTSType>(_sxDurationSerie, _sxDurationChart, LearnType.LearnTime, POTSType.None);
            }
            else {
                OnResetView();
            }
        });
    }
    private void InitLearnDatas()
    {
        if (AppController.Instance.currentUserLearnDatas == null || AppController.Instance.currentUserLearnDatas.Count <= 0)
        {
            UpdateLearnDatas();
        }
        else
        {
            List<SoftwareLearningData> datas = AppController.Instance.currentUserLearnDatas.FindAll(a => a.Time >= _studentZHTJPanel._StartTime && a.Time <= _studentZHTJPanel._EndTime);
            if (datas != null && datas.Count > 0)
            {
                SpawnDurationItem(datas);
                ChartHelper.UpdateChart<POTSType>(_sxDurationSerie, _sxDurationChart, LearnType.LearnTime, POTSType.None);
            }
            else {
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
        Dictionary<int, SoftwareLearningData> learnDataDict = AppController.Instance.currentUserLearnDatas
          .ToDictionary(data => data.CourseID, data => data);
        int sequenceNumber = 1; 
        for (int i = 0; i < courseIds.Count; i++)
        {
            int courseId = courseIds[i];
            if (learnDataDict.TryGetValue(courseId, out SoftwareLearningData data))
            {
                GameObject go = Instantiate(_learningNotesItem, _ItemRoot);
                LearningNotesItem learningNotesItem = go.AddComponent<LearningNotesItem>();
                learningNotesItem.Init(sequenceNumber, data, Gamemode.ShiXun);
                sequenceNumber++;
            }
        }
    }
    public override void OnResetView()
    {
        DeleteChildren(_ItemRoot);
        ChartHelper.ResetChart<POTSType>(_sxDurationSerie, _sxDurationChart, POTSType.None);
    }
}
