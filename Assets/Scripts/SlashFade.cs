using UnityEngine;

public class SlashFade : MonoBehaviour
{
    public float lifetime = 0.08f;

    SpriteRenderer sr;
    float t;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = 1f - Mathf.Clamp01(t / lifetime);

        if (sr)
        {
            Color c = sr.color;
            c.a = k;          // fade alpha to 0
            sr.color = c;
        }
    }
}