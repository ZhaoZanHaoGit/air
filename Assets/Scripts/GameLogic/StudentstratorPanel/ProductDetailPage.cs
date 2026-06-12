using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProductDetailPage : BaseWindowPanel
{
    
    private Button closeBtn;
    private Toggle operationStepToggle, workOrderToggle;
    public GameObject operationStepPage, workOrderPage;
    //public TMP_Text operationStepText, workOrderStepText;
    private SoftwareLearningData Data;
    public VerticalLayoutGroup operationStepVercalGroup, workOrderVercalGroup;
    private BuiltinPool operationContent, workOrderConnect;

    private void Awake()
    {
        closeBtn = transform.Find("bg/closeButton").GetComponent<Button>();
        operationStepToggle = transform.Find("bg/btns/Toggle").GetComponent<Toggle>();
        workOrderToggle = transform.Find("bg/btns/Toggle_1").GetComponent<Toggle>();
        operationContent = transform.Find("bg/OperationContent").GetComponent<BuiltinPool>();
        workOrderConnect = transform.Find("bg/WorkOrderConnect").GetComponent<BuiltinPool>();

    }
    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            operationContent.Despawn();
            workOrderConnect.Despawn();
            OnHide();
        });
        operationStepToggle.onValueChanged.AddListener(isOn =>
        {
            operationStepPage.SetActive(isOn);
            ShowOperationStepInfo();
        });
        workOrderToggle.onValueChanged.AddListener(isOn =>
        {
            workOrderPage.SetActive(isOn);
            ShowWorkOrderInfo();
        });
    }
    public void Init(SoftwareLearningData data)
    {
        Data = data;
        operationStepToggle.isOn = true;
        ShowOperationStepInfo();
    }
    public void ShowOperationStepInfo()
    {
        //operationStepText.text = "";
        if (!string.IsNullOrEmpty(Data.OperationContent))
        {
            string url = Data.OperationContent;
            PhpHelper.Instance.LoadTxt(url, str =>
            {
                ParseOperationStepInfo(str);
            });

        }
    }
    public void ShowWorkOrderInfo()
    {
        //workOrderStepText.text = "";
        if (!string.IsNullOrEmpty(Data.WorkOrder))
        {
            string url = Data.WorkOrder;
            PhpHelper.Instance.LoadTxt(url, str =>
            {
                ParseWorkOrderInfo(str);
            });
        }
    }

    void ParseOperationStepInfo(string infostr)
    {
        if (operationContent.GetCounts()>0) {
            return;
        }
        OperationStepsData operationStepsData = JsonConvert.DeserializeObject<OperationStepsData>(infostr);
        //int i = 1;
        List<StepInfo> steps = operationStepsData.StepInfos;
        List<StepInfo> distinctSteps = steps
            .GroupBy(s => new { s.StepName, s.CreatTime })
            .Select(g => g.First())
            .ToList();

        for (int j = 0; j < distinctSteps.Count; j++) {
            GameObject go = operationContent.Spawn(operationStepVercalGroup.transform);
            go.GetComponent<OperationItem>().Init(j+1, distinctSteps[j]);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(operationStepVercalGroup.GetComponent<RectTransform>());
    }

    void ParseWorkOrderInfo(string infostr)
    {
        if (workOrderConnect.GetCounts()>0) {
            return;
        }
        //StringBuilder sb = new StringBuilder();
        WorkOrder workOrder = JsonConvert.DeserializeObject<WorkOrder>(infostr);
        for (int i = 0; i < workOrder.sections.Count; i++)
        {
            for (int j = 0; j < workOrder.sections[i].checkItems.Count; j++)
            {
                GameObject go = workOrderConnect.Spawn(workOrderVercalGroup.transform);
                go.GetComponent<WorkOrderItem>().Init(j+1, workOrder.sections[i].checkItems[j]);
            }
        }
        //workOrderStepText.text = sb.ToString(); // 最后一次性赋值给 UI 文本
        LayoutRebuilder.ForceRebuildLayoutImmediate(workOrderVercalGroup.GetComponent<RectTransform>());
    }
}

