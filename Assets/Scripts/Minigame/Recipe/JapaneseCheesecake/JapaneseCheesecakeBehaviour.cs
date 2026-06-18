using System.Collections;
using TMPro;
using UnityEngine;

public class JapaneseCheesecakeBehaviour : MiniGamesBase, IAddIngredients, IWASDInput, IPointerClick
{
    [Header("World References")]

    [SerializeField] private GameObject[] objectsPhase1; //GameObjects que se usan en la fase 1
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usan en la fase 2
    [SerializeField] private GameObject[] objectsPhase3; //GameObjects que se usan en la fase 3

    [SerializeField] private TextMeshProUGUI counterText;

    [Header("Object Result")]
    [SerializeField] private GameObject cheescakeToSpawn;
    [SerializeField] private Transform cheescakeTransformToSpawn;

    [Header("Phase 1")]
    [SerializeField] private int eggsToBreak = 4; //La cantidad de huevos que se deben de romper para pasar a la siguiente fase
    private int eggsOnBowl;

    [SerializeField] private IngredientGenerator eggs; //Para sacar los huevos restantes

    [Header("Phase2")]
    [SerializeField] private float amountNeededOfIngredient;
    [SerializeField] private float actualAmount;

    [SerializeField] private string[] ingredientsNeededInBowl;
    private int ingredientCountOnBowl; //Determina que ingrediente es el que se debe de poner después de haber agregado un ingrediente

    private string ingredientNeeded; //Si es empty significa que no se necesita ningún ingrediente

    private int actualLaps; //Veces que se hizo el batido y poner los condimentos
    [SerializeField] private int lapsToDo; //La cantidad de veces que se tiene que batir para pasar a la siguiente fase

    [SerializeField] private BeatBehaviour beat;

    [SerializeField] private int extraControl; //Número de index que se debe acceder al control extra para la segunda fase (drag)

    [SerializeField] private Bowl bowlPhase2;

    private bool isAddingIngredients; //Para el momento donde se agregan ingredientes en la fase dos así no se usan los dos inputs

    [SerializeField] private IngredientGenerator suggar;
    [SerializeField] private IngredientGenerator butter;

    [Header("Phase 3")]
    [SerializeField] private int amountOfMoldsToDo = 6;
    private int actualMoldsComplete;

    [SerializeField] private GameObject[] objectsToRendererPhase1;
    [SerializeField] private GameObject[] objectsToRendererPhase2;
    [SerializeField] private GameObject[] objectsToRendererPhase3;

    protected override void Start()
    {
        base.Start();
    }

    public void OnEnable()
    {
        actualPhase = 0;
        eggsOnBowl = 0;
        actualLaps = 0;
        isAddingIngredients = false;

        ingredientCountOnBowl = 0;

        bowlPhase2.enabled = false;

        StartCoroutine(InitWithDelay());
    }

