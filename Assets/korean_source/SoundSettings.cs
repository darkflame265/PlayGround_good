using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [Header("UI")]
    public Slider bgmSlider;
    public Text bgmValueText; // 선택: 0~100 숫자 표시용

    [Header("References")]
    public AudioManager audioManager; // GameDirector에 있는 AudioManager

    const string KEY_BGM_VOLUME = "opt_bgm_volume";

    void Start()
    {
        // AudioManager 자동 검색 (못찾으면 로그로 안내)
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        // ★★★ 변경: 저장된 볼륨 불러오기 (없으면 0.0)
        float saved = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 0f); // 1f를 0f로 변경

        // 슬라이더 초기화 (0~100 UI → 0.0~1.0 내부)
        if (bgmSlider)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 100f;
            bgmSlider.value = saved * 100f;
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        }

        ApplyVolume(saved);

        if (bgmValueText)
            bgmValueText.text = Mathf.RoundToInt(bgmSlider.value).ToString();
    }

    void OnBgmSliderChanged(float val)
    {
        float volume01 = val / 100f;
        ApplyVolume(volume01);
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, volume01);
        PlayerPrefs.Save();

        if (bgmValueText)
            bgmValueText.text = Mathf.RoundToInt(val).ToString();
    }

    void ApplyVolume(float volume)
    {
        if (audioManager != null)
        {
            var src = audioManager.GetComponent<AudioSource>();
            if (src != null)
                src.volume = volume;
        }
    }
}