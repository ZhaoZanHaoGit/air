using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch; // 消除歧义
namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Universal Input Definition", menuName = "Grid/Universal Input Definition")]
    public class UniversalInputDefinition : GridInputDefinition
    {
        private void OnEnable() // ScriptableObject 建议用 OnEnable 开启支持
        {
#if ENABLE_INPUT_SYSTEM
            if (!UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled)
            {
                UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
            }
#endif
        }

        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM
            // 1. 优先检测触摸（针对手机 WebGL）
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
            }

            // 2. 如果没触摸，检测鼠标（针对 PC WebGL）
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }

            return null;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            // 旧版输入系统逻辑
            if (Input.touchCount > 0) return Input.GetTouch(0).position;
            return Input.mousePosition;
#endif
        }

        public override bool ShouldInteract()
        {
#if ENABLE_INPUT_SYSTEM
            // 手机端：有触摸即视为交互
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0) return true;

            // PC端：按下鼠标左键视为交互
            if (UnityEngine.InputSystem.Mouse.current != null)
                return UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;

            return false;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetMouseButton(0) || Input.touchCount > 0;
#endif
        }

        // 确保你的蓝色配置文件类里包含这个方法
        public float GetZoomDelta()
        {
#if ENABLE_INPUT_SYSTEM
            // --- 1. 手机端：双指缩放 ---
            if (ETouch.EnhancedTouchSupport.enabled && ETouch.Touch.activeTouches.Count == 2)
            {
                var touch0 = ETouch.Touch.activeTouches[0];
                var touch1 = ETouch.Touch.activeTouches[1];

                // 计算当前帧两指间距
                float currentDist = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
                // 计算上一帧两指间距
                float prevDist = Vector2.Distance(
                    touch0.screenPosition - touch0.delta,
                    touch1.screenPosition - touch1.delta);

                // 返回间距差值（放大为正，缩小为负）
                return (currentDist - prevDist) * 0.01f; // 乘以系数调优灵敏度
            }

            // --- 2. PC端：鼠标滚轮 ---
            if (Mouse.current != null)
            {
                float scroll = Mouse.current.scroll.ReadValue().y;
                return scroll * 0.001f; // 归一化处理
            }
#endif
            return 0;
        }

        public Vector2 GetZoomScreenPivot()
        {
#if ENABLE_INPUT_SYSTEM
            // 1. 手机端：返回双指的中点
            if (UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled &&
                UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 2)
            {
                var t0 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
                var t1 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[1].screenPosition;
                return (t0 + t1) * 0.5f;
            }
            // 2. PC端：返回当前鼠标位置
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (UnityEngine.Input.touchCount == 2)
            {
                return (UnityEngine.Input.GetTouch(0).position + UnityEngine.Input.GetTouch(1).position) * 0.5f;
            }
            return UnityEngine.Input.mousePosition;
#endif
            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }
    }
}
