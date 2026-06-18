using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MochisBehaviour : MiniGamesBase, ICollitionMouse, IOneInputDetection
{
    [Header("World References")]

    [SerializeField] private GameObject[] objectsPhase1; //GameObjects que se usaen la fase 1
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usaen la fase 2

    [SerializeField] private TextMeshProUGUI counter;

    [Header("Cut Ingredients Part")]
    [SerializeField] private int ingredientsNeededToCut = 9;
    private int ingredientsCut;

    [SerializeField] private LifeBehaviour life;

    [SerializeField] private GameObject ingredientOnMinigame; //El ingrediente que se uso en la mesa, el cuál se usará para el minijuego

    [SerializeField] private ThrowingIngredients throwingSystem;

    [SerializeField] private GameObject defaultIngredient;

    [SerializeField] private TextMeshProUGUI readyText;

    [Header("BeatPart")]

    private bool isInSkillCheck;

    [SerializeField] private BeatBehaviour beat;

    [SerializeField] private AutomaticSkillCheck automaticSkillCheck;

    private int mochisDone; //Contador de mochis realizados
    [SerializeField] private int mochisToDo; //La cantidad de mochis necesarios para que se acabe el minijuego

    [SerializeField] private GameObject circleFeedback;

    [SerializeField] private GameObject spaceFeedback;

    [Header("Results")]
    [SerializeField] private GameObject mochisToSpawn;
    [SerializeField] private Transform mochisTransformToSpawn;

    [SerializeField] private GameObject[] objectsToRendererPhase1;
    [SerializeField] private GameObject[] objectsToRendererPhase2;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip[] regresiveCountSound;
    [SerializeField] private AudioClip katanaCutSound;
    [SerializeField] private AudioClip bombSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip succesSoundSkillCheck;

    protected override void Start()
    {
        base.Start();
    }

    public override bool Request(GameObject objectNeeded)
    {
        if (objectNeeded != null)
        {
            if (objectNeeded.GetComponent<FoodBehaviour>() != null)
            {
                if (objectNeeded.GetComponent<MeshFilter>() != null)
                {
                    //ingredientOnMinigame.GetComponent<MeshFilter>().mesh = objectNeeded.GetComponent<MeshFilter>().mesh;
                    //ingredientOnMinigame.GetComponent<MeshRenderer>().material = objectNeeded.GetComponent<MeshRenderer>().material;
                    //ingredientOnMinigame.transform.localScale = objectNeeded.transform.localScale * 3f;
                }

                if (objectNeeded.GetComponent<FoodBehaviour>()._food == requestFood[0])
                {
                    return true;
                }
                else if (objectNeeded.GetComponent<FoodBehaviour>()._food == requestFood[1])
                {
                    return true;
                }
                else if (objectNeeded.GetComponent<FoodBehaviour>()._food == requestFood[2])
                {
                    return true;
                }
            }

        }

        ingredientOnMinigame = defaultIngredient;
        return false;
    }

    public void OnEnable()
    {
        inCinematic = false;
        actualPhase = 0;
        ingredientsCut = 0;
        mochisDone = 0;
        isInSkillCheck = false;
        readyText.text = string.Empty;
        beat.onMixCompleted += StartSkillCheck;
        StartCoroutine(InitWithDelay());
    }

    private void OnDisable()
    {
        automaticSkillCheck.gameObject.SetActive(false);
        readyText.text = string.Empty;
        beat.onMixCompleted -= StartSkillCheck;
    }

    public void StartSkillCheck()
    {
        beat.SetMixState(false);
        isInSkillCheck = true;
        SkillCheck();
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
        inCinematic = true;

        StartCoroutine(MochisSetReady());
    }

    private IEnumerator MochisSetReady()
    {
        readyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.3f);

        readyText.text = "Set";
        soundManager.ReproduceSound(regresiveCountSound[0]);

        yield return new WaitForSeconds(0.5f);

        readyText.text = "Ready";
        soundManager.ReproduceSound(regresiveCountSound[1]);

        yield return new WaitForSeconds(0.5f);

        readyText.text = "CUT";
        soundManager.ReproduceSound(regresiveCountSound[2]);

        foreach (GameObject phase1Object in objectsPhase1)
        {
            phase1Object.SetActive(true);
        }

        throwingSystem.SetDefaultsValues(ingredientOnMinigame);
        UpdateUI();

        yield return new WaitForSeconds(1f);

        inCinematic = false;
        readyText.text = string.Empty;
    }

    public override void PhaseTwo()
    {
        foreach (GameObject phase1Object in objectsPhase1)
        {
            phase1Object.SetActive(false);
        }

        foreach (GameObject phase2Object in objectsPhase2)
        {
            phase2Object.SetActive(true);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
        UpdateUI();
    }

    public override void StopMinigame(bool succes)
    {
        foreach (GameObject objects in objectsPhase2) //Se desactivan los minijuegos de la fase 2
        {
            objects.SetActive(false);
        }

        circleFeedback.SetActive(false);
        beat.ResetMix();

        if (succes == true) //Si se completo
        {
            GameObject mochis = Instantiate(mochisToSpawn, mochisTransformToSpawn.position, mochisTransformToSpawn.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
            mochis.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.Mochis, valueOfFood, "WokRice"); //Se accede al componente de comida para asignale sus valores.
            mochis.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction>()._heldObject = mochis;
        }
        else
        {
            foreach (GameObject objects in objectsPhase1) //Se desactivan los minijuegos de la fase 2
            {
                objects.SetActive(false);
            }
        }

        miniGameManager.StopMiniGameExecute(); //Desde el manager se da la señal para que vuelva al minijuego
    }

    public void Collition(GameObject collider)
    {
        if (collider.CompareTag("IngredientGenerator") == true)
        {
            collider.GetComponent<IngredientToCut>().CutIngredient();
            ingredientsCut++;

            soundManager.ReproduceSound(katanaCutSound);

            UpdateUI();

            if (ingredientsCut >= ingredientsNeededToCut)
            {
                actualPhase++;
                PhaseTwo();
            }
        }
        else if (collider.CompareTag("Bomb") == true)
        {
            collider.GetComponent<IngredientToCut>().CutIngredient();

            soundManager.ReproduceSound(bombSound);

            if (life.RestLife() < 1)
            {
                StopMinigame(false);
            }
        }
    }

    //public void ReciveInput(char input)
    //{
    //    if (isInSkillCheck == false)
    //    {
    //        if (beat.BeatComplete(input) == true)
    //        {
    //            isInSkillCheck = true;
    //            SkillCheck();
    //        }
    //    }
    //}

    private void SkillCheck()
    {
        circleFeedback.SetActive(false);

        spaceFeedback.SetActive(true);

        automaticSkillCheck.gameObject.SetActive(true);

        beat.SetMixState(false);
    }

    public void InputEvent(bool remove = false)
    {
        if (isInSkillCheck == true)
        {
            if (automaticSkillCheck.ResolveSkillCheck(remove) == true)
            {
                mochisDone++;
                soundManager.ReproduceSound(succesSoundSkillCheck);

                UpdateUI();

                if (mochisDone >= 3)
                {
                    StopMinigame(true);
                    beat.ResetMix();

                    beat.SetMixState(true);

                    isInSkillCheck = false;
                    spaceFeedback.SetActive(false);

                    return;
                }
            }
            else
            {
                circleFeedback.SetActive(true);
                soundManager.ReproduceSound(failSound);
            }

            //beat.BeatComplete("0"[0]);
            beat.ResetMix();
            beat.SetMixState(true);
            isInSkillCheck = false;
            spaceFeedback.SetActive(false);
        }
    }

    public override void PauseMinigame()
    {
        switch (actualPhase)
        {
            case 0:
                foreach (GameObject objectOfMinigame in objectsToRendererPhase1)
                {
                    objectOfMinigame.SetActive(false);
                }

                GameManager.instance.DefineCursor(0);

                break;

            case 1:
                InputEvent(true);

                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(false);
                }

                spaceFeedback.SetActive(false);
                beat.ResetMix();
                break;
        }

        StopAllCoroutines();
    }

    public override void ResumeMinigame()
    {
        switch (actualPhase)
        {
            case 0:
                foreach (GameObject objectOfMinigame in objectsToRendererPhase1)
                {
                    objectOfMinigame.SetActive(true);
                }

                ingredientsCut = 0;

                readyText.gameObject.SetActive(false);
                throwingSystem.SetDefaultsValues(ingredientOnMinigame);
                break;

            case 1:
                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;
        }
        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
        UpdateUI();
    }

    private void UpdateUI()
    {
        switch (actualPhase)
        {
            case 0:
                counter.text = ingredientsCut.ToString() + "/" + ingredientsNeededToCut.ToString();
                GameManager.instance.DefineCursor(1);
                break;

            case 1:
                counter.text = mochisDone.ToString() + "/" + mochisToDo.ToString();
                GameManager.instance.DefineCursor(0);
                break;
        }
    }

}
