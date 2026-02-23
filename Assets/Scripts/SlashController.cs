using UnityEngine;

public class SlashController : MonoBehaviour
{
    public GameObject slashPrefab;
    public GameObject sparksPrefab;
    [Header("Swipe")]
    public float minSwipeDistance = 0.5f; // world units
    public float slashDuration = 0.08f;

    [Header("Slash size")]
    public float slashLength = 3f;
    public float slashThickness = 0.18f;
    Vector3 startWorld;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startWorld = GetMouseWorld();
            AudioManager.Instance?.PlaySwipe();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 endWorld = GetMouseWorld();
            Vector3 delta = endWorld - startWorld;
            delta.z = 0f;

            if (delta.magnitude < minSwipeDistance) return;

            Vector2 snappedDir = SnapTo8Directions(delta.normalized);
            SpawnSlash(snappedDir, (startWorld + endWorld) * 0.5f);
        }
    }

    Vector3 GetMouseWorld()
    {
        Vector3 w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        w.z = 0f;
        return w;
    }

    Vector2 SnapTo8Directions(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 8 directions = 45-degree buckets
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void SpawnSlash(Vector2 dir, Vector3 center)
    {
        GameObject slash = Instantiate(slashPrefab, center, Quaternion.identity);
        if (sparksPrefab)
        {
            GameObject fx = Instantiate(sparksPrefab, center, Quaternion.identity);
            // Destroy(fx, 0.5f);
        }
        // rotate to face direction
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        slash.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        // scale to desired size
        slash.transform.localScale = new Vector3(slashLength, slashThickness, 1f);

        Destroy(slash, slashDuration);
    }
}