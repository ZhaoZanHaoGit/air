using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class systemControl : MonoBehaviour
{
    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    // 确保在 WebGL 运行环境下开启触摸支持
    if (!EnhancedTouchSupport.enabled)
    {
        EnhancedTouchSupport.Enable();
    }
#endif
    }
}
