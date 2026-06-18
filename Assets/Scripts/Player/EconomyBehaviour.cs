using TMPro;
using UnityEngine;

public class EconomyBehaviour : MonoBehaviour
{
    private float moneyOfPlayer;

    [SerializeField]private float initialMoney;

    [SerializeField] private TextMeshProUGUI textMoney;

    public float _moneyOfThePlayer => moneyOfPlayer;

    private void Awake()
    {
        initialMoney = 100;

        moneyOfPlayer = initialMoney;
        UpdateUI();
    }

    public void IncreaseMoneyForClient (float amount, float happinesBar)
    {
        moneyOfPlayer += amount * GettingMoneyToPay(happinesBar);
        UpdateUI();
    }

    public void IncreaseMoney(float amount)
    {
        moneyOfPlayer += amount;
        UpdateUI();
    }

    public void DecreaseMoney(float amount)
    {
        moneyOfPlayer -= amount;
        UpdateUI();
    }

    public void IncreaseInitialMoney(float amount)
    {

    }

    public void DecreaseInitialMoney (float amount)
    {

    }

    public void UpdateUI()
    {
        textMoney.text = "$" + moneyOfPlayer.ToString();
    }

    private float GettingMoneyToPay(float happiness) //Devuelve el multiplicador de paga según la felicidad que tenia el cliente
    {
        if (happiness > 0.94)
        {
            return 2.5f;
        }
        else if (happiness > 0.79f && happiness < 0.94f)
        {
            return 2f;
        }
        else if (happiness > 0.49f && happiness < 0.79f)
        {
            return 1.5f;
        }
        else if (happiness > 0.19f && happiness < 0.49f)
        {
            return 1f;
        }
        else if (happiness > 0.09f && happiness < 0.19f)
        {
            return 0.5f;
        }
        else if (happiness > 0.01f && happiness < 0.09f)
        {
            return 0.2f;
        }

        return 0f;
    }
}
