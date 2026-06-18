using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwinkleBehaviour : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Image image;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float time;
    [SerializeField] private int amount;

    private float currentTime;
    private int currentAmount;

    private Transform positionToGet;

    private bool canTwinkle = false;
    private bool isTwinkle = false;

    [SerializeField] private bool fromEnable = false;

    public Transform _positionToGet { get { return positionToGet; } set { positionToGet = value; } }

    private void Awake()
    {
        if (GetComponent<Image>() != null) { image = GetComponent<Image>(); }
        if (GetComponent<TextMeshProUGUI>() != null) { text = GetComponent<TextMeshProUGUI>(); }
        if (GetComponent<SpriteRenderer>() != null) { spriteRenderer = GetComponent<SpriteRenderer>(); }
    }

    private void OnEnable()
    {
        if (fromEnable == true)
        {
            StartInit();
        }
    }

    public void StartInit()
    {
        if (positionToGet != null) { transform.position = new Vector3(positionToGet.position.x, transform.position.y, positionToGet.position.z); }
        canTwinkle = false;
        currentAmount = 0;
        currentTime = time;

        isTwinkle = true;
    }



    private void Update()
    {
        if (isTwinkle == true)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= time)
            {
                Twinkle(canTwinkle);
                canTwinkle = !canTwinkle;
                currentAmount++;
                currentTime = 0;

                if (currentAmount >= amount)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void Twinkle(bool transparent)
    {
        if (transparent == true)
        {
            if (GetComponent<Image>() != null) { image.color = new Color(1, 1, 1, 0); }
            if (GetComponent<TextMeshProUGUI>() != null) { text.color = new Color(1, 1, 1, 0); }
            if (GetComponent<SpriteRenderer>() != null) { spriteRenderer.color = new Color(1, 1, 1, 0); }
        }
        else 
        {
            if (GetComponent<Image>() != null) { image.color = new Color(1, 1, 1, 1); }
            if (GetComponent<TextMeshProUGUI>() != null) { text.color = new Color(1, 1, 1, 1); }
            if (GetComponent<SpriteRenderer>() != null) { spriteRenderer.color = new Color(1, 1, 1, 1); }
        }
    }

    public void StopTwinkle()
    {
        isTwinkle = false;

        if (GetComponent<Image>() != null) { image.color = new Color(1, 1, 1, 1); }
        if (GetComponent<TextMeshProUGUI>() != null) { text.color = new Color(1, 1, 1, 1); }
        if (GetComponent<SpriteRenderer>() != null) { spriteRenderer.color = new Color(1, 1, 1, 1); }
    }
}
