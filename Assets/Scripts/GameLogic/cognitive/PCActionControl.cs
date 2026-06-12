
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PCActionControl : MonoBehaviour

{
    public InputActionAsset inputActions; // 引用 InputActionAsset
    public Camera mainCamera; // 摄像机
    private InputAction moveAction; // 处理鼠标移动
    private InputAction rotateAction; // 处理鼠标右键旋转
    private InputAction clickAction; // 处理鼠标左键点击
    private InputAction hideAction; //处理鼠标左键隐藏
    private InputAction zoomAction;
    [HideInInspector] public GameObject mainGameobject;
    private GameObject mainGameobjectPerfab;
    private GameObject targetObject; // 当前控制的物体
    private bool isDragging = false;
    private bool isRotating = false;
    private Vector3 offset;
    InputActionMap playerActionMap;
    [SerializeField] private bool canDrag, canRotate,startDrag,startRotate;

    public string assetName;
    public Transform StartTransform;

    public Button all, drag, rotate, hide;

    public Button disassemble, combin, init;


    private float distanceToCamera; // 物体到相机的距离
    public float rotationSpeed = 1.0f;

    public bool CanDrag
    {
        get => canDrag;
        set => canDrag = value;
    }

    public bool CanRotate
    {
        get => canRotate;
        set => canRotate = value;
    }


    //相机缩放
    public float zoomSpeed = 5f; // 缩放速度
    public float minZoom = 10f; // 最小缩放值
    public float maxZoom = 50f; // 最大缩放值

    private float currentZoom = 30f; // 当前缩放值


    public float minX, maxX, minY, maxY;

    private void Start()
    {
        // 从 InputActionAsset 中获取 Action Map 和对应的 Action


        // 获取用于鼠标操作的 Action

        #region 输入绑定

        playerActionMap = inputActions.FindActionMap("RenZhi");
        clickAction = playerActionMap.FindAction("Click");

        hideAction = playerActionMap.FindAction("Hide");
        // moveAction = playerActionMap.FindAction("Move");
        rotateAction = playerActionMap.FindAction("Rotate");

        zoomAction = playerActionMap.FindAction("Zoom");


        // 订阅事件
        clickAction.started += OnMouseClickStarted;
        clickAction.canceled += OnMouseClickCanceled;
        rotateAction.performed += OnMouseRightPress;
        rotateAction.canceled += OnMouseRightRelease;
        hideAction.performed += HideOBJ;
        zoomAction.performed += ctx => Zoom(ctx.ReadValue<float>());

        // 启用 Action Map
        playerActionMap.Enable();
        MoveActionControl(startDrag);
        RotateActionControl(startRotate);
        hideAction.Disable();


        BindUI();

        #endregion

        Addressables.LoadAssetAsync<GameObject>(assetName).Completed += OnAssetLoaded;
    }

    private void Update()
    {
        // 执行拖拽
        if (isDragging && !isRotating && targetObject != null)
        {
            DragObjectClamp();
        }

        // 执行旋转
        if (isRotating && !isDragging && targetObject != null)
        {
            RotateObject();
        }
        /*
        if (!hideAction.enabled)
        {
            Debug.Log("ClickAction 仍然被禁用");
        }
        else
        {
            Debug.Log("ClickAction 被起用");
        }
        */
    }

    private void OnDestroy()
    {
        playerActionMap.Disable();
        clickAction.started -= OnMouseClickStarted;
        clickAction.canceled -= OnMouseClickCanceled;
        rotateAction.performed -= OnMouseRightPress;
        rotateAction.canceled -= OnMouseRightRelease;
        hideAction.performed -= HideOBJ;
        zoomAction.RemoveAllBindingOverrides(); //-= ctx => Zoom(ctx.ReadValue<float>());
    }


    #region UI绑定

    private void BindUI()
    {
        if (drag != null)
        {
            drag.onClick.AddListener(() =>
            {
                Debug.Log("11111");
                MoveActionControl(true);
            });
            drag.onClick.AddListener(() =>
            {
                Debug.Log("22222");

                MoveActionControl(false);
            });
        }

        if (rotate != null)
        {
            rotate.onClick.AddListener(() =>
            {
                Debug.Log("11111");
                RotateActionControl(true);
            });

            rotate.onClick.AddListener(() =>
            {
                Debug.Log("22222");

                RotateActionControl(false);
            });
        }

        if (hide != null)
        {
            hide.onClick.AddListener(() =>
            {
                Debug.Log("11111");
                HideActionControl(true);
            });

            hide.onClick.AddListener(() =>
            {
                Debug.Log("22222");

                HideActionControl(false);
            });
        }

        if (all != null)
        {
            all.onClick.AddListener(() =>
            {
                Debug.Log("11111");
                MainOBJInteractionState(false);
            });

            all.onClick.AddListener(() =>
            {
                Debug.Log("22222");

                MainOBJInteractionState(true);
            });
        }

        if (disassemble != null)
        {
            disassemble.onClick.AddListener(MainOBJDisassemble);
        }

        if (combin != null)
        {
            combin.onClick.AddListener(MainOBJCombin);
        }

        if (init != null)
        {
            init.onClick.AddListener(InitMainOBJ);
        }
    }

    #endregion


    #region ui交互

    /// <summary>
    /// 控制旋转输入开关
    /// </summary>
    /// <param name="isopen"></param>
    public void RotateActionControl(bool isopen)
    {
        canRotate = isopen;
        if (rotateAction != null)
        {
            if (isopen)
            {
                rotateAction.Enable();
            }
            else
            {
                rotateAction.Disable();
            }
        }
    }

    /// <summary>
    /// 控制移动输入开关
    /// </summary>
    /// <param name="isopen"></param>
    public void MoveActionControl(bool isopen)
    {
        canDrag = isopen;
        if (clickAction != null && hideAction != null)
        {
            if (isopen)
            {
                clickAction.Enable();
                hideAction.Disable();
            }
            else
            {
                clickAction.Disable();
            }
        }
    }

    /// <summary>
    /// 隐藏开关
    /// </summary>
    /// <param name="isopen"></param>
    public void HideActionControl(bool isopen)
    {
        if (clickAction != null && hideAction != null)
        {
            if (isopen)
            {
                clickAction.Disable();
                hideAction.Enable();
            }
            else
            {
                hideAction.Disable();
            }
        }
    }

    /// <summary>
    /// 重置物体
    /// </summary>
    public void InitMainOBJ()
    {
        if (mainGameobject != null)
        {
            GameObject newMainObj =
                Instantiate(mainGameobjectPerfab, StartTransform.position, StartTransform.rotation);
            Destroy(mainGameobject);

            mainGameobject = newMainObj;
            mainGameobject.SetActive(true);
            if (all)
            {
               
            }
            
        }
    }

    /// <summary>
    /// 当前物体拆分
    /// </summary>
    public void MainOBJDisassemble()
    {
        if (mainGameobject != null && mainGameobject.GetComponent<CognitiveIntreaction>() != null)
        {
            mainGameobject.GetComponent<CognitiveIntreaction>().Disassemble();
        }
    }

    /// <summary>
    /// 当前物体组合
    /// </summary>
    public void MainOBJCombin()
    {
        if (mainGameobject != null && mainGameobject.GetComponent<CognitiveIntreaction>() != null)
        {
            mainGameobject.GetComponent<CognitiveIntreaction>().Combin();
        }
    }

    /// <summary>
    /// 当前物体交互主体切换
    /// </summary>
    /// <param name="state"></param>
    public void MainOBJInteractionState(bool state)
    {
        if (mainGameobject != null && mainGameobject.GetComponent<CognitiveIntreaction>() != null)
        {
            mainGameobject.GetComponent<CognitiveIntreaction>().SetInteractionState(state);
        }
    }

    #endregion

    private void OnDisable()
    {
        // 禁用 Action Map
        var playerActionMap = inputActions.FindActionMap("RenZhi");
        playerActionMap.Disable();
    }

    private void OnMouseClickStarted(InputAction.CallbackContext context)
    {
        if (canDrag && !isRotating)
        {
            if (targetObject == null)
            {
                // 鼠标左键点击开始时，进行射线检测
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out hit))
                {
                    // 如果点击的是物体，将其设置为目标物体
                    targetObject = hit.collider.gameObject;
                    Debug.Log("Selected object: " + targetObject.name);

                    distanceToCamera = hit.distance; // 记录物体与相机的距离
                    isDragging = true;
                }
            }
        }
    }

    private void OnMouseClickCanceled(InputAction.CallbackContext context)
    {
        // 鼠标左键松开时停止拖拽
        isDragging = false;
        if (!isRotating)
            targetObject = null;
    }

    private void OnMouseRightPress(InputAction.CallbackContext context)
    {
        if (canRotate && !isDragging)
        {
            if (targetObject == null)
            {
                // 右键按下时，开始旋转
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out hit))
                {
                    // 如果点击的是物体，将其设置为目标物体
                    targetObject = hit.collider.gameObject;
                    Debug.Log("Selected object: " + targetObject.name);
                }
            }

            isRotating = true; 
            lastMousePosition = Input.mousePosition; 
        }
    }

    private void OnMouseRightRelease(InputAction.CallbackContext context)
    {
        // 右键松开时，停止旋转
        isRotating = false;
        if (!isDragging)
        {
            targetObject = null;
        }
    }

    private void HideOBJ(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit))
        {
            // 如果点击的是物体，将其设置为目标物体
            targetObject = hit.collider.gameObject;
            Debug.Log("Hide object: " + targetObject.name);
            hit.collider.gameObject.SetActive(false);
        }
    }


    // 拖拽物体
    void DragObject()
    {
        // 获取鼠标当前的世界坐标
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float z = mainCamera.WorldToScreenPoint(targetObject.transform.position).z; // 获取物体当前的z坐标
        Vector3 mousePosition = ray.GetPoint(distanceToCamera);
        // 设置物体的新位置，保持物体与相机的距离
        targetObject.transform.position =
            new Vector3(mousePosition.x, mousePosition.y, targetObject.transform.position.z);
    }

    void DragObjectClamp()
    {
        // 获取鼠标当前的世界坐标
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float z = mainCamera.WorldToScreenPoint(targetObject.transform.position).z; // 获取物体当前的z坐标
        Vector3 mousePosition = ray.GetPoint(distanceToCamera);

        // 限制X, Y轴的拖动范围
        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY);

        // 设置物体的新位置，保持物体与相机的距离，并限制范围
        targetObject.transform.position = new Vector3(clampedX, clampedY, targetObject.transform.position.z);
    }
 private Vector3 lastMousePosition;
    // 旋转物体
    void RotateObject()
    {/*
        // 获取鼠标的移动量
        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();
        // 旋转物体，绕 Y 轴旋转
        targetObject.transform.Rotate(Vector3.up, mouseX * rotationSpeed);*/
       
        Vector3 delta = Input.mousePosition - lastMousePosition; // 计算鼠标移动的距离
        float rotationX = delta.y * rotationSpeed * Time.deltaTime; // 计算绕X轴旋转的角度
        float rotationY = delta.x * rotationSpeed * Time.deltaTime; // 计算绕Y轴旋转的角度

        // 应用旋转
        targetObject.transform.Rotate(Vector3.left * rotationX, Space.World);
        targetObject.transform.Rotate(Vector3.up * rotationY, Space.World);

        // 更新鼠标位置
        lastMousePosition = Input.mousePosition;
        
    }
   

    private void OnAssetLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            // 加载成功后实例化资产
            mainGameobjectPerfab = obj.Result;
            mainGameobject = Instantiate(mainGameobjectPerfab, StartTransform.position, StartTransform.rotation);
        }
        else
        {
            Debug.LogError("Failed to load asset: " + assetName);
        }
    }


    private void Zoom(float zoomInput)
    {
        currentZoom -= zoomInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        Camera.main.fieldOfView = currentZoom;
    }
}