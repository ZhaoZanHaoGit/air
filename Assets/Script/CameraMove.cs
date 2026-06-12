using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CameraMove : MonoBehaviour
{
    public float sensitivityMouse = 0.5f;
    public float sensitivetyKeyBoard = 0.5f;   // 注意这里拼写有误，建议改为 sensitivityKeyboard
    public GameObject MarkerList;
    public GameObject TextMarkerList;
    public PlayableDirector director;

    private float movementX, movementY, movementZ;
    private bool moving;
    private bool requestStartMoving = false;
    private GameObject touchOnGo = null;
    private float totalMoveDistance = 0;
    private float minHeight = 5.0f;
    public float maxHeight = 20.0f;
    public bool isMoving = false;

    private void Awake()
    {
    }

    private void OnMove(InputValue movementValue)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector2 vec = movementValue.Get<Vector2>();
        movementX = vec.x;
        movementY = vec.y;
    }

    private void OnLook(InputValue movementValue)
    {
        if (Camera.main.orthographic)
        {
            return;
        }

        Vector2 vec = movementValue.Get<Vector2>();

        if (touchOnGo != null)
        {
            totalMoveDistance += Math.Abs(vec.x) + Math.Abs(vec.y);
            if (totalMoveDistance < 5)
            {
                return;
            }
            touchOnGo = null;
            moving = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!moving)
        {
            return;
        }

        float rotationX = transform.localEulerAngles.y + vec.x * sensitivityMouse;
        float rotationY = -transform.localEulerAngles.x;

        if (rotationY < -180)
        {
            rotationY += 360;
        }

        rotationY += vec.y * sensitivityMouse;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    private void OnTestaction(InputValue btnValue)
    {
        Debug.Log("TEST" + btnValue.isPressed);
    }

    private void OnMouseRB(InputValue btnValue)
    {
        Debug.Log("rbbtn" + btnValue.isPressed);

        if (moving)
        {
            if (!btnValue.isPressed)
            {
                Debug.Log("rbbtn release");
                moving = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }

        if (btnValue.isPressed)
        {
            requestStartMoving = true;
        }
        else if (touchOnGo != null)
        {
            touchOnGo.SendMessage("OnClicked");
            touchOnGo = null;
        }

        Debug.Log("Camera.main.orthographic: " + Camera.main.orthographic);
        Debug.Log("移动模式状态: " + moving);
    }

    private void OnJump(InputValue movementValue)
    {
        float vec = movementValue.Get<float>();
        movementZ = vec;
    }

    private void FixedUpdate()
    {
        this.updateMarkers();
        // this.updateTextMarkers();

        if (!Camera.main.orthographic && !isMoving)
        {
            transform.Translate(movementX * sensitivetyKeyBoard,
                               movementZ * sensitivetyKeyBoard,
                               movementY * sensitivetyKeyBoard);

            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(transform.position.y, minHeight, maxHeight),
                transform.position.z);
        }

        if (requestStartMoving)
        {
            requestStartMoving = false;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            touchOnGo = this.testGameObject();
            if (touchOnGo != null)
            {
                totalMoveDistance = 0;
                return;
            }

            if (!Camera.main.orthographic)
            {
                moving = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void updateMarkers()
    {
        bool playing = director.state == PlayState.Playing;
        int c = MarkerList.transform.childCount;
        bool isOrthographic = Camera.main.orthographic;

        for (int i = 0; i < c; i++)
        {
            var marker = MarkerList.transform.GetChild(i);
            var mag = (transform.position - marker.position).magnitude;

            marker.gameObject.SetActive(!playing && mag > 50);

            if (isOrthographic && marker.position.y < 100)
            {
                marker.position = new Vector3(marker.position.x, marker.position.y + 200, marker.position.z);
            }
            if (!isOrthographic && marker.position.y > 100)
            {
                marker.position = new Vector3(marker.position.x, marker.position.y - 200, marker.position.z);
            }
        }
    }

    void updateTextMarkers()
    {
        bool playing = director.state == PlayState.Playing;
        bool isOrthographic = Camera.main.orthographic;
        int c = TextMarkerList.transform.childCount;

        for (int i = 0; i < c; i++)
        {
            var marker = TextMarkerList.transform.GetChild(i);
            marker.gameObject.SetActive(isOrthographic);
        }
    }

    GameObject testGameObject()
    {
        var pos = Input.touches.Length > 0 ? Input.touches[0].position : (Vector2)Input.mousePosition;
        var customRay = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(customRay, out var objHit, 10000f))
        {
            return objHit.collider.gameObject;
        }
        return null;
    }

    // ────────────────────────────────────────────────
    // 新增方法：直接设置摄像机的世界位置和欧拉角度
    // ────────────────────────────────────────────────
    public void SetCameraPositionAndRotation(Vector3 position, Vector3 eulerAngles)
    {
        // 直接设置位置
        transform.position = position;

        // 限制高度（保持与原有逻辑一致）
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, minHeight, maxHeight),
            transform.position.z);

        // 设置旋转（欧拉角，顺序通常是 X-Y-Z）
        transform.eulerAngles = eulerAngles;

        // 可选：如果你希望同时退出自由移动/鼠标锁定状态
        // moving = false;
        // requestStartMoving = false;
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    // 更友好的重载版本：使用 Quaternion
    public void SetCameraPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, minHeight, maxHeight),
            transform.position.z);

        transform.rotation = rotation;
    }
}