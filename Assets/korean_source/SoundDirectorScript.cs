// SoundDirectorScript.cs (새로운 스크립트 파일)

using UnityEngine;

public class SoundDirectorScript : MonoBehaviour
{
    public AudioClip goalSoundClip; // Inspector에서 재생할 MP3 파일을 연결해주세요.
    private AudioSource audioSource;

    void Awake() // Start보다 더 빨리 호출되어 초기화에 적합합니다.
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("SoundDirector 오브젝트에 AudioSource 컴포넌트가 없습니다! 소리 재생을 위해 추가해주세요.", this);
            // 없으면 강제로 추가하거나, 기능을 비활성화할 수도 있습니다.
            // audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Play On Awake를 껐을 것이므로 기본적으로 아무 소리도 재생되지 않습니다.
    }

    // 외부에서 호출하여 골인 소리를 재생하는 메서드
    public void PlayGoalSound()
    {
        if (audioSource != null && goalSoundClip != null)
        {
            // 한 번만 소리를 재생합니다.
            audioSource.PlayOneShot(goalSoundClip);
            Debug.Log("골인 소리 재생!");
        }
        else
        {
            if (audioSource == null)
                Debug.LogWarning("SoundDirector의 AudioSource가 설정되지 않았습니다.");
            if (goalSoundClip == null)
                Debug.LogWarning("SoundDirector의 goalSoundClip이 설정되지 않았습니다.");
        }
    }
}