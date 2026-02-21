using UnityEngine;
using TMPro;

public class MoneyBall : MonoBehaviour
{
    public TextMeshProUGUI valueText;

    [Header("Value")]
    public int value = 20;              // can be positive or negative
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
    
    void Start()
    {
        Randomize();

        revealed = false;
        revealTimer = 0f;

        if (valueText) valueText.text = hiddenText; // hide first
    }

    public void RefreshLabel()
    {
        if (!valueText) return;

        if (isBankruptcy) valueText.text = "BANKRUPT";
        else if (isMoneyBag) valueText.text = "💰";
        else valueText.text = (value >= 0 ? "+" : "") + value.ToString();
    }
    void Randomize()
    {
        // Pick a value from a discrete set (bucketed outcomes)
        int[] possibleValues = { -50, -20, -10, 10, 20, 30, 50 };
        value = possibleValues[Random.Range(0, possibleValues.Length)];

        // Randomly choose a color INDEPENDENT of value (misleading on purpose)
        bool makeGreen = Random.value < 0.5f;

        if (spriteRenderer)
            spriteRenderer.color = makeGreen ? greenColor : redColor;

        // Make text opposite color of ball
        if (valueText)
        {
            valueText.color = makeGreen ? redColor : greenColor;
        }

        // (for now) no special types
        isBankruptcy = false;
        isMoneyBag = false;
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