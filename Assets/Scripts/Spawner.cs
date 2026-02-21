using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject moneyBallPrefab;

    [Header("Spawn")]
    public float spawnInterval = 1.0f;
    public float spawnXRange = 7f;

    [Header("Throw")]
    public float minUpForce = 8f;
    public float maxUpForce = 12f;
    public float minSideForce = -2f;
    public float maxSideForce = 2f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        float x = Random.Range(-spawnXRange, spawnXRange);
        Vector3 pos = new Vector3(x, transform.position.y, 0f);

        GameObject obj = Instantiate(moneyBallPrefab, pos, Quaternion.identity);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb)
        {
            float up = Random.Range(minUpForce, maxUpForce);
            float side = Random.Range(minSideForce, maxSideForce);
            rb.AddForce(new Vector2(side, up), ForceMode2D.Impulse);
        }
    }
}