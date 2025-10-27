using UnityEngine;
using UnityEngine.UI;

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
    

    void Awake()
    {
        if (monitorButton) monitorButton.onClick.AddListener(ShowMonitor);
        if (soundButton)   soundButton.onClick.AddListener(ShowSound);
        if (generalButton) generalButton.onClick.AddListener(ShowGeneral);
        if (exitButton)    exitButton.onClick.AddListener(ShowExit);
        if (bookButton)    bookButton.onClick.AddListener(ShowBook); // ✅ 새로 추가

        // 초기 탭 설정 (원하면 다른 탭으로 바꿔도 됨)
        ShowMonitor();
    }

    public void ShowMonitor()
    {
        SetActivePanel(monitorPanel, true);
        SetActivePanel(soundPanel,   false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel,    false);
        SetActivePanel(bookPanel,    false);
    }

    public void ShowSound()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel,   true);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel,    false);
        SetActivePanel(bookPanel,    false);
    }

    public void ShowGeneral()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel,   false);
        SetActivePanel(generalPanel, true);
        SetActivePanel(exitPanel,    false);
        SetActivePanel(bookPanel,    false);
    }

    public void ShowExit()
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel,   false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel,    true);
        SetActivePanel(bookPanel,    false);
    }

    public void ShowBook() // ✅ 새로 추가
    {
        SetActivePanel(monitorPanel, false);
        SetActivePanel(soundPanel,   false);
        SetActivePanel(generalPanel, false);
        SetActivePanel(exitPanel,    false);
        SetActivePanel(bookPanel,    true);
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
}