    public override IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        StartMinigame();
    }

    public override void StartMinigame()
    {
        miniGameControllers.ChangeControl(controlsToUse[actualPhase]); //Se cambian los controles a los de la fase actual con el pasaje de un string con los controles deseados

        PhaseOne();
    }

    public override void PhaseOne()
    {
        foreach (GameObject phase1Object in objectsPhase1)
        {
            phase1Object.SetActive(true);
        }

        UpdateUI();
    }

    public override void PhaseTwo()
    {
        foreach (GameObject phase1Object in objectsPhase1)
        {
            phase1Object.SetActive(false);
        }
        foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
        {
            ingredients.SetActive(false);
        }

        foreach (GameObject phase2Object in objectsPhase2)
        {
            phase2Object.SetActive(true);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);

        SetTypeOfBeat();

        UpdateUI();
    }

    public override void PhaseThree()
    {
        foreach (GameObject phase1Object in objectsPhase2)
        {
            phase1Object.SetActive(false);
        }
        foreach (GameObject ingredients in suggar._ingredientsList) //Si quedo algo de azucar en la mesa sin usar se desactiva
        {
            ingredients.SetActive(false);
        }
        foreach (GameObject ingredients in butter._ingredientsList) //Si quedo algo de manteca en la mesa sin usar se desactiva
        {
            ingredients.SetActive(false);
        }

        foreach (GameObject phase3Object in objectsPhase3)
        {
            phase3Object.SetActive(true);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
    }

    public override void StopMinigame(bool succes)
    {
        foreach (GameObject objects in objectsPhase3) //Se desactivan los minijuegos de la fase 2
        {
            objects.SetActive(false);
        }

        if (succes == true) //Si se completo
        {
            GameObject cheescake = Instantiate(cheescakeToSpawn, cheescakeTransformToSpawn.position, cheescakeTransformToSpawn.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
            cheescake.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.JapaneseCheesecakeIncomplete, valueOfFood, "JapaneseCheescakeIncomplete"); //Se accede al componente de comida para asignale sus valores.
        }

        miniGameManager.StopMiniGameExecute(); //Desde el manager se da la señal para que vuelva al minijuego
    }

    public void AddIngredientsToBowl() //Para la primera fase del minijuego en la que se deben de agregar huevos
    {
        eggsOnBowl++; //Se suma un huevo al contador 
        UpdateUI();

        if (eggsOnBowl >= eggsToBreak) //Al llegar a los huevos necesarios
        {
            actualPhase++; //Se cambia de fase a la siguiente
            PhaseTwo();
        }
    }

    private void SetTypeOfBeat()
    {
        int typeOfSenseBeat = Random.Range(0, 2);

        //beat.BeatComplete(char.MinValue);
        //beat.SetTypeOfBeat(typeOfSenseBeat);
    }

    public void ReciveInput(char input)
    {
        if (isAddingIngredients == false)
        {
            //if (beat.BeatComplete(input) == true)
            //{
            //    InitializeBowlPart();
            //}
        }
    }

    private void InitializeBowlPart()
    {
        isAddingIngredients = true;
        miniGameControllers.ChangeControl(controlsToUse[extraControl]);

        ingredientNeeded = ingredientsNeededInBowl[ingredientCountOnBowl];
        bowlPhase2.enabled = true;
    }

    public void AddIngredientsToBowl(float amount, string ingredientPassed)
    {
        if (ingredientPassed == ingredientNeeded)
        {
            actualAmount += amount;

            if (actualAmount >= amountNeededOfIngredient)
            {
                actualAmount = 0;
                ingredientCountOnBowl++;

                if (ingredientCountOnBowl >= ingredientsNeededInBowl.Length)
                {
                    ingredientNeeded = string.Empty;
                }
                else
                {
                    ingredientNeeded = ingredientsNeededInBowl[ingredientCountOnBowl];
                }

                if (ingredientNeeded == string.Empty)
                {
                    actualLaps++;
                    ingredientCountOnBowl = 0;
                    bowlPhase2.enabled = false;

                    miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
                    SetTypeOfBeat();
                    isAddingIngredients = false;

                    foreach (GameObject ingredients in suggar._ingredientsList) //Si quedo algo de azucar en la mesa sin usar se desactiva
                    {
                        ingredients.SetActive(false);
                    }
                    foreach (GameObject ingredients in butter._ingredientsList) //Si quedo algo de manteca en la mesa sin usar se desactiva
                    {
                        ingredients.SetActive(false);
                    }

                    if (actualLaps >= lapsToDo)
                    {
                        actualPhase++;
                        PhaseThree();
                    }

                    UpdateUI();
                }
            }
        }
    }

    public void Click(GameObject objectOnClick)
    {
        if (objectOnClick.CompareTag("Mold"))
        {
            if (objectOnClick.GetComponent<MoldBehaviour>().CompleteMold() == true)
            {
                actualMoldsComplete++;
                UpdateUI();

                if (actualMoldsComplete >= amountOfMoldsToDo)
                {
                    StopMinigame(true);
                }
            }
        }
    }

    public void UpdateUI() //Para actualizar el contador en la UI
    {
        switch (actualPhase)
        {
            case 0:
                counterText.text = eggsOnBowl.ToString() + "/" + eggsToBreak.ToString();
                break;

            case 1:
                counterText.text = actualLaps.ToString() + "/" + lapsToDo.ToString();
                break;

            case 2:
                counterText.text = actualMoldsComplete.ToString() + "/" + amountOfMoldsToDo.ToString();
                break;
        }
    }
    public override void PauseMinigame()
    {
        switch (actualPhase)
        {
            case 0:
                foreach (GameObject objectOfMinigame in objectsPhase1)
                {
                    objectOfMinigame.SetActive(false);
                }
                foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
                {
                    ingredients.SetActive(false);
                }
                break;

            case 1:
                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(false);
                }
                foreach (GameObject ingredients in suggar._ingredientsList) //Si quedo algo de azucar en la mesa sin usar se desactiva
                {
                    ingredients.SetActive(false);
                }
                foreach (GameObject ingredients in butter._ingredientsList) //Si quedo algo de manteca en la mesa sin usar se desactiva
                {
                    ingredients.SetActive(false);
                }
                break;

            case 2:
                foreach (GameObject objectOfMinigame in objectsPhase3)
                {
                    objectOfMinigame.SetActive(false);
                }

                break;
        }

        isAddingIngredients = false;
        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
    }

    public override void ResumeMinigame()
    {
        switch (actualPhase)
        {
            case 0:
                foreach (GameObject objectOfMinigame in objectsPhase1)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;

            case 1:
                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;

            case 2:
                foreach (GameObject objectOfMinigame in objectsPhase3)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
        UpdateUI();
    }

}
