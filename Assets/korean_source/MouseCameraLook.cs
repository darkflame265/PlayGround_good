using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MouseCameraLook : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("The follow target the Cinemachine Virtual Camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private StarterAssets.StarterAssetsInputs _input;

    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }

    private void Start()
    {
        if (CinemachineCameraTarget == null)
        {
            Debug.LogError("CinemachineCameraTarget is not assigned!");
            enabled = false;
            return;
        }

        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#endif
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // 입력이 있고, 카메라 락이 안 돼있을 때만 실행
        if (_input != null && _input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // 위아래 각도 제한
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 실제 회전 적용
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw,
            0.0f
        );
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
