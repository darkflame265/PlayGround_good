using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using Cinemachine;

public class GeneralSettings : MonoBehaviour
{
    [Header("UI")]
    public Slider sensitivitySlider;
    public Text sensitivityValueText;

    [Header("Player reference (drag the GameObject)")]
    public GameObject player;  // ← 플레이어 GameObject만 드래그하면 됨

    [Header("(Optional) Cinemachine fallback")]
    public CinemachineVirtualCamera vcam;
    public CinemachineFreeLook freeLook;
    [Range(0.1f, 1.0f)] public float freeLookYScale = 0.7f;

    [Header("Range")]
    public float minSensitivity = 50f;
    public float maxSensitivity = 300f;

    const string KEY_SENS = "opt_look_sensitivity";

    // 캐시
    Component _tpcComponent;
    FieldInfo _fiLook;
    PropertyInfo _piLook;

    void Start()
    {
        // 플레이어에서 TPC 컴포넌트 자동 탐색 (타입/네임스페이스 몰라도 작동)
        ResolveTPC(player);

        // 슬라이더 초기화
        if (sensitivitySlider)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;

            float saved = PlayerPrefs.GetFloat(KEY_SENS, Mathf.Clamp(ReadCurrentSensitivity(), minSensitivity, maxSensitivity));
            sensitivitySlider.value = saved;
            sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);

            if (sensitivityValueText) sensitivityValueText.text = Mathf.RoundToInt(saved).ToString();
            ApplySensitivity(saved, save:false);
        }
    }

    void OnSliderChanged(float val)
    {
        ApplySensitivity(val, save:true);
        if (sensitivityValueText) sensitivityValueText.text = Mathf.RoundToInt(val).ToString();
    }

    // --- 핵심: TPC 자동 해석 ---
    void ResolveTPC(GameObject go)
    {
        _tpcComponent = null;
        _fiLook = null; _piLook = null;

        if (go == null) return;

        // 1) 우선 모든 컴포넌트에서 "LookSensitivity" 필드/프로퍼티를 가진 놈을 찾는다.
        foreach (var comp in go.GetComponentsInChildren<Component>(true))
        {
            if (comp == null) continue;
            var t = comp.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var fi = t.GetField("LookSensitivity", flags);
            var pi = (fi == null) ? t.GetProperty("LookSensitivity", flags) : null;

            if ((fi != null && fi.FieldType == typeof(float)) ||
                (pi != null && pi.PropertyType == typeof(float) && (pi.CanRead || pi.CanWrite)))
            {
                _tpcComponent = comp;
                _fiLook = fi;
                _piLook = pi;
                Debug.Log($"[GeneralSettings] TPC resolved to {t.FullName} (LookSensitivity)");
                return;
            }
        }

        // 2) 실패 시: Cinemachine 경로로 폴백
        if (vcam == null) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (freeLook == null) freeLook = FindObjectOfType<CinemachineFreeLook>();
        Debug.Log("[GeneralSettings] No LookSensitivity field found on player. Will use Cinemachine fallback if available.");
    }

    void ApplySensitivity(float s, bool save)
    {
        bool applied = false;

        // A) TPC 경로: LookSensitivity에 넣기
        if (_tpcComponent != null)
        {
            if (_fiLook != null) { _fiLook.SetValue(_tpcComponent, s); applied = true; }
            else if (_piLook != null && _piLook.CanWrite) { _piLook.SetValue(_tpcComponent, s); applied = true; }

            Debug.Log($"[Sensitivity] Set {s} via {_tpcComponent.GetType().Name}.LookSensitivity");
        }

        // B) 폴백: Cinemachine 회전형이면 Axis Speed 조절
        if (!applied)
        {
            var pov = vcam ? vcam.GetComponent<CinemachinePOV>() : null;
            if (pov != null)
            {
                pov.m_HorizontalAxis.m_MaxSpeed = s;
                pov.m_VerticalAxis.m_MaxSpeed   = s;
                applied = true;
                Debug.Log($"[Sensitivity] Set {s} via CinemachinePOV MaxSpeed");
            }
            else if (freeLook != null)
            {
                freeLook.m_XAxis.m_MaxSpeed = s;
                freeLook.m_YAxis.m_MaxSpeed = s * freeLookYScale;
                applied = true;
                Debug.Log($"[Sensitivity] Set {s} via FreeLook Axis MaxSpeed");
            }
        }

        if (save) { PlayerPrefs.SetFloat(KEY_SENS, s); PlayerPrefs.Save(); }
    }

    float ReadCurrentSensitivity()
    {
        if (_tpcComponent != null)
        {
            if (_fiLook != null) return (float)_fiLook.GetValue(_tpcComponent);
            if (_piLook != null && _piLook.CanRead) return (float)_piLook.GetValue(_tpcComponent, null);
        }

        var pov = vcam ? vcam.GetComponent<CinemachinePOV>() : null;
        if (pov != null) return pov.m_HorizontalAxis.m_MaxSpeed;
        if (freeLook != null) return freeLook.m_XAxis.m_MaxSpeed;

        return 120f;
    }
}
