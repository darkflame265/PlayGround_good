using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public GameObject GameDirector;

    public GameObject particleEffectPrefab; // 파티클 이펙트 프리팹 (Inspector 창에서 연결)
    public string playerTag = "Player"; // 플레이어의 태그
    public bool destroyItem = true; // 아이템 획득 후 파괴 여부

    // ★ 추가: SoundDirectorScript에 대한 참조
    // Inspector에서 SoundDirector 오브젝트를 여기에 드래그하여 연결해주세요.
    public SoundDirectorScript soundDirectorRef; 

    // Start is called before the first frame update
    void Start()
    {
        // 만약 Inspector에서 soundDirectorRef를 연결하는 것을 잊었을 경우
        // 씬에서 GameDirector처럼 FindObjectOfType으로 찾아올 수도 있습니다.
        if (soundDirectorRef == null)
        {
            soundDirectorRef = FindObjectOfType<SoundDirectorScript>();
            if (soundDirectorRef == null)
            {
                Debug.LogError("씬에 SoundDirectorScript를 가진 오브젝트를 찾을 수 없습니다! 수동으로 연결하거나 생성해주세요.", this);
            }
        }
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

            // ★ 추가: SoundDirector를 통해 골인 소리 재생
            if (soundDirectorRef != null)
            {
                soundDirectorRef.PlayGoalSound();
            }
            else
            {
                Debug.LogWarning("SoundDirector가 연결되지 않아 골인 소리를 재생할 수 없습니다.");
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
                    ps.Play(); 
                    Destroy(effect, ps.main.duration); 
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