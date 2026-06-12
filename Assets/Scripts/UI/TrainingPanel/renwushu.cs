using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class renwushu : MonoBehaviour
{
    [Header("任务书显示区")]
    public TMP_Text taskDocumentText;       // 显示 TaskDocument 信息的 TextMeshPro
   // public TMP_Text taskName;       // 显示 TaskDocument 信息的 TextMeshPro



    private void OnEnable()
    {
        TaskDocument doc = SimulationLoop.Instance.CurrentCase.TaskDocument;

        UpdateTaskDocumentUI(doc);
    }
    /// <summary>
    /// 更新任务书 UI (TaskDocument)
    /// </summary>
    private void UpdateTaskDocumentUI(TaskDocument doc)
    {
        if (doc == null)
        {
            taskDocumentText.text = "暂无任务书数据。";
            return;
        }


        // 将任务书内的不同部分拼接显示，若部分字段为空则自动忽略
        string content = "";

        if (!string.IsNullOrEmpty(doc.TaskRequirements))
            content += $"<b>任务要求：</b>\n{doc.TaskRequirements}\n\n";

        if (!string.IsNullOrEmpty(doc.TechnicalDocumentation))
            content += $"<b>技术资料：</b>\n{doc.TechnicalDocumentation}\n\n";

        if (!string.IsNullOrEmpty(doc.TaskImplementation))
            content += $"<b>任务实施：</b>\n{doc.TaskImplementation}";

        taskDocumentText.text = content;
        RectTransform Rect = taskDocumentText.GetComponent<RectTransform>();
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
    }
}
