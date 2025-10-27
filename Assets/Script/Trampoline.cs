using StarterAssets;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceMultiplier = 1.5f; // 기존 점프 높이에 곱해줄 배율 (또는 직접 지정)
    public float forwardSpeedBoost = 5f; // 앞으로 추가될 속도
    public string playerTag = "Player"; // 플레이어의 태그

    // OnCollisionEnter 대신 Character Controller의 충돌 감지 함수 사용
    // 이 스크립트가 트램펄린 오브젝트에 붙어 있고, 트램펄린은 일반 콜라이더(Is Trigger 해제)여야 합니다.
    // 플레이어 오브젝트에 Character Controller가 있어야 이 이벤트가 발생합니다.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 충돌한 오브젝트가 플레이어 태그를 가지고 있는지 확인
        if (hit.gameObject.CompareTag(playerTag))
        {
            // 플레이어 오브젝트에서 Third Person Controller 스크립트 가져오기
            ThirdPersonController playerController = hit.gameObject.GetComponent<ThirdPersonController>();

            // 스크립트가 있다면
            if (playerController != null)
            {
                Debug.Log("플레이어 Character Controller 감지!");

                // *** 점프 기능 구현 (플레이어 스크립트 구조에 따라 달라짐) ***
                // 예시 1: 플레이어 스크립트에 public Jump() 함수가 있다면
                // playerController.Jump();

                // 예시 2: 플레이어 스크립트에 점프 힘 변수나 속도 변수를 직접 수정해야 한다면
                // playerController.JumpHeight *= bounceMultiplier; // 점프 높이 배율 증가 (일시적 변경 후 되돌리는 로직 필요)
                // 또는 플레이어의 현재 속도에 위쪽 및 앞쪽 속도를 더해주는 방식
                Vector3 bounceVelocity = Vector3.up * playerController.JumpHeight * bounceMultiplier; // 기존 점프 높이 기반 계산 예시
                Vector3 forwardVelocity = playerController.transform.forward * forwardSpeedBoost;

                // 플레이어 컨트롤러의 이동 속도나 방향을 직접 제어하는 로직 필요
                // Character Controller는 Rigidbody처럼 AddForce를 사용하지 않습니다.
                // Character Controller의 Move() 함수나 속도 관련 변수를 수정해야 합니다.
                // 이것은 사용하시는 ThirdPersonController 스크립트의 구현 방식에 따라 크게 달라집니다.

                // 예를 들어, 플레이어 컨트롤러 스크립트 내부에
                // public void ApplyExternalForce(Vector3 force) { ... } 와 같은 함수가 있다면 호출
                // playerController.ApplyExternalForce(Vector3.up * bounceStrength + playerController.transform.forward * forwardForce);

                // 또는 Character Controller 스크립트 내부에
                // private Vector3 _verticalVelocity;
                // private float _speed;
                // 와 같은 변수가 있다면 public으로 만들거나 접근 가능한 함수를 통해 수정
                // playerController._verticalVelocity.y = Mathf.Sqrt(bounceHeight * -2f * Gravity); // 점프 공식 사용
                // playerController._speed += forwardSpeedBoost; // 앞으로 속도 증가

                // 이 부분은 사용 중인 Third Person Controller 스크립트를 직접 확인하고 수정해야 합니다.
                // 가장 간단한 방법은 플레이어 스크립트에 public void TrampolineJump(float bounce, float forward) 와 같은 함수를 만들고
                // 트램펄린 스크립트에서 이 함수를 호출하는 것입니다.
                // 예: playerController.TrampolineJump(bounceStrength, forwardForce);

                // *** 임시 방편 예시 (Character Controller의 Move 함수에 직접 영향을 주는 것은 복잡) ***
                // Character Controller는 Rigidbody와 달리 외부에서 속도를 직접 더하기 어렵습니다.
                // Character Controller의 Move() 함수는 주로 사용자의 입력에 기반한 이동을 처리하므로,
                // 외부 힘(트램펄린)에 의한 순간적인 속도 변화를 적용하려면 플레이어 컨트롤러 스크립트 자체의 수정이 필수적입니다.
            } else
            {
                Debug.Log("sss");
            }
        }
    }
}
