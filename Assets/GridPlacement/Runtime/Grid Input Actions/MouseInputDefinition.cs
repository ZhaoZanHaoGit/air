using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch; // 确保引用了这个
#endif

namespace Hypertonic.GridPlacement.GridInput
{
    [CreateAssetMenu(fileName = "Mouse Input Definition", menuName = "Grid/Mouse Input Definition")]
    public class MouseInputDefinition : GridInputDefinition
    {
        public override Vector3? InputPosition()
        {
#if ENABLE_INPUT_SYSTEM
            // 1. 尝试获取鼠标位置
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
                if (mousePos.sqrMagnitude > 0) return mousePos;
            }

            // 2. 明确使用新版增强触摸的 Touch 类型（消除歧义）
            if (UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled &&
                UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                // 显式指定使用 UnityEngine.InputSystem.EnhancedTouch.Touch
                return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
            }

            return null;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            // 旧版输入系统使用的是 UnityEngine.Touch，这里也要注意
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
#endif
        }

        public override bool ShouldInteract()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.leftButton.isPressed;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.Mouse0);
#endif
        }
    }
}