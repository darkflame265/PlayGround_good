// ResolutionBootstrapper.cs
using UnityEngine;

public class ResolutionBootstrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    // static void ForceNativeFullscreenBeforeSplash()
    // {
    //     // 1) 네이티브(물리) 해상도 읽기
    //     int w = Display.main.systemWidth;
    //     int h = Display.main.systemHeight;
    //     int hz = Screen.currentResolution.refreshRate > 0 ? Screen.currentResolution.refreshRate : 60;

    //     // 2) 보더리스 전체화면으로 강제
    //     Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    //     Screen.SetResolution(w, h, true, hz);

    //     // 3) 로그로 검증
    //     Debug.unityLogger.logEnabled = true;
    //     Debug.Log($"[ResolutionBootstrapper] Applied {w}x{h}@{hz} (FullScreenWindow)");

    //     // (선택) FHD로 저장된 이전 값 초기화하고 싶다면:
    //     // PlayerPrefs.DeleteKey("opt_res_index"); PlayerPrefs.Save();
    // }
    static void ForceNativeFullscreenBeforeSplash()
    {
        int w = Display.main.systemWidth;
        int h = Display.main.systemHeight;
        int hz = Screen.currentResolution.refreshRate > 0 ? Screen.currentResolution.refreshRate : 60;

        // 여기를 ExclusiveFullScreen으로 변경해 보세요!
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(w, h, true, hz); // 세 번째 인자가 true인 한 FullScreenMode 값에 따라 동작합니다.

        Debug.unityLogger.logEnabled = true;
        Debug.Log($"[ResolutionBootstrapper] Applied {w}x{h}@{hz} (ExclusiveFullScreen)");
    }

}
