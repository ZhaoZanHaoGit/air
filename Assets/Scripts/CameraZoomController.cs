using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public UniversalInputDefinition inputDef; // 拖入你那个蓝色的配置文件
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Camera cam;

    void Start() { cam = GetComponent<Camera>(); }

    void Update()
    {
        float delta = inputDef.GetZoomDelta();
        if (Mathf.Abs(delta) > 0.001f)
        {
            if (cam.orthographic)
            {
                // 正交相机：改变 Size
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - delta * zoomSpeed, minZoom, maxZoom);
            }
            else
            {
                // 透视相机：改变 Field of View (FOV)
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - delta * zoomSpeed * 10f, 30f, 90f);
            }
        }
    }
}