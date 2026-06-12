using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tuzhi : MonoBehaviour
{
    [Header("回路图显示区")]
    public Transform schematicContainer;    // 生成图片的父节点
    public GameObject schematicImagePrefab; // 带有 Image 组件的预制体
                                            // Start is called before the first frame update

    private void OnEnable()
    {
        List<string> names = SimulationLoop.Instance.CurrentCase.CircuitSchematicNames;
        UpdateCircuitSchematicsUI(names);
    }
    /// <summary>
    /// 更新回路图 UI (CircuitSchematicNames)
    /// </summary>
    private void UpdateCircuitSchematicsUI(List<string> schematicNames)
    {
        // 每次点击新案例时，清空之前生成的旧图片
        foreach (Transform child in schematicContainer)
        {
            Destroy(child.gameObject);
        }

        if (schematicNames == null || schematicNames.Count == 0) return;

        foreach (string schematicName in schematicNames)
        {
            if (string.IsNullOrEmpty(schematicName)) continue;

            // 在 Assets/Resources/CircuitSchematic 目录下加载同名 Sprite 资源
            Sprite loadedSprite = Resources.Load<Sprite>($"CircuitSchematic/{schematicName}");

            if (loadedSprite != null)
            {
                // 创建图片对象并设置资源
                GameObject imgObj = Instantiate(schematicImagePrefab, schematicContainer);
                Image img = imgObj.GetComponent<Image>();
                img.sprite = loadedSprite;

                // 1. 先让 Image 恢复到资源的原始物理像素尺寸
                img.SetNativeSize();

                // 2. 获取 RectTransform 并将宽高各减半
                RectTransform rect = img.rectTransform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x * 0.5f, rect.sizeDelta.y * 0.5f);

            }
            else
            {
                Debug.LogWarning($"缺少回路图资源: Assets/Resources/CircuitSchematic/{schematicName}");
            }
        }
    }
}
