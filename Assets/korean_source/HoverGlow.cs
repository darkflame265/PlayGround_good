using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class HoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Targets")]
    public Image targetImage;        // 아이콘 이미지 (비우면 자동 할당)
    public Outline optionalOutline;  // 선택: 후광(Outline) 연출용

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor  = new Color(1f, 0.95f, 0.7f, 1f);  // 은은한 노랑빛
    public Color pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    [Header("Scale")]
    public float hoverScale = 1.06f;
    public float pressedScale = 0.98f;
    public float animTime = 0.12f;

    Vector3 baseScale;
    Coroutine animCo;

    void Reset()
    {
        targetImage = GetComponent<Image>();
        optionalOutline = GetComponent<Outline>(); // 있으면 자동 참조
    }

    void Awake()
    {
        if (!targetImage) targetImage = GetComponent<Image>();
        baseScale = transform.localScale;

        // 초기 상태
        ApplyInstant(normalColor, baseScale);
        if (optionalOutline)
        {
            optionalOutline.effectColor = new Color(1f, 0.95f, 0.7f, 0f); // 투명 시작
            optionalOutline.effectDistance = new Vector2(2f, -2f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LerpTo(hoverColor, baseScale * hoverScale, outlineAlpha: 0.55f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LerpTo(normalColor, baseScale, outlineAlpha: 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LerpTo(pressedColor, baseScale * pressedScale, outlineAlpha: 0.35f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 커서가 여전히 위에 있으면 hover로, 아니면 normal로
        bool hovering = RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)transform, Input.mousePosition, eventData.enterEventCamera);
        if (hovering) OnPointerEnter(eventData); else OnPointerExit(eventData);
    }

    void LerpTo(Color col, Vector3 scale, float outlineAlpha)
    {
        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(LerpRoutine(col, scale, outlineAlpha));
    }

    IEnumerator LerpRoutine(Color col, Vector3 scale, float outlineAlpha)
    {
        Color c0 = targetImage.color;
        Vector3 s0 = transform.localScale;
        float t = 0f;
        float startA = optionalOutline ? optionalOutline.effectColor.a : 0f;

        while (t < animTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / animTime);
            targetImage.color = Color.Lerp(c0, col, k);
            transform.localScale = Vector3.Lerp(s0, scale, k);

            if (optionalOutline)
            {
                var oc = optionalOutline.effectColor;
                oc.a = Mathf.Lerp(startA, outlineAlpha, k);
                optionalOutline.effectColor = oc;
            }
            yield return null;
        }
        ApplyInstant(col, scale, outlineAlpha);
    }

    void ApplyInstant(Color col, Vector3 scale, float outlineAlpha = -1f)
    {
        targetImage.color = col;
        transform.localScale = scale;
        if (optionalOutline && outlineAlpha >= 0f)
        {
            var oc = optionalOutline.effectColor;
            oc.a = outlineAlpha;
            optionalOutline.effectColor = oc;
        }
    }
}
