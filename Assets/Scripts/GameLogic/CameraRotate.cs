using UnityEngine;

/// <summary>
/// 相机围绕物体全方面观察的脚本类
/// 可以旋转自主控制观察角度等等
/// 同时相机默认在自动围绕观察
/// </summary>
public class CameraRotate : MonoBehaviour
{
    [Header("相机观察的目标物体")]
    public Transform targetObject;
    public Vector3 targetOffset;
    public float averageDistance = 5.0f;
    public float maxDistance = 20;
    [Header("相机距离观察目标物体的最小距离，距离过小，相机会进入物体内部")]
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    [Header("相机y轴视野范围")]
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    [Header("滑轮控制缩放的速度")]
    public int zoomSpeed = 40;

    public float panSpeed = 0.3f;
    [Header("旋转阻尼")]
    public float zoomDampening = 5.0f;
    [Header("相机自主旋转开关阀值，等于小于0既是关闭自主旋转")]
    public float rotateOnOff = 1;
    [Header("相机自主旋转速率")]
    public float amaze;
    [Header("鼠标控制视野旋转开关")]
    public bool buttonDown;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private float idleTimer = 0.0f;
    private float idleSmooth = 0.0f;

    // NEW: 记录初始视角/距离
    private float initXDeg;
    private float initYDeg;
    private float initDistance;
    private Quaternion initRotation;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
      
        //如果目标物体为空，则在相机正前方aberageDistance处生成一个观察目标
        if (!targetObject)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * averageDistance);
            targetObject = go.transform;
        }

        currentDistance = averageDistance;
        desiredDistance = averageDistance;

        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
        //获取相机当前在x和y轴上的一个旋转角度
        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
        //相机位置=要观察的目标位置-目标和相机在三维下的位置差值（目标的旋转*世界坐标正前方*距离+目标差值）
        position = targetObject.position - (rotation * Vector3.forward * currentDistance + targetOffset);

        // NEW: 保存初始视角/距离
        initXDeg = xDeg;
        initYDeg = yDeg;
        initDistance = currentDistance;
        initRotation = rotation;
    }

    void LateUpdate()
    {
        //if (UIHelper.Instance.IsPointerOverUI()) return;
        // NEW: 快捷键恢复初始视角
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToInitialView();
            return; // 本帧直接用初始值，不再继续计算
        }
        if (Input.GetMouseButton(1) && !buttonDown)//按下鼠标左键
        {
            //通过鼠标的移动控制相机的旋转角度
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);//限制y轴方向上的旋转角度

            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            //通过插值运算将旋转后的相机角度赋值给当前相机
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening);
            transform.rotation = rotation;
            idleTimer = 0;
            //    idleSmooth=0;

        }
        else
        {//当鼠标没有操作时，相机默认自动围绕目标物体水平旋转
            idleTimer += 0.02f;
            if (idleTimer > rotateOnOff && rotateOnOff > 0)
            {
                idleSmooth += (0.02f + idleSmooth) * amaze;
                idleSmooth = Mathf.Clamp(idleSmooth, 0, 1);
                xDeg += xSpeed * 0.001f * idleSmooth;
            }

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening * 2);
            transform.rotation = rotation;
        }
        if (!UIHelper.Instance.IsPointerOverUI()) {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                desiredDistance -= scroll * 0.02f * zoomSpeed * Mathf.Abs(desiredDistance);
                desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            }
        }
        // 其余插值和位置更新照旧
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * zoomDampening);
        position = targetObject.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }

    /// <summary>
    /// 限制角度angle的大小在一个范围内
    /// </summary>
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// 设置自主旋转速率
    /// </summary>
    public void offRotation(float num)
    {
        amaze = num;
    }
    public void ButtonDown()
    {
        buttonDown = true;
    }
    public void ButtonUp()
    {
        buttonDown = false;
    }

    // =========================
    // NEW: 恢复/保存 初始视角功能
    // =========================

    /// <summary>
    /// 恢复到初始视角与距离（Start/OnEnable时记录）
    /// </summary>
    public void ResetToInitialView()
    {
        // 恢复角度变量与插值目标，避免恢复后一两帧被Lerp“拉走”
        xDeg = initXDeg;
        yDeg = initYDeg;
        desiredRotation = initRotation;
        currentRotation = initRotation;
        rotation = initRotation;

        desiredDistance = Mathf.Clamp(initDistance, minDistance, maxDistance);
        currentDistance = desiredDistance;

        // 直接设置相机的旋转与位置
        transform.rotation = initRotation;
        transform.position = targetObject.position - (rotation * Vector3.forward * currentDistance + targetOffset);

        // 清空空闲旋转计时
        idleTimer = 0f;
        idleSmooth = 0f;
    }

    /// <summary>
    /// 将“当前视角/距离”保存为新的初始视角（可绑定到按钮）
    /// </summary>
    public void SaveCurrentAsInitialView()
    {
        // 从当前transform反推 xDeg / yDeg（与初始化时一致的计算方式）
        initXDeg = Vector3.Angle(Vector3.right, transform.right);
        initYDeg = Vector3.Angle(Vector3.up, transform.up);
        initDistance = currentDistance;
        initRotation = transform.rotation;
    }
}
