using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeacherCourseStatistics_Page : BasePage
{
    public override PageUIType GetUIType()
    {
        return PageUIType.TeacherCourseStatistics_Page;
    }
    private StudentZHTJItem _studentZHTJItem;
    private Transform _Root;
    private TeacherStudentTj_Panel _teacherZHTJPanel
    {
        get
        {
            return TeacherTjPanel.GetPanel((int)PanelUIType.TeacherStudentTj_Panel) as TeacherStudentTj_Panel;
        }
    }
    public override void Awake()
    {
        base.Awake();
        _studentZHTJItem = transform.Find("PrefabT/ZhtjItem").GetComponent<StudentZHTJItem>();
        _Root = transform.Find("View/Scroll View/Viewport/Content").transform;
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
            List<SoftwareLearningData> dataArray = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherZHTJPanel._StartTime && a.Time <= _teacherZHTJPanel._EndTime &&a.Account== _teacherZHTJPanel._currentStudentAccount);
            dataArray = dataArray.OrderByDescending(x => x.Time).ToList();
            
            if (dataArray != null || dataArray.Count > 0)
            {
                //Debug.Log(dataArray.Count);
                SpawnLearnUIData(dataArray);
            }
            else {
                DeleteChildren(_Root);
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
            List<SoftwareLearningData> dataArray = AppController.Instance.classesLearnDatas.FindAll(a => a.Time >= _teacherZHTJPanel._StartTime && a.Time <= _teacherZHTJPanel._EndTime && a.Account == _teacherZHTJPanel._currentStudentAccount);
            dataArray = dataArray.OrderByDescending(x => x.Time).ToList();
            if (dataArray != null || dataArray.Count > 0)
            {
                Debug.Log(dataArray.Count);
                SpawnLearnUIData(dataArray);
            }
            else
            {
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
