using UnityEngine;

public static class AutoFullscreen
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void ForceBorderlessAtStartup()
    {
        // 모니터 네이티브 해상도 읽기
        int w = Display.main.systemWidth;
        int h = Display.main.systemHeight;
        int hz = Screen.currentResolution.refreshRate > 0 ? Screen.currentResolution.refreshRate : 60;

        // 보더리스 전체화면
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(w, h, true, hz);

        // (선택) 과거에 저장된 창모드/해상도 값 초기화하고 싶다면:
        // PlayerPrefs.DeleteKey("Screenmanager Resolution Width");
        // PlayerPrefs.DeleteKey("Screenmanager Resolution Height");
        // PlayerPrefs.DeleteKey("Screenmanager Is Fullscreen mode");
        // PlayerPrefs.Save();

        Debug.Log($"[AutoFullscreen] {w}x{h}@{hz} FullScreenWindow 적용");
    }
}
