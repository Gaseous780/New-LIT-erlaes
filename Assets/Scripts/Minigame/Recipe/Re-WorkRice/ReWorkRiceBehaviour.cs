using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReWorkRiceBehaviour : MiniGamesBase, IAddIngredients, ISkillCheck, IReSkillCheck, IAddedGradualIngredient, IDancer
{
    [SerializeField] private GameObject[] objectsPhase1; //GameObjects que se usaen la fase 1
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usaen la fase 2

    //[SerializeField] private string[] controlsToUse; //Un string que guarda los nombres de los controles que se usan en cada fase del minijuego, estos luego son cargados para cambiar los controles. Su index es la fase del juego (el número esta con uno menos, debido a que se cuenta la 0 como 1, 1 como 2 y así progresivamente). Deben de ser exactos a como estan en el playerInput de los minijuegos

    [SerializeField] private IngredientGenerator eggs; //La clase que genera los huevos
    [SerializeField] private BarBehaviour barProgress; //La clase con la barra de ritmo
    [SerializeField] private FryPanSkillCheck fryPanSkillCheck; //La clase con el skill check

    [Header("Ingredient Section")]
    [SerializeField] private int eggsNecesary = 2;
    private int eggsOnBowl; //La cantidad de huevos que van en la sarten (antes era un bol)
    [SerializeField] private TextMeshProUGUI counterEggs;
    [SerializeField] private Image checkImageEggs;
    private float amountOfRice;
    [SerializeField] private float riceNecesary;
    [SerializeField] private TextMeshProUGUI counterRice;
    [SerializeField] private Image checkImageRice;

    [SerializeField] private GameObject UIPhaseOne;

    [SerializeField] private GameObject lightEgg;
    [SerializeField] private GameObject lightRice;

    [Header("New Section RE-wok")]
    [SerializeField] private ReFryVariant reFry;
    [SerializeField] private TextMeshProUGUI textFeedback;
    private DirectionsController directionsController;

    private int counterSuccesReFry;
    private int nextDirection;

    [SerializeField] private int amountOfSuccesNecesariesRe = 6;
    [SerializeField] private int amountOfErrorsRe = 3;

    [SerializeField] private Image nextArrow;
    [SerializeField] private Image currentArrow;

    [SerializeField] private GameObject UIPhaseTwo;
    [SerializeField] private TextMeshProUGUI readyText;

    [SerializeField] private float timeToRecallArrow;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private Image[] arrowsOnScreenUI;

    [SerializeField] private TextMeshProUGUI textProgressWokPart2;
    [SerializeField] private Image[] errorsImagesUI;
    [SerializeField] private TextMeshProUGUI itsOverText;

    [Header("SkillCheck")]
    [SerializeField] private float maxTimeToAppaer; //Tiempo máximo que puede tardar en aparecer el skillCheck
    [SerializeField] private float minTimeToAppaer; //Tiempo minimo que puede tardar en aparecer el skillCheck

    private float pointsOnFryPan; //Puntos en los que se acerto el skill check

    [SerializeField] private float timeToReCall;

    [SerializeField] private ParticleSystem particles;

    [Header("Mesh")]
    [SerializeField] private MeshRenderer rendererSG;
    [SerializeField] private int index = 1;
    private MaterialPropertyBlock mpb;


    [Header("In - World")]
    [SerializeField] private GameObject wokToSpawn; //El prefab del arroz con wok
    [SerializeField] private Transform spawnPosition; //La posición en la que aparecerá el arroz con wok
    [SerializeField] private FireBehaviour fireBehaviour; //Clase del fuego que se encuentra sobre la mesa
    [SerializeField] private GameObject[] backGround;
    [SerializeField] private MeshRenderer wokRice;
 
    [SerializeField] private GameObject[] objectsToRendererPhase1;
    [SerializeField] private GameObject[] objectsToRendererPhase2;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip initSoundPan;
    [SerializeField] private AudioClip fireErrorSound;
    [SerializeField] private AudioClip succesSound;
    [SerializeField] private AudioClip finalSkillCheck;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    protected override void Start()
    {
        base.Start();

        directionsController = reFry.gameObject.GetComponent<DirectionsController>();
    }

    private void OnEnable() //Se tiene que activar al encender del minijuego, así de esta forma se va a poder a hacer cada vez que se inicialice el minijuego. Esto significa que cada vez que se prenda el gameObject, el minijuego se activará desde el inicio. No implica que al desactivarlo se apaguen los elementos del minijuego
    {//Seteo de variables a sus valores iniciales
        actualPhase = 0;

        counterSuccesReFry = 0;

        pointsOnFryPan = 0;
        eggsOnBowl = 0;

        errorCounters = 0;
        amountOfRice = 0;

        nextDirection = Random.Range(0, 8);

        inCinematic = false;

        foreach (GameObject go in backGround) 
        {
            go.SetActive(true);
        }

        StartCoroutine(InitWithDelay());

        rendererSG.GetPropertyBlock(mpb, index);

        mpb.SetFloat("_OnFire", 0);

        rendererSG.SetPropertyBlock(mpb, index);
    }

    public override IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(0.3f);

        StartMinigame(); //Si no se inicia con un margen de tiempo dará null
    }

    public override void StartMinigame()
    {
        miniGameControllers.ChangeControl(controlsToUse[actualPhase]); //Se cambian los controles a los de la fase actual (1) con el pasaje de un string con los controles deseados

        PhaseOne();
    }

    public override void PhaseOne()
    {
        foreach (GameObject objects in objectsPhase1) //Activación de todos los gameObjects de la fase 1
        {
            objects.SetActive(true);
        }

        UpdateUI();
    }
    public override void PhaseTwo()
    {
        inCinematic = true;

        foreach (GameObject objects in objectsPhase1) //Desactivación de todos los gameObjects de la fase 1
        {
            objects.SetActive(false);
        }
        foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
        {
            ingredients.SetActive(false);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]); //Se cambia a los controles de la fase 2
        minigameCanvas.gameObject.SetActive(true);

        foreach (Image arrow in arrowsOnScreenUI)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }

        foreach (Image error in errorsImagesUI)
        {
            error.color = new Color(1, 1, 1, 0);
        }
        itsOverText.enabled = false;

        StartCoroutine(SetReady());
    }

    private IEnumerator SetReady()
    {
        wokRice.enabled = false;

        readyText.text = "Let";

        yield return new WaitForSeconds(0.5f);

        readyText.text = "Him";

        yield return new WaitForSeconds(0.5f);

        readyText.text = "COOK";

        soundManager.ReproduceSound(initSoundPan);

        foreach (GameObject phase1Object in objectsPhase2)
        {
            phase1Object.SetActive(true);
        }

        DefineNextDirection();

        UpdateUI();

        yield return new WaitForSeconds(1f);

        readyText.text = string.Empty;
        inCinematic = false;
    }

    public override void StopMinigame(bool succes)
    {
        base.StopMinigame(succes);

        foreach (GameObject objects in objectsPhase2) //Se desactivan los minijuegos de la fase 2
        {
            objects.SetActive(false);
        }
        fryPanSkillCheck.gameObject.SetActive(false); //Se desactiva la skill check, debido a que no esta en el array de los gameObjects

        if (succes == true) //Si se completo
        {
            GameObject wok = Instantiate(wokToSpawn, spawnPosition.position, spawnPosition.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
            wok.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.WorkRice, valueOfFood, "WokRice"); //Se accede al componente de comida para asignale sus valores.
            wok.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction>()._heldObject = wok;
        }
        else //Si no se completo
        {
            fireBehaviour.gameObject.SetActive(true); //Se activa el gameObject del fuego
            fireBehaviour.EnableConsecuences(); //Se activan las consecuenciaas
        }

        miniGameManager.StopMiniGameExecute(); //Desde el manager se da la señal para que vuelva al minijuego

        inCinematic = false;
    }

    public override bool Error()
    {
        rendererSG.GetPropertyBlock(mpb, index);

        mpb.SetFloat("_OnFire", 1);

        rendererSG.SetPropertyBlock(mpb, index);

        if (errorCounters > amountOfErrorsRe) //Al llegar al máximo de errores
        {
            StopMinigame(false); //Se para el juego, con la señal de que se fallo

            return true;
        }

        errorsImagesUI[errorCounters].color = new Color(1, 1, 1, 1);
        if (errorCounters == amountOfErrorsRe) { itsOverText.enabled = true; }

        errorCounters++; //Se aumenta el contador de errores
        soundManager.ReproduceSound(fireErrorSound);

        return false;
    }
    public void AddIngredientsToBowl() //Para la primera fase del minijuego en la que se deben de agregar huevos
    {
        eggsOnBowl++; //Se suma un huevo al contador 

        if (eggsOnBowl >= eggsNecesary)
        {
            lightEgg.SetActive(false);
        }

        CheckIngrediens();
    }
    public void AddThrowIngredient(float amount)
    {
        amountOfRice += amount;
        
        if (amountOfRice >= riceNecesary)
        {
            lightRice.SetActive(false);
        }

        CheckIngrediens();
    }

    private void CheckIngrediens()
    {
        UpdateUI();

        if (amountOfRice >= riceNecesary && eggsOnBowl >= eggsNecesary)
        {
            actualPhase++;
            PhaseTwo();
        }
    }

    public void DefineNextDirection()
    {
        int previousDirection = nextDirection;

        //reFry.DefineDirection(previousDirection);
        arrows[previousDirection].SetActive(true);
        directionsController.SetVisuals(previousDirection);

        nextDirection = SetDifferentNumber (previousDirection);

        //GetDirectionArrow(previousDirection);

        StartCoroutine(CallArrow());
    }

    private IEnumerator CallArrow()
    {
        yield return new WaitForSeconds(timeToRecallArrow);

        DefineNextDirection();
    }

    public int SetDifferentNumber (int numberToAvoid)
    {
        int number = Random.Range(0, 8);

        if (number == numberToAvoid)
        {
            number = SetDifferentNumber(numberToAvoid);
        }

        return number;
    }

    //Nuevo RE---
    public void CompleteSkillCheck(bool succes)
    {
        if (succes == true)
        {
            counterSuccesReFry++;
            soundManager.ReproduceSound(succesSound);

            UpdateUI();

            if (counterSuccesReFry >= amountOfSuccesNecesariesRe)
            {
                StartFinalSkillCheck();
                return;
            }
        }
        else
        {
            Error();
        }

        //DefineNextDirection();
    }

    //SkillCheck final

    private void StartFinalSkillCheck()
    {
        StopAllCoroutines();
        foreach (GameObject arrow in arrows)
        {
            arrow.SetActive(false);
        }
        objectsPhase2[0].SetActive(false); //Son las flechas. En estos momentos estan en el index 0

        reFry.Drop();
        reFry.SetReset(true);
        reFry.gameObject.GetComponent<Collider>().enabled = false;
        fryPanSkillCheck.gameObject.SetActive(true);
    }

    public void FryPanAddPoints() //Se llama cuando se cumple el skill check una vez
    {
        pointsOnFryPan++; //Se agrega un punto
        fryPanSkillCheck.gameObject.SetActive(false); //Se desactiva el gameObject del skill check

        FeedbackSkillCheckComplete();

        inCinematic = true;
        reFry.PlayFinalAnimation();
    }

    private void FeedbackSkillCheckComplete()
    {
        particles.Play();
        soundManager.ReproduceSound(finalSkillCheck);
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
                if (eggs._ingredientsList.Count > 0)
                {
                    foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
                    {
                        ingredients.SetActive(false);
                    }
                }

                wokRice.enabled = false;
                break;

            case 1:
                StopAllCoroutines();

                foreach (GameObject arrow in arrows)
                {
                    arrow.SetActive(false);
                }

                foreach (GameObject objectOfMinigame in objectsToRendererPhase2)
                {
                    objectOfMinigame.SetActive(false);
                }

                foreach (Image arrow in arrowsOnScreenUI)
                {
                    arrow.color = new Color(1, 1, 1, 0);
                }

                foreach (Image error in errorsImagesUI)
                {
                    error.color = new Color(1, 1, 1, 0);
                }

                itsOverText.enabled = false;

                counterSuccesReFry = 0;
                break;
        }
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
                break;

            case 1:
                foreach (GameObject objectOfMinigame in objectsToRendererPhase2)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
        UpdateUI();

        DefineNextDirection();
    }

    public void UpdateUI()
    {
        switch (actualPhase)
        {
            case 0:
                UIPhaseTwo.SetActive(false);
                UIPhaseOne.SetActive(true);
                textFeedback.text = "Ingredients needed";

                if (amountOfRice < riceNecesary)
                {
                    checkImageRice.color = new Color32(255, 255, 255, 0);
                    counterRice.text = amountOfRice.ToString("f2") + "/" + riceNecesary.ToString();
                }
                else
                {
                    checkImageRice.color = new Color32(255, 255, 255, 255);
                    counterRice.text = string.Empty;
                }

                if (eggsOnBowl < eggsNecesary)
                {
                    checkImageEggs.color = new Color32(255, 255, 255, 0);
                    counterEggs.text = eggsOnBowl.ToString() + "/" + eggsNecesary.ToString();
                }
                else
                {
                    checkImageEggs.color = new Color32(255, 255, 255, 255);
                    counterEggs.text = string.Empty;
                }
                break;

            case 1:
                UIPhaseOne.SetActive(false);
                UIPhaseTwo.SetActive(true);

                checkImageRice.color = new Color32(255, 255, 255, 0);
                counterRice.text = string.Empty;
                checkImageEggs.color = new Color32(255, 255, 255, 0);
                counterEggs.text = string.Empty;

                textFeedback.text = "Arrows\r\nin screen";

                textProgressWokPart2.text = counterSuccesReFry.ToString() + "/" + amountOfSuccesNecesariesRe;
                break;
        }
    }

    public void EnableArrowInfo(int directionPass)
    {
        for (int i = 0; i < arrowsOnScreenUI.Length - 1; i++)
        {
            if (arrowsOnScreenUI[i].color.a == 0)
            {
                arrowsOnScreenUI[i].transform.eulerAngles = DirectionRotationArrow(directionPass);
                arrowsOnScreenUI[i].color = new Color(255, 255, 255, 255);
                return;
            }
        }
    }

    public void DisableArrowInfo (int directionPass)
    {
        for (int i = 0; i < arrowsOnScreenUI.Length - 1; i++)
        {
            if (arrowsOnScreenUI[i].transform.eulerAngles == DirectionRotationArrow(directionPass) && arrowsOnScreenUI[i].color.a != 0)
            {
                arrowsOnScreenUI[i].color = new Color32(255, 255, 255, 0);
                return;
            }
        }
    }

    private Vector3 DirectionRotationArrow(int directionToGet)
    {
        switch (directionToGet)
        {
            case 0:
                return new Vector3(0, 0, 0);

            case 1:
                return new Vector3(0, 0, 180);

            case 2:
                return new Vector3(0, 0, 270);

            case 3:
                return new Vector3(0, 0, 90);

            case 4:
                return new Vector3(0, 0, 315);

            case 5:
                return new Vector3(0, 0, 45);
                
            case 6:
                return new Vector3(0, 0, 225);

            case 7:
                return new Vector3(0, 0, 135);

            default:
                return Vector3.zero;
        }
    }
}
