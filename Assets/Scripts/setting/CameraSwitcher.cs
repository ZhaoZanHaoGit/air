
using UnityEngine;


public class CameraSwitcher : MonoBehaviour
{
    public Camera vrCamera; // VR摄像机
    public Camera desktopCamera; // 桌面显示摄像机
    private bool isShowingDesktopCamera = true;
    public float movementSpeed = 5.0f; // 控制移动速度
    public float mouseSensitivity = 100.0f; // 控制鼠标旋转灵敏度

    private float xRotation = 0f; // 用于上下旋转

    // 定义移动范围限制
    public Vector2 horizontalMovementRange = new Vector2(-10f, 10f);
    public Vector2 verticalMovementRange = new Vector2(-5f, 5f);
    public Vector2 depthMovementRange = new Vector2(-10f, 10f);
    private bool wander;

    void Start()
    {
        wander = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // 按下空格键切换摄像机
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeWander();
            
            // 当桌面摄像机启用时，允许通过WASD和鼠标控制
            // Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
        
        if (wander)
        {
            MoveCamera();
            RotateCamera();
            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = false;
        }
    }
    void SwitchCamera()
    {
        desktopCamera.enabled = !desktopCamera.enabled;
    }

    void MoveCamera()
    {
        float x = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
       

        // 新增：处理Q和E键的输入以控制上下移动
        float y = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            y = -movementSpeed * Time.deltaTime; // 向下移动
        }
        else if (Input.GetKey(KeyCode.E))
        {
            y = movementSpeed * Time.deltaTime; // 向上移动
        }

        Vector3 nextPosition = desktopCamera.transform.parent.position + desktopCamera.transform.parent.right * x + desktopCamera.transform.parent.up * y + desktopCamera.transform.parent.forward * z;

        // 限制摄像机的移动范围
        nextPosition.x = Mathf.Clamp(nextPosition.x, horizontalMovementRange.x, horizontalMovementRange.y);
        nextPosition.y = Mathf.Clamp(nextPosition.y, verticalMovementRange.x, verticalMovementRange.y);
        nextPosition.z = Mathf.Clamp(nextPosition.z, depthMovementRange.x, depthMovementRange.y);

        desktopCamera.transform.parent.position = nextPosition;
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        Debug.Log(mouseX + mouseY);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 防止过度旋转
                                               
        // 应用垂直旋转
        desktopCamera.transform.localRotation = Quaternion.Euler(xRotation, transform.rotation.x, 0f);

        // 应用水平旋转 (Y轴) - 需要应用于桌面摄像机或其父对象
        desktopCamera.transform.parent.Rotate(Vector3.up * mouseX);

    }

    void ChangeWander()
    {
        wander= !wander;
        if (wander)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

}
