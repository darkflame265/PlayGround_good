using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public GameObject GameDirector;

    public GameObject particleEffectPrefab; // 파티클 이펙트 프리팹 (Inspector 창에서 연결)
    public string playerTag = "Player"; // 플레이어의 태그
    public bool destroyItem = true; // 아이템 획득 후 파괴 여부

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameDirector gameDirector = FindObjectOfType<GameDirector>();
            Debug.Log("GOAL!!!!");
            gameDirector.StopTime();
            gameDirector.Goal();

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
}
