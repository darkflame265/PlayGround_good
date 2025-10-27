// 2025-10-22 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("점프 설정")]
    public float jumpForce = 8f;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.3f;
    public float groundRayStartOffset = 0.5f; // ✅ Ray 시작점 높이 오프셋
    public float coyoteTime = 0.15f;

    private Rigidbody rb;
    private Animator animator;
    private float lastGroundedTime = 0f;
    private bool isGrounded = false;

    private float targetYRotation = 450f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 없습니다. Animator를 추가해주세요.");
        }
    }

    void Update()
    {
        // ✅ 레이 시작점을 약간 위로 올려서 계산
        Vector3 rayStart = transform.position + Vector3.up * groundRayStartOffset;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = false;
        }

        //Debug.Log("iSgrounded: " + isGrounded);
        // 점프 (Coyote Time 적용)
        if (Input.GetButtonDown("Jump") && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Jump 애니메이션 트리거
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        // 방향키 및 WASD 입력
        bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        // 회전 방향 결정 (대각선 포함)
        if (up && right)
            targetYRotation = 405f;
        else if (up && left)
            targetYRotation = 315f;
        else if (down && right)
            targetYRotation = 495f;
        else if (down && left)
            targetYRotation = 585f;
        else if (up)
            targetYRotation = 360f;
        else if (down)
            targetYRotation = 540f;
        else if (right)
            targetYRotation = 450f;
        else if (left)
            targetYRotation = 270f;

        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);

        // 이동 애니메이션 설정
        if (animator != null)
        {
            bool isMoving = up || down || left || right;
            animator.SetBool("IsRunning", isMoving);
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;

        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // ✅ Gizmo로 실제 레이 시작점과 거리 시각화
        Vector3 rayStart = transform.position + Vector3.up * groundRayStartOffset;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * groundCheckDistance);
    }
}