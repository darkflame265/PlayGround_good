// CursorManager.cs
using UnityEngine;

[DefaultExecutionOrder(-1000)] // 아주 이른 순서
public class CursorManager : MonoBehaviour
{
    public static CursorManager I { get; private set; }

    [Tooltip("게임 시작 시 자동으로 커서를 잠그고 숨깁니다.")]
    public bool lockOnStart = true;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        if (lockOnStart) Lock();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Alt+Tab 후 포커스 복귀 시 다시 잠가줌
        if (hasFocus && lockOnStart)
            Lock();
    }

    public static void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
