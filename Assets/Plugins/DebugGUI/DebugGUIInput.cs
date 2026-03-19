using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

// Centralizes all input handling so the rest of the code stays clean.
// Swap Active Input Handling in Project Settings without touching any other file.
static class DebugGUIInput
{
    /// <summary>Screen-space mouse position. Y is NOT flipped — callers flip as needed.</summary>
    public static Vector2 MousePosition
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            return Input.mousePosition;
#endif
        }
    }

    public static bool MiddleMouseButtonDown
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current?.middleButton.wasPressedThisFrame ?? false;
#else
            return Input.GetMouseButtonDown(2);
#endif
        }
    }

    public static bool MiddleMouseButtonUp
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current?.middleButton.wasReleasedThisFrame ?? false;
#else
            return Input.GetMouseButtonUp(2);
#endif
        }
    }

    public static bool LeftMouseButtonPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current?.leftButton.isPressed ?? false;
#else
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
