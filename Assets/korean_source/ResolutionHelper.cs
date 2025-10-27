using UnityEngine;
using System.Linq;

public static class ResolutionHelper
{
    public static void ApplyBestDefault(bool force = false)
    {
        // 첫 실행에만 적용하고 싶다면 PlayerPrefs로 체크
        if (!force && PlayerPrefs.GetInt("res_inited", 0) == 1) return;

        var native = Screen.currentResolution; // 모니터 네이티브
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // 보더리스
        Screen.SetResolution(native.width, native.height, true, native.refreshRate);

        PlayerPrefs.SetInt("res_inited", 1);
    }

    public static Resolution[] GetSupportedResolutionsDistinct()
    {
        return Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height, refreshRate = r.refreshRate })
            .Distinct()
            .OrderBy(r => r.width).ThenBy(r => r.height).ThenBy(r => r.refreshRate)
            .ToArray();
    }

    public static void ApplyResolution(Resolution r, FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
        bool fullscreen = mode != FullScreenMode.Windowed;
        Screen.SetResolution(r.width, r.height, fullscreen, r.refreshRate);
    }
}
