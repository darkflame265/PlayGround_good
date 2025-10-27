using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 전환할 씬 이름
    [SerializeField] private string sceneName = "MainScene";

    // UI 버튼에서 OnClick 이벤트로 연결
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    // 게임 종료용 함수 (원하면 Exit 버튼에도 사용 가능)
    public void QuitGame()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 플레이 모드 종료
#else
        Application.Quit(); // 실제 빌드에서는 게임 종료
#endif
    }
}
