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

    [Header("Difficulty")]
    public float difficultyRampSpeed = 0.02f;
    public float minSpawnInterval = 0.3f;
    
    float timer;

    [Header("Special Spawn Chances")]
    [Range(0f, 1f)] public float moneyBagChance = 0.08f;
    [Range(0f, 1f)] public float bankruptcyChance = 0.05f;
    
    void Update()
    {
        spawnInterval -= difficultyRampSpeed * Time.deltaTime;
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval);
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
        
        MoneyBall mb = obj.GetComponent<MoneyBall>();
        
        if (mb)
        {
            float r = Random.value;

            if (r < bankruptcyChance)
            {
                mb.isBankruptcy = true;
                mb.isMoneyBag = false;
            }
            else if (r < bankruptcyChance + moneyBagChance)
            {
                mb.isMoneyBag = true;
                mb.isBankruptcy = false;
            }
            else
            {
                mb.isMoneyBag = false;
                mb.isBankruptcy = false;
            }

            mb.RefreshLabel();
        }
        
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb)
        {
            float up = Random.Range(minUpForce, maxUpForce);
            float side = Random.Range(minSideForce, maxSideForce);
            rb.AddForce(new Vector2(side, up), ForceMode2D.Impulse);
        }
    }
    
}