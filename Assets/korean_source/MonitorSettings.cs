using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Cinemachine;

public class MonitorSettings : MonoBehaviour
{
    [Header("UI")]
    public Toggle windowedToggle;      // “Windowed” 체크박스
    public Dropdown resolutionDropdown; // 해상도 드롭다운 (uGUI)
    public Dropdown displayModeDropdown; // (선택) 디스플레이 모드: Exclusive/Borderless/Windowed

    // 저장 키
    const string KEY_WINDOWED = "opt_windowed";
    const string KEY_RES_INDEX = "opt_res_index";
    const string KEY_MODE_INDEX = "opt_mode_index";

    Resolution[] _resolutions;

    // === FOV 설정 ===
    [Header("FOV (POV)")]
    public Slider fovSlider;                 // UI 슬라이더
    public Text fovValueText;                // (선택) 현재 값 표시
    public CinemachineVirtualCamera vcam;    // 조절 대상 vcam (없으면 자동 탐색)
    public float fovMin = 40f;
    public float fovMax = 90f;
    public float defaultFov = 60f;

    const string KEY_FOV = "opt_fov";

    void Start()
    {
        // 1) 해상도 목록 채우기 (중복 제거, 보통 가로x세로 기준)
        _resolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height, refreshRate = r.refreshRate })
            .Distinct()
            .OrderBy(r => r.width).ThenBy(r => r.height).ThenBy(r => r.refreshRate)
            .ToArray();

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            var options = _resolutions.Select(r => $"{r.width} x {r.height} @ {r.refreshRate}Hz").ToList();
            resolutionDropdown.AddOptions(options);

            // 저장된 인덱스 로드 (없으면 현재에 가장 가까운 해상도)
            int savedIndex = PlayerPrefs.GetInt(KEY_RES_INDEX, FindClosestResolutionIndex());
            savedIndex = Mathf.Clamp(savedIndex, 0, _resolutions.Length - 1);
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        // 2) 디스플레이 모드 드롭다운 (선택)
        if (displayModeDropdown)
        {
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new System.Collections.Generic.List<string>{
                "Exclusive Fullscreen", "Borderless Fullscreen", "Windowed"
            });
            int modeIdx = PlayerPrefs.GetInt(KEY_MODE_INDEX, 1); // 기본: Borderless
            displayModeDropdown.value = modeIdx;
            displayModeDropdown.RefreshShownValue();
            displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        }

        // 3) Windowed 토글
        if (windowedToggle)
        {
            bool isWindowed = PlayerPrefs.GetInt(KEY_WINDOWED, 0) == 1;
            windowedToggle.isOn = isWindowed;
            windowedToggle.onValueChanged.AddListener(OnWindowedToggle);
        }

        // vcam 없으면 자동으로 찾아보기 (첫 번째 활성 vcam)
        if (vcam == null)
            vcam = FindObjectOfType<CinemachineVirtualCamera>();

        // 저장된 값 가져오거나 기본값/현재값 사용
        float savedFov = PlayerPrefs.HasKey(KEY_FOV)
                        ? PlayerPrefs.GetFloat(KEY_FOV)
                        : (vcam != null ? vcam.m_Lens.FieldOfView : defaultFov);

        // 슬라이더 바인딩
        if (fovSlider != null)
        {
            fovSlider.minValue = fovMin;
            fovSlider.maxValue = fovMax;
            fovSlider.value = Mathf.Clamp(savedFov, fovMin, fovMax);
            fovSlider.onValueChanged.AddListener(OnFovChanged);
        }

        // 4) 최초 적용
        ApplyAll();

        InitFovFromPrefs();
    }

    void InitFovFromPrefs()
    {
        // vcam 자동 탐색(비어있으면)
        if (vcam == null) vcam = FindObjectOfType<CinemachineVirtualCamera>();

        // 저장값 로드(없으면 현재 FOV 또는 기본값)
        float saved = PlayerPrefs.HasKey(KEY_FOV)
                        ? PlayerPrefs.GetFloat(KEY_FOV)
                        : (vcam != null ? vcam.m_Lens.FieldOfView : defaultFov);

        // 슬라이더 초기화
        if (fovSlider)
        {
            fovSlider.minValue = fovMin;
            fovSlider.maxValue = fovMax;
            fovSlider.value = Mathf.Clamp(saved, fovMin, fovMax);
            // 중복 추가 방지 위해 먼저 리스너 제거 후 다시 등록
            fovSlider.onValueChanged.RemoveListener(OnFovChanged);
            fovSlider.onValueChanged.AddListener(OnFovChanged);
        }

        // 즉시 적용 + 표시 갱신
        ApplyFov(saved);
        if (fovValueText) fovValueText.text = Mathf.RoundToInt(fovSlider ? fovSlider.value : saved).ToString();
    }

    void OnFovChanged(float value)
    {
        ApplyFov(value);
        PlayerPrefs.SetFloat(KEY_FOV, value);
        PlayerPrefs.Save();
        if (fovValueText) fovValueText.text = Mathf.RoundToInt(value).ToString();
    }

    void ApplyFov(float value)
    {
        if (vcam != null)
        {
            var lens = vcam.m_Lens;
            lens.FieldOfView = value;
            vcam.m_Lens = lens; // 구조체라 재할당
        }
        // (선택) FreeLook도 같이 쓰면:
        // var free = FindObjectOfType<CinemachineFreeLook>();
        // if (free != null) { var l=free.m_Lens; l.FieldOfView=value; free.m_Lens=l; }

        if (fovValueText != null)
            fovValueText.text = Mathf.RoundToInt(value).ToString();
    }

    int FindClosestResolutionIndex()
    {
        var cur = Screen.currentResolution;
        int idx = 0;
        int bestScore = int.MaxValue;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            int dx = Mathf.Abs(_resolutions[i].width - cur.width);
            int dy = Mathf.Abs(_resolutions[i].height - cur.height);
            int dr = Mathf.Abs(_resolutions[i].refreshRate - cur.refreshRate);
            int score = dx + dy + dr;
            if (score < bestScore) { bestScore = score; idx = i; }
        }
        return idx;
    }

    void OnWindowedToggle(bool isOn)
    {
        PlayerPrefs.SetInt(KEY_WINDOWED, isOn ? 1 : 0);
        ApplyAll();
    }

    void OnResolutionChanged(int idx)
    {
        PlayerPrefs.SetInt(KEY_RES_INDEX, idx);
        ApplyAll();
    }

    void OnDisplayModeChanged(int idx)
    {
        PlayerPrefs.SetInt(KEY_MODE_INDEX, idx);
        ApplyAll();
    }

    void ApplyAll()
    {
        int resIdx = Mathf.Clamp(PlayerPrefs.GetInt(KEY_RES_INDEX, 0), 0, _resolutions.Length - 1);
        var r = _resolutions.Length > 0 ? _resolutions[resIdx] : Screen.currentResolution;

        // 디스플레이 모드 결정
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen; // 기본값
        int modeIdx = PlayerPrefs.GetInt(KEY_MODE_INDEX, 1);
        switch (modeIdx)
        {
            case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: mode = FullScreenMode.FullScreenWindow;    break; // Borderless
            case 2: mode = FullScreenMode.Windowed;            break;
        }

        // Windowed 토글이 On이면 강제로 Windowed
        bool isWindowed = PlayerPrefs.GetInt(KEY_WINDOWED, 0) == 1;
        if (isWindowed) mode = FullScreenMode.Windowed;

        // 적용
        Screen.fullScreenMode = mode;
        Screen.SetResolution(r.width, r.height, mode != FullScreenMode.Windowed, r.refreshRate);

        // (선택) 창모드일 때 창 크기/위치 더 손보고 싶으면 여기서 처리
        // - Editor에선 SetResolution가 Game 뷰에만 영향. Standalone 빌드에서 테스트 권장.
    }
}
