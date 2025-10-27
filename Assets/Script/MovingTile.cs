using UnityEngine;
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.

public class MovingPlatform : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(-298.3f, -30f, -71f); // 발판이 이동할 목표 위치
    public float moveSpeed = 1.0f; // 발판 이동 속도
    public float returnDelay = 5.0f; // 플레이어 감지 해제 후 원래 위치로 돌아가기까지의 지연 시간
    public string playerTag = "Player"; // 플레이어 오브젝트의 태그

    private Vector3 startPosition; // 발판의 원래 위치
    private bool isMovingToTarget = false; // 목표 위치로 이동 중인지 여부
    private bool isMovingToStart = false; // 원래 위치로 이동 중인지 여부
    private int playerCount = 0; // 발판 위에 올라와 있는 플레이어 수
    private Coroutine returnCoroutine; // 원래 위치로 돌아가는 코루틴 참조

    void Start()
    {
        // 스크립트 시작 시 발판의 현재 위치를 원래 위치로 저장합니다.
        startPosition = transform.position;
    }

    void Update()
    {
        // 발판 이동 로직 (Update 함수에서 부드럽게 이동)
        if (isMovingToTarget)
        {
            // 현재 위치에서 목표 위치까지 moveSpeed로 이동합니다.
            // Transform.position을 직접 변경하여 이동합니다.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 목표 위치에 거의 도달했으면 이동 중 상태를 해제합니다.
            if (Vector3.Distance(transform.position, targetPosition) < 10f)
            {
                isMovingToTarget = false;
                Debug.Log("발판 목표 위치 도달.");
            }
        }
        else if (isMovingToStart)
        {
            // 현재 위치에서 원래 위치까지 moveSpeed로 이동합니다.
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);

            // 원래 위치에 거의 도달했으면 이동 중 상태를 해제합니다.
            if (Vector3.Distance(transform.position, startPosition) < 0.001f)
            {
                isMovingToStart = false;
                Debug.Log("발판 원래 위치 도달.");
            }
        }
    }

    // 플레이어가 트리거 영역에 진입했을 때 호출됩니다. (Is Trigger가 체크된 콜라이더에 스크립트가 붙어있어야 함)
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 설정된 playerTag와 일치하는지 확인합니다.
        if (other.gameObject.CompareTag(playerTag))
        {
            // *** 수정된 부분: 플레이어 오브젝트를 발판 오브젝트의 자식으로 설정합니다. ***
            // 이렇게 하면 발판이 움직일 때 플레이어도 함께 움직입니다.
            other.transform.SetParent(this.transform); // 'this.transform'은 이 스크립트가 붙어있는 발판의 Transform입니다.
            Debug.Log(other.gameObject.name + "이 발판 트리거에 진입했습니다. 발판의 자식으로 설정합니다.");

            playerCount++; // 발판 위에 올라온 플레이어 수 증가

            // 플레이어가 감지되면, 원래 위치로 돌아가는 코루틴이 실행 중이라면 중지합니다.
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null; // 코루틴 참조 초기화
                Debug.Log("플레이어 감지됨, 원래 위치로 돌아가는 코루틴 중지.");
            }

            // 플레이어가 올라탔고 (playerCount > 0), 발판이 아직 목표 위치로 이동 중이 아니라면 이동을 시작합니다.
            // transform.position != targetPosition 조건은 이미 목표 위치에 도달해 있다면 다시 이동을 시작하지 않게 합니다.
            if (!isMovingToTarget && transform.position != targetPosition)
            {
                isMovingToTarget = true; // 목표 위치로 이동 시작
                isMovingToStart = false; // 혹시 원래 위치로 이동 중이었다면 중지
                Debug.Log("플레이어 감지, 발판이 목표 위치로 이동 시작.");
            }
        }
    }

    // 플레이어가 트리거 영역에서 벗어났을 때 호출됩니다.
    private void OnTriggerExit(Collider other)
    {
        // 충돌한 오브젝트의 태그가 설정된 playerTag와 일치하는지 확인합니다.
        if (other.gameObject.CompareTag(playerTag))
        {
            // *** 수정된 부분: 플레이어 오브젝트의 부모를 해제합니다. ***
            // null로 설정하면 플레이어는 다시 월드 좌표계를 기준으로 독립적으로 움직이게 됩니다.
            other.transform.SetParent(null);
            Debug.Log(other.gameObject.name + "이 발판 트리거에서 벗어났습니다. 부모 관계를 해제합니다.");

            playerCount--; // 발판 위 플레이어 수 감소
            Debug.Log("플레이어 감지 해제. 현재 발판 위 플레이어 수: " + playerCount);

            // 발판 위에 플레이어가 아무도 없게 되면, 일정 시간 후 원래 위치로 돌아가는 코루틴을 시작합니다.
            if (playerCount <= 0)
            {
                playerCount = 0; // 플레이어 수가 음수가 되지 않도록 최소 0으로 유지
                // 이미 코루틴이 실행 중이면 새로 시작하지 않도록 체크합니다.
                if (returnCoroutine == null)
                {
                    returnCoroutine = StartCoroutine(ReturnToStartPositionAfterDelay(returnDelay));
                    Debug.Log(returnDelay + "초 후 원래 위치로 돌아가는 코루틴 시작.");
                }
            }
        }
    }

    // 일정 시간 대기 후 원래 위치로 돌아가는 코루틴
    IEnumerator ReturnToStartPositionAfterDelay(float delay)
    {
        Debug.Log("원래 위치 복귀 대기 시작 (" + delay + " 초)");
        yield return new WaitForSeconds(delay);

        // 기다린 후에 플레이어 수가 여전히 0 이하라면 (즉, 다시 플레이어가 올라타지 않았다면)
        // 원래 위치로 이동을 시작합니다.
        if (playerCount <= 0)
        {
            Debug.Log("지연 시간 경과, 발판이 원래 위치로 돌아갑니다.");
            isMovingToStart = true; // 원래 위치로 이동 시작
            isMovingToTarget = false; // 목표 위치로 이동 중지
        }
        returnCoroutine = null; // 코루틴 완료 후 참조 초기화 (다음 트리거 감지를 위해 중요)
    }

    // 2D 게임의 경우 OnTriggerEnter2D와 OnTriggerExit2D를 사용합니다.
    // Parenting 로직은 2D에서도 transform.SetParent 동일하게 적용됩니다.
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
         if (other.gameObject.CompareTag(playerTag))
        {
            other.transform.SetParent(this.transform);
             Debug.Log(other.gameObject.name + "이 발판 트리거에 진입했습니다. 발판의 자식으로 설정합니다.");

            playerCount++;

            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
                 Debug.Log("플레이어 감지됨, 원래 위치로 돌아가는 코루틴 중지.");
            }

            if (!isMovingToTarget && transform.position != targetPosition)
            {
                isMovingToTarget = true;
                isMovingToStart = false;
                 Debug.Log("플레이어 감지, 발판이 목표 위치로 이동 시작.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
             other.transform.SetParent(null);
              Debug.Log(other.gameObject.name + "이 발판 트리거에서 벗어났습니다. 부모 관계를 해제합니다.");

            playerCount--;
             Debug.Log("플레이어 감지 해제. 현재 발판 위 플레이어 수: " + playerCount);

            if (playerCount <= 0)
            {
                 playerCount = 0;
                 if (returnCoroutine == null)
                {
                    returnCoroutine = StartCoroutine(ReturnToStartPositionAfterDelay(returnDelay));
                     Debug.Log(returnDelay + "초 후 원래 위치로 돌아가는 코루틴 시작.");
                }
            }
        }
    }
    */
}
