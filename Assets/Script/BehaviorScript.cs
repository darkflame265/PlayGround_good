using UnityEngine;

public class BehaviorScript : MonoBehaviour
{
    public float moveDistance = 2f; // 좌우 이동 범위 (현재 위치 기준)
    public float moveSpeed = 1f; // 이동 속도
    public bool moveOnXAxis = true; // X축 이동 여부
    public bool moveOnZAxis = false; // Z축 이동 여부
    private Vector3 startPosition; // 시작 위치

    void Start()
    {
        // 스크립트 시작 시 오브젝트의 현재 위치를 시작 위치로 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. 이동 방향 결정
        // Time.time을 사용하여 시간에 따라 부드러운 사인파 형태의 값 생성
        // moveDistance를 곱하여 이동 범위를 조절
        float xOffset = 0f;
        float zOffset = 0f;

        if (moveOnXAxis)
        {
            xOffset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        }
        if (moveOnZAxis)
        {
            zOffset = Mathf.Cos(Time.time * moveSpeed) * moveDistance; // Cos 사용으로 X축과 위상 차이를 줌
        }

        // 2. 새로운 위치 계산
        // 시작 위치에서 xOffset과 zOffset을 더하여 새로운 위치 계산
        Vector3 newPosition = startPosition + new Vector3(xOffset, 0, zOffset);

        // 3. 오브젝트 위치 변경
        // 계산된 새로운 위치로 오브젝트 이동
        transform.position = newPosition;
    }
}
