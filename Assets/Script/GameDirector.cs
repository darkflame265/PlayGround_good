using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.
using System.Collections; // 코루틴 사용을 위해 필요
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 타이머 값을 표시할 TextMeshPro Text (UI) 오브젝트
    public TextMeshProUGUI upgradeText; // 타이머 값을 표시할 TextMeshPro Text (UI) 오브젝트
    public TextMeshProUGUI goalText;

    private float currentTime = 0f; // 현재 시간 (초 단위)
    private bool isTimerRunning = true; // 타이머가 현재 실행 중인지 여부를 나타내는 변수

    [Header("게임 종료 UI 설정")]
    public GameObject settingUI; // 게임 종료 확인 UI 패널 (Inspector에서 연결)

    private bool issettingUIActive = false; // 종료 UI 활성화 상태 확인

    void Start()
    {
        // ... (기존 Start 로직) ...
        // 게임 시작 시 종료 UI는 비활성화 상태로 시작합니다.
        if (settingUI != null)
        {
            settingUI.SetActive(false);
        }
    }

    void Update()
    {
        // isTimerRunning이 true일 때만 시간을 증가시키고 텍스트를 업데이트합니다.
        if (isTimerRunning)
        {
            // 매 프레임마다 시간을 증가시킵니다.
            currentTime += Time.deltaTime;

            // 시간을 분:초 형식으로 포맷합니다.
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime % 60F);

            // TextMeshPro 텍스트를 업데이트합니다.
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        // else 블록은 필요 없습니다. isTimerRunning이 false이면 Update 함수 내부의 if 블록이 실행되지 않아
        // currentTime이 더 이상 증가하지 않고, 마지막으로 업데이트된 timerText.text 값이 그대로 유지됩니다.

        // ... (기존 Update 로직) ...
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingUI != null)
            {
                if (issettingUIActive)
                {
                    // 종료 UI가 활성화 상태일 때 ESC 누르면 숨김
                    HideSettingUI();
                }
                else
                {
                    // 종료 UI가 비활성화 상태일 때 ESC 누르면 표시
                    ShowsettingUI();
                }
            }
        }

    }

    // 게임 종료 확인 UI를 표시하는 함수
    public void ShowsettingUI()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(true);
            issettingUIActive = true;
            // 필요하다면 게임 일시 정지 로직 추가 (Time.timeScale = 0f;)
            Time.timeScale = 0f; // 게임 시간 정지

            //마우스 커서 보이게.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // 게임 종료 확인 UI를 숨기는 함수
    public void HideSettingUI()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(false);
            issettingUIActive = false;
            // 필요하다면 게임 일시 정지 해제 로직 추가 (Time.timeScale = 1f;)
            Time.timeScale = 1f; // 게임 시간 다시 시작

            //마우스 보이지 않게.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // 점프력 강화 알림 메시지를 표시하는 함수
    public void NoticeUpgrade(string msg)
    {
        // 코루틴을 시작하여 텍스트를 표시하고 잠시 후 숨깁니다.
        StartCoroutine(ShowAndHideText(msg));
    }
    

    // 텍스트를 표시하고 잠시 후 숨기는 코루틴
    private IEnumerator ShowAndHideText(string msg)
    {
        // 텍스트 설정
        upgradeText.text = msg;
        // Text 오브젝트 활성화 (필요한 경우)
        upgradeText.gameObject.SetActive(true);

        // 지정된 시간(noticeDuration) 동안 대기
        yield return new WaitForSeconds(5f);

        // 텍스트 초기화 (빈 문자열)
        upgradeText.text = "";
        // Text 오브젝트 비활성화 (필요한 경우)
        upgradeText.gameObject.SetActive(false);
    }

    public void NoticeSpeedUpgrade()
    {
        // 코루틴을 시작하여 텍스트를 표시하고 잠시 후 숨깁니다.
        StartCoroutine(ShowAndHideText2());
    }

    // 텍스트를 표시하고 잠시 후 숨기는 코루틴
    private IEnumerator ShowAndHideText2()
    {
        // 텍스트 설정
        upgradeText.text = "[Upgrade Speed]\n 1.2 -> 5.0";
        // Text 오브젝트 활성화 (필요한 경우)
        upgradeText.gameObject.SetActive(true);

        // 지정된 시간(noticeDuration) 동안 대기
        yield return new WaitForSeconds(5f);

        // 텍스트 초기화 (빈 문자열)
        upgradeText.text = "";
        // Text 오브젝트 비활성화 (필요한 경우)
        upgradeText.gameObject.SetActive(false);
    }

    public void Goal()
    {
        goalText.text = "CONGRATURATION!\n YOU ARE WINNER!!";
        // Text 오브젝트 활성화 (필요한 경우)
        goalText.gameObject.SetActive(true);
    }

    // 타이머를 리셋하는 함수 (필요하다면 사용)
    public void ResetTimer()
    {
        currentTime = 0f;
        isTimerRunning = true; // 타이머 리셋 시 다시 실행 상태로 설정 (원하는 로직에 따라 변경 가능)
        // 리셋 후 바로 정지 상태로 유지하고 싶다면 isTimerRunning = false; 로 설정합니다.
    }

    // 타이머를 정지시키는 함수
    public void StopTime()
    {
        isTimerRunning = false; // 타이머 실행 상태를 false로 변경합니다.
        Debug.Log("타이머가 정지되었습니다. 현재 시간: " + timerText.text);
    }

    // (선택 사항) 정지된 타이머를 다시 시작하는 함수를 추가할 수도 있습니다.
    public void StartTimer()
    {
        isTimerRunning = true; // 타이머 실행 상태를 true로 변경합니다.
        Debug.Log("타이머가 다시 시작되었습니다.");
    }
}
