using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

// Centralizes all input handling so the rest of the code stays clean.
// Swap Active Input Handling in Project Settings without touching any other file.
// Touch support: single finger maps to left mouse button; two-finger tap maps to middle mouse (window drag).
static class DebugGUIInput
{
    /// <summary>Screen-space pointer position (mouse or primary touch). Y is NOT flipped — callers flip as needed.</summary>
    public static Vector2 MousePosition
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var touch = Touchscreen.current?.touches[0];
            if (touch != null && touch.press.isPressed)
                return touch.position.ReadValue();
            return Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            return Input.mousePosition;
#endif
        }
    }

    /// <summary>True on the frame a drag begins: middle mouse button pressed, or a second finger touched down.</summary>
    public static bool MiddleMouseButtonDown
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current?.touches[1].press.wasPressedThisFrame ?? false)
                return true;
            return Mouse.current?.middleButton.wasPressedThisFrame ?? false;
#else
            if (Input.touchCount >= 2 && Input.GetTouch(1).phase == TouchPhase.Began)
                return true;
            return Input.GetMouseButtonDown(2);
#endif
        }
    }

    /// <summary>True on the frame a drag ends: middle mouse button released, or the second finger lifted.</summary>
    public static bool MiddleMouseButtonUp
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current?.touches[1].press.wasReleasedThisFrame ?? false)
                return true;
            return Mouse.current?.middleButton.wasReleasedThisFrame ?? false;
#else
            if (Input.touchCount >= 2 && Input.GetTouch(1).phase == TouchPhase.Ended)
                return true;
            return Input.GetMouseButtonUp(2);
#endif
        }
    }

    /// <summary>True while left mouse button or primary touch is held.</summary>
    public static bool LeftMouseButtonPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current?.touches[0].press.isPressed ?? false)
                return true;
            return Mouse.current?.leftButton.isPressed ?? false;
#else
            if (Input.touchCount > 0)
            {
                var phase = Input.GetTouch(0).phase;
                return phase == TouchPhase.Began || phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            }
            return Input.GetMouseButton(0);
#endif
        }
    }

    /// <summary>
    /// Returns true if the given key was pressed this frame.
    /// Note: KeyCode name must match the new Input System key name (works for most standard keys).
    /// </summary>
    public static bool GetKeyDown(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        return (Keyboard.current?[key.ToString()] as KeyControl)?.wasPressedThisFrame ?? false;
#else
        return Input.GetKeyDown(key);
#endif
    }
}
