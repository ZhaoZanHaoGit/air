using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StudentCourseStatistics_Page : BasePage
{
    public override PageUIType GetUIType()
    {
        return PageUIType.StudentCourseStatistics_Page;
    }
    private StudentZHTJItem _studentZHTJItem;
    private Transform _Root;
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
        _studentZHTJItem = transform.Find("PrefabT/StudentZHTJItem").GetComponent<StudentZHTJItem>();
        _Root = transform.Find("View/Scroll View/Viewport/Content").transform;
    }
    public override void Init()
    {
        InitLearnDatas();
        //Debug.Log("课程统计简略初始化");
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
            _datas = _datas.OrderByDescending(x => x.Time).ToList();
            if (_datas != null || _datas.Count > 0)
            {
                SpawnLearnUIData(_datas);
            }
            else {
                DeleteChildren(_Root);
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
            datas = datas.OrderByDescending(x => x.Time).ToList();
            if (datas != null || datas.Count > 0)
            {
                Debug.Log(datas.Count);
                SpawnLearnUIData(datas);
            }
            else {
                DeleteChildren(_Root);
            }
        }
    }
    private void SpawnLearnUIData(List<SoftwareLearningData> datas)
    {
        DeleteChildren(_Root);
        for (int i = 0; i < datas.Count; i++)
        {
            GameObject go = Instantiate(_studentZHTJItem.gameObject, _Root);
            StudentZHTJItem item = go.GetComponent<StudentZHTJItem>();
            item.OnInit(i + 1, datas[i]);
        }
    }
}
