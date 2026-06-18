using UnityEngine;
using UnityEngine.UI;

public class OvenBehaviour : MonoBehaviour
{
    [SerializeField] private Image ovenImage;

    [SerializeField] private float timeToComplete = 20;

    private float progressTime;
    private GameObject resultFood;
    [SerializeField] private Transform positionToSpawn;

    private bool isOven;

    [SerializeField] private TypeOfFoods[] foodsAllowed;
    [SerializeField] private float[] timesOfOvensForFoods;
    [SerializeField] private GameObject[] resultFoods; 

    private void Awake()
    {
        isOven = false;
    }

    private void Start()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        progressTime = 0;

        ovenImage.fillAmount = 0;
    }

    public void Update()
    {
        if (isOven == true)
        {
            progressTime += Time.deltaTime;
            ovenImage.fillAmount = progressTime / timeToComplete;

            if (progressTime >= timeToComplete)
            {
                isOven = false;
                GameObject cheescake = Instantiate(resultFood, positionToSpawn.position, positionToSpawn.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
                cheescake.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.JapaneseCheesecake, cheescake.GetComponent<FoodBehaviour>()._valueOfFood, "JapaneseCheescake"); //Se accede al componente de comida para asignale sus valores.
                ResetValues();
            }
        }
    }

    public bool StartOven(FoodBehaviour foodToOven)
    {
        for (int i = 0; i < foodsAllowed.Length; i++ )
        {
            if (foodToOven._food == foodsAllowed[i])
            {
                timeToComplete = timesOfOvensForFoods[i];
                resultFood = resultFoods[i];

                ResetValues();

                isOven = true;
                return true;
            }
        }

        return false;
    }
}
