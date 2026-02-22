using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyBall : MonoBehaviour
{
    public TextMeshProUGUI valueText;

    [Header("Value")]
    public float percent = 20f;             // can be positive or negative
    public bool isBankruptcy = false;   // special loss
    public bool isMoneyBag = false;     // special bonus

    //random color
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    public Color greenColor = new Color(0.2f, 0.9f, 0.2f);
    public Color redColor   = new Color(0.95f, 0.2f, 0.2f);
    
    [Header("Reveal")]
    public float revealDelay = 0.45f;
    public string hiddenText = "???";

    float revealTimer = 0f;
    bool revealed = false;
    bool sliced = false;
    
    //slicing
    public GameObject sliceVFX; // optional
    
    void Start()
    {
        Randomize();

        revealed = false;
        revealTimer = 0f;

        if (valueText) valueText.text = hiddenText; // hide first
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Blade"))
        {
            HandleSlice();
        }
    }
    
    void HandleSlice()
    {
        if (sliced) return;
        sliced = true;

        if (isBankruptcy)
        {
            AudioManager.Instance?.PlayBankrupt();
            GameManager.Instance.GameOver(true);
        }
        else if (isMoneyBag)
        {
            AudioManager.Instance?.PlayBonus();
            GameManager.Instance.AddFlatCash(50f);
        }
        else
        {
            if (percent >= 0) AudioManager.Instance?.PlayGain();
            else AudioManager.Instance?.PlayLoss();

            GameManager.Instance.ApplyPercent(percent);
        }

        StartCoroutine(SliceAndDestroy());
    }
    IEnumerator SliceAndDestroy()
    {
        // optional VFX pop
        if (sliceVFX) Instantiate(sliceVFX, transform.position, Quaternion.identity);

        float t = 0f;
        Vector3 start = transform.localScale;

        while (t < 0.08f)
        {
            t += Time.deltaTime;
            float k = 1f - (t / 0.08f);
            transform.localScale = start * k;
            yield return null;
        }

        Destroy(gameObject);
    }
    public void RefreshLabel()
    {
        if (!valueText) return;

        if (isBankruptcy) valueText.text = "CRASH";
        else if (isMoneyBag) valueText.text = "BONUS";
        else valueText.text = (percent >= 0 ? "+" : "") + percent.ToString("0") + "%";
    }
    void Randomize()
    {
        // Pick a value from a discrete set (bucketed outcomes)
        float[] possiblePercents = { -50f, -20f, -10f, 10f, 20f, 30f, 50f };
        percent = possiblePercents[Random.Range(0, possiblePercents.Length)];
        if (isBankruptcy)
        {
            spriteRenderer.color = Color.black;
            valueText.color = Color.white;
        }
        else if (isMoneyBag)
        {
            spriteRenderer.color = Color.yellow;
            valueText.color = Color.black;
        }
        // Randomly choose a color INDEPENDENT of value (misleading on purpose)
        bool makeGreen = Random.value < 0.5f;

        if (spriteRenderer)
            spriteRenderer.color = makeGreen ? greenColor : redColor;

        // Make text opposite color of ball
        if (valueText)
        {
            valueText.color = makeGreen ? redColor : greenColor;
        }
        // Reset to normal size first
        Vector3 baseScale = transform.localScale;
        if (isBankruptcy || isMoneyBag)
        {
            transform.localScale = baseScale * 1.8f;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
    
    void Update()
    {
        if (revealed) return;

        revealTimer += Time.deltaTime;
        if (revealTimer >= revealDelay)
        {
            revealed = true;
            RefreshLabel(); // reveal real value/special text
        }
    }
}