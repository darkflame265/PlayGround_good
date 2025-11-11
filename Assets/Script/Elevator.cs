using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요


public class Elevator : MonoBehaviour
{
    public GameObject targetObject; // 움직일 대상 오브젝트 (Inspector 창에서 연결)
    public float moveSpeed = 5f; // 오브젝트 이동 속도
    public string playerTag = "Player"; // 플레이어의 태그
    public float moveDistance = 600f; // 이동할 최대 거리
    public float returnDelay = 10f; // 원래 위치로 돌아가기 전 대기 시간
    private Vector3 initialPosition; // 시작 위치
    private bool isMovingUp = false; // 위로 움직이는 중인지 확인
    private bool isMovingDown = false; // 아래로 움직이는 중인지 확인

    private Transform currentPlatformUser = null;

    void Start()
    {
        // 시작 시 초기 위치를 저장
        if (targetObject != null)
        {
            initialPosition = targetObject.transform.position;
        }
        else
        {
            Debug.LogError("움직일 오브젝트가 지정되지 않았습니다!");
        }
    }

    void Update()
    {
        // 위로 움직이는 중이라면 오브젝트를 위로 이동
        if (isMovingUp && targetObject != null)
        {
            // 현재 위치에서 위쪽 방향으로 moveSpeed 속도로 이동
            targetObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 이동 거리가 moveDistance를 초과하면 움직임 멈춤
            if (Vector3.Distance(initialPosition, targetObject.transform.position) > moveDistance)
            {
                isMovingUp = false; // 멈춤
                Debug.Log("목표 거리에 도달하여 멈춥니다. " + returnDelay + "초 후 원래 위치로 돌아갑니다.");
                StartCoroutine(ReturnToInitialPositionAfterDelay(returnDelay)); // 코루틴 시작
            }
        }

        // 아래로 움직이는 중이라면 오브젝트를 아래로 이동
        if (isMovingDown && targetObject != null)
        {
            // 현재 위치에서 아래쪽 방향으로 moveSpeed 속도로 이동
            targetObject.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

            // 이동 거리가 0보다 작아지면 움직임 멈춤
            if (Vector3.Distance(initialPosition, targetObject.transform.position) < 0.1f)
            {
                isMovingDown = false; // 멈춤
                targetObject.transform.position = initialPosition; // 정확한 초기 위치 설정
                Debug.Log("원래 위치로 돌아왔습니다.");
            }
        }
    }

    // 충돌 감지 시 호출되는 함수 (트리거 콜라이더 사용)
    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 오브젝트의 태그가 playerTag와 같은지 확인
        if (other.gameObject.CompareTag(playerTag))
        {
            // 움직일 오브젝트가 지정되었는지 확인
            if (targetObject != null && !isMovingUp && !isMovingDown)
            {
                isMovingUp = true; // 움직임 시작
                Debug.Log("오브젝트 움직임 시작!");
            }
        }
    }

    // 딜레이 후 원래 위치로 이동하는 코루틴
    IEnumerator ReturnToInitialPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // delay 초 동안 대기
        isMovingDown = true; // 아래로 움직임 시작
        isMovingUp = false; // 위로 움직임 중단
        Debug.Log("원래 위치로 돌아갑니다.");
    }
}