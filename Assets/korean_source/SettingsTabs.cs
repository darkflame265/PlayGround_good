using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsTabs : MonoBehaviour
{
    [Header("Panels")]
    public GameObject monitorPanel;
    public GameObject soundPanel;
    public GameObject generalPanel;
    public GameObject bookPanel; // ✅ 새로 추가
    public GameObject exitPanel;


    [Header("Tab Buttons")]
    public Button monitorButton;
    public Button soundButton;
    public Button generalButton;
    public Button bookButton; // ✅ 새로 추가
    public Button exitButton;

    [Header("Actions")]
    public Button restartButton; // ✅ 재시작 버튼
    public Button lobbyButton;   // ✅ 새로 추가
    public Button gameExitButton;   // ✅ 게임 종료 버튼 (새로 추가)


    void Awake()
    {
        if (monitorButton) monitorButton.onClick.AddListener(ShowMonitor);
        if (soundButton) soundButton.onClick.AddListener(ShowSound);
        if (generalButton) generalButton.onClick.AddListener(ShowGeneral);
        if (exitButton) exitButton.onClick.AddListener(ShowExit);
        if (restartButton) restartButton.onClick.AddListener(RestartScene); // ✅ 클릭 연결
        if (bookButton) bookButton.onClick.AddListener(ShowBook); // ✅ 새로 추가
        if (gameExitButton)  gameExitButton.onClick.AddListener(QuitGame);
        if (lobbyButton) lobbyButton.onClick.AddListener(GoToLobby); // ✅ 클릭 이벤트 연결

        // 초기 탭 설정 (원하면 다른 탭으로 바꿔도 됨)
        ShowMonitor();
    }

    public void ShowMonitor()
    {
        SetActivePanel(monitorPanel, true);
        SetActivePanel(soundPanel, false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel, false);
        SetActivePanel(bookPanel, false);
    }

    public void ShowSound()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel, true);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel, false);
        SetActivePanel(bookPanel, false);
    }

    public void ShowGeneral()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel, false);
        SetActivePanel(generalPanel, true);
        SetActivePanel(exitPanel, false);
        SetActivePanel(bookPanel, false);
    }

    public void ShowExit()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel, false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel, true);
        SetActivePanel(bookPanel, false);
    }

    public void ShowBook() // ✅ 새로 추가
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel, false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel, false);
        SetActivePanel(bookPanel, true);
    }

    void SetActivePanel(GameObject panel, bool on)
    {
        if (!panel) return;

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = on ? 1f : 0f;
            cg.interactable = on;
            cg.blocksRaycasts = on;
        }

        panel.SetActive(on);
    }

    public void RestartScene()
    {
        // 정지 상태에서 누를 수도 있으니 원복
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        var active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex); // 또는 active.name
    }

    // ✅ 로비로 이동하는 함수 추가
    public void GoToLobby()
    {
        // (선택) 타임스케일 원복
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        Debug.Log("로비로 이동합니다!");
        SceneManager.LoadScene("LobbyScene");
    }

        // 게임을 종료하는 함수
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        // 유니티 에디터에서는 Application.Quit()가 작동하지 않습니다.
        // 에디터에서 테스트 시에는 Debug.Log로 확인합니다.
        // 빌드된 게임에서는 이 함수가 게임을 종료시킵니다.
        Application.Quit();

        // 에디터에서 게임 종료 테스트를 위한 코드 (빌드 시에는 주석 처리 또는 제거)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
