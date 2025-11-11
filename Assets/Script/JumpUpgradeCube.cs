using UnityEngine;
using StarterAssets; // ThirdPersonController가 Starter Assets에 있다면 필요

public class JumpUpgradeCube : MonoBehaviour
{
    public float jumpBoostAmount = 2f; // 점프 높이 증가량
    public float duration = 5f; // 점프력 강화 지속 시간
    public string playerTag = "Player"; // 플레이어의 태그
    private float originalJumpHeight; // 원래 점프 높이
    private bool isJumpBoosted = false; // 점프력 강화 상태 여부

    public GameObject gameDirector; // GameDirector 오브젝트를 연결할 변수

    public GameObject particleEffectPrefab; // 파티클 이펙트 프리팹 (Inspector 창에서 연결)
    public bool destroyItem = true; // 아이템 획득 후 파괴 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            // ThirdPersonController 컴포넌트 가져오기
            ThirdPersonController controller = other.GetComponent<ThirdPersonController>();

            // GameDirector 오브젝트가 연결되었는지 확인
            if (gameDirector != null)
            {
                GameDirector gd = gameDirector.GetComponent<GameDirector>(); // GameDirector 컴포넌트 가져오기

                // ThirdPersonController 컴포넌트가 있고, 아직 점프력 강화가 되지 않았는지 확인
                if (controller != null && !isJumpBoosted)
                {
                    // 원래 점프 높이 저장
                    originalJumpHeight = controller.JumpHeight;

                    // 점프력 강화
                    controller.JumpHeight += jumpBoostAmount;
                    isJumpBoosted = true;

                    // GameDirector의 NoticeUpgrade 함수 호출
                    gd.NoticeUpgrade("JUMP POWER UP : "  + jumpBoostAmount);

                    Debug.Log("점프력 강화! 증가량: " + jumpBoostAmount + ", 지속 시간: " + duration + "초");

                    // 지정된 시간 후에 점프력 복구
                    Invoke("RestoreJumpHeight", duration);
                }
            }
            else
            {
                Debug.LogError("UpgradeCube: GameDirector 오브젝트가 연결되지 않았습니다!");
            }

            // 파티클 이펙트 프리팹이 지정되었는지 확인
            if (particleEffectPrefab != null)
            {
                // 파티클 이펙트 생성
                GameObject effect = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);

                // 파티클 시스템 컴포넌트 가져오기
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();

                // 파티클 시스템이 있다면 실행
                if (ps != null)
                {
                    ps.Play(); // 또는 ps.Emit(amount);
                    // 파티클 시스템이 끝난 후 파괴 (선택 사항)
                    Destroy(effect, ps.main.duration); // 파티클 시스템의 duration 시간 후 파괴
                }
                else
                {
                    Debug.LogWarning("파티클 이펙트 프리팹에 ParticleSystem 컴포넌트가 없습니다!");
                }
            }
            else
            {
                Debug.LogWarning("파티클 이펙트 프리팹이 지정되지 않았습니다!");
            }

            // 아이템 획득 후 아이템 오브젝트 파괴 (선택 사항)
            if (destroyItem)
            {
                Destroy(gameObject);
            }
        }
    }

    // 점프력을 원래대로 복구하는 함수
    private void RestoreJumpHeight()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                // 점프력 복구
                controller.JumpHeight = originalJumpHeight;
                isJumpBoosted = false;

                Debug.Log("점프력 복구!");
            }
        }
    }
}
