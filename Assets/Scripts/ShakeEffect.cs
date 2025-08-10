using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShakeEffect : MonoBehaviour
{
    private SpriteRenderer m_Renderer;

    public void PlayAt(float amplitude, float duration, Vector2 worldPoint, Color color)
    {
        transform.position = worldPoint;
        m_Renderer.color = color;

        StartCoroutine(EffectExecutor(amplitude, duration));
    }

    void Awake()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator EffectExecutor(float amplitude, float duration)
    {
        float t = duration;
        Vector2 startScale = new Vector2();
        Vector2 endScale = new Vector2(amplitude, amplitude);
        Color startColor = m_Renderer.color;
        Color endColor = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            0.0f
        );

        while (t > 0)
        {
            t -= Time.deltaTime;
            float progress = Ease.OutCubic(1 - (t / duration));
            transform.localScale = Vector2.LerpUnclamped(startScale, endScale, progress);
            m_Renderer.color = Color.LerpUnclamped(startColor, endColor, progress);
            yield return null;
        }

        Destroy(gameObject);
    }

    public static implicit operator bool(ShakeEffect instance) => instance != null;
}
