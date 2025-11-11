using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FloatingLoot : MonoBehaviour
{
    [Header("Floating")]
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1.5f;

    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

    [Header("Glow")]
    public Color glowColor = Color.yellow; // 빛 색깔
    public float glowIntensityMin = 0.5f;
    public float glowIntensityMax = 2.0f;
    public float glowPulseSpeed = 2.0f;

    private Vector3 _startPos;
    private Material _mat;
    private static readonly string _emissColorProp = "_EmissionColor";

    void Start()
    {
        _startPos = transform.position;

        // 머티리얼 캐시
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            // 공유 머티리얼 건들면 모든 아이템이 같이 변하니까
            // 개별 인스턴스를 복제해서 씀
            _mat = r.material;
            _mat.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        // 1) 위아래 둥실둥실
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = _startPos + new Vector3(0f, offsetY, 0f);

        // 2) 회전
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);

        // 3) 글로우 (Emission Pulse)
        if (_mat != null)
        {
            // 0~1 사이로 왔다갔다 하는 값
            float ping = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f;
            // 최소~최대 사이로 보간
            float intensity = Mathf.Lerp(glowIntensityMin, glowIntensityMax, ping);

            Color finalEmission = glowColor * intensity; // 색 * 세기
            _mat.SetColor(_emissColorProp, finalEmission);
        }
    }
}
