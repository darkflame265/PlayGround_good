using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnR : MonoBehaviour
{
    [Tooltip("재시작할 씬 이름 (기본: MainScene)")]
    public string sceneName = "MainScene";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    public void RestartScene()
    {
        // 일시정지 해제
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        // 커서 상태 복구(필요하면 주석 해제)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;

        // 씬 재로딩
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // 만약 sceneName이 비어있다면 현재 씬을 다시 불러오기
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
