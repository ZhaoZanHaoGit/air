using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

public class AdvancedZoomController : MonoBehaviour
{
    public UniversalInputDefinition inputDef; // 你的蓝色配置文件
    public float zoomSpeed = 0.1f;
    public float minSize = 2f;
    public float maxSize = 20f;

    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    void Update()
    {
        float delta = inputDef.GetZoomDelta();
        if (Mathf.Abs(delta) < 0.0001f) return;

        // 1. 获取缩放中心点的屏幕坐标
        Vector2 screenPivot = inputDef.GetZoomScreenPivot();

        // 2. 将屏幕中心点转换到世界坐标
        Vector3 worldPivotBeforeZoom = cam.ScreenToWorldPoint(new Vector3(screenPivot.x, screenPivot.y, cam.nearClipPlane));

        // 3. 执行缩放
        float oldSize = cam.orthographicSize;
        float newSize = Mathf.Clamp(oldSize - delta * zoomSpeed * oldSize, minSize, maxSize);
        cam.orthographicSize = newSize;

        // 4. 关键：重新计算缩放后的世界坐标位置并补偿位移
        // 缩放后，同一个屏幕点对应的世界坐标会发生偏移
        Vector3 worldPivotAfterZoom = cam.ScreenToWorldPoint(new Vector3(screenPivot.x, screenPivot.y, cam.nearClipPlane));

        // 补偿相机位置差值，使得中心点看起来“没动”
        transform.position += (worldPivotBeforeZoom - worldPivotAfterZoom);
    }
}
