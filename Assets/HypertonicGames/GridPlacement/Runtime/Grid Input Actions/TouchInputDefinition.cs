using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch; // 消除歧义
namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Touch Input Definition", menuName = "Grid/Touch Input Definition")]
    public class TouchInputDefinition : GridInputDefinition
    {
        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
#endif
        }

        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM

            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count <= 0)
            {
                return null;
            }

            return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
 if (Input.touchCount <= 0)
            {
                return null;
            }

            UnityEngine.Touch touch = Input.GetTouch(0);

            return touch.position;
#endif
        }

        public override bool ShouldInteract()
        {
            return true;
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

    }



}