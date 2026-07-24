using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tuzhi : MonoBehaviour
{
    [Header("回路图显示区")]
    public Transform schematicContainer;    // 生成图片的父节点
    public GameObject schematicImagePrefab; // 带有 Image 组件的预制体

    [Header("缩放控制")]
    public Button zoomInButton;            // 放大按钮
    public Button zoomOutButton;           // 缩小按钮

    private float currentScale = 1f;       // 当前缩放比例
    private readonly float scaleStep = 0.5f; // 每次缩放步长
    private readonly float minScale = 0.5f; // 最小缩放
    private readonly float maxScale = 5f;   // 最大缩放

    // 记录每张图片的原始尺寸，用于缩放计算
    private List<Vector2> baseImageSizes = new List<Vector2>();

    private void OnEnable()
    {
        List<string> names = SimulationLoop.Instance.CurrentCase.CircuitSchematicNames;
        UpdateCircuitSchematicsUI(names);
    }

    private void Start()
    {
        if (zoomInButton != null)
            zoomInButton.onClick.AddListener(() => Zoom(scaleStep));
        if (zoomOutButton != null)
            zoomOutButton.onClick.AddListener(() => Zoom(-scaleStep));
    }

    /// <summary>
    /// 缩放所有回路图图片，并同步更新 ScrollRect 布局
    /// </summary>
    /// <param name="delta">缩放增量（正数放大，负数缩小）</param>
    public void Zoom(float delta)
    {
        currentScale = Mathf.Clamp(currentScale + delta, minScale, maxScale);
        ApplyScale();
    }

    private void ApplyScale()
    {
        int index = 0;
        foreach (Transform child in schematicContainer)
        {
            if (index >= baseImageSizes.Count) break;
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.rectTransform.sizeDelta = baseImageSizes[index] * currentScale;
            }
            index++;
        }

        // 强制重建布局，使 ContentSizeFitter / LayoutGroup 重新计算容器尺寸
        // 这样 ScrollRect 才能正确更新滚动条和拖动范围
        LayoutRebuilder.ForceRebuildLayoutImmediate(schematicContainer as RectTransform);
    }
    /// <summary>
    /// 更新回路图 UI (CircuitSchematicNames)
    /// </summary>
    private void UpdateCircuitSchematicsUI(List<string> schematicNames)
    {
        // 每次点击新案例时，清空之前生成的旧图片
        baseImageSizes.Clear();
        currentScale = 1f;
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

                // 记录原始尺寸，供缩放使用
                baseImageSizes.Add(rect.sizeDelta);
            }
            else
            {
                Debug.LogWarning($"缺少回路图资源: Assets/Resources/CircuitSchematic/{schematicName}");
            }
        }
    }
}
