using System.Collections;
using UnityEngine;

public class WokRiceBehaviour : MiniGamesBase, ISkillCheck, IRythm, IAddIngredients //Hereda de la clase base para ser tratado como un minijuego más y sus interfaces son para los métodos de cada minijuego
{
    [SerializeField] private GameObject[] objectsPhase1; //GameObjects que se usaen la fase 1
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usaen la fase 2

    //[SerializeField] private string[] controlsToUse; //Un string que guarda los nombres de los controles que se usan en cada fase del minijuego, estos luego son cargados para cambiar los controles. Su index es la fase del juego (el número esta con uno menos, debido a que se cuenta la 0 como 1, 1 como 2 y así progresivamente). Deben de ser exactos a como estan en el playerInput de los minijuegos

    [SerializeField] private IngredientGenerator eggs; //La clase que genera los huevos
    [SerializeField] private BarBehaviour barProgress; //La clase con la barra de ritmo
    [SerializeField] private FryPanSkillCheck fryPanSkillCheck; //La clase con el skill check

    private int eggsOnBowl; //La cantidad de huevos que van en la sarten (antes era un bol)

    private float pointsOnFryPan; //Puntos en los que se acerto el skill check

    [Header("SkillCheck")]
    [SerializeField] private float maxTimeToAppaer; //Tiempo máximo que puede tardar en aparecer el skillCheck
    [SerializeField] private float minTimeToAppaer; //Tiempo minimo que puede tardar en aparecer el skillCheck

    [SerializeField] private float timeToDisableFryPan = 3f; //Tiempo que tarda en desactivarse el skill check

    private bool canCallAgain;

    [SerializeField] private float timeToReCall;

    [SerializeField] private ParticleSystem particles;

    [Header("In - World")]
    [SerializeField] private GameObject wokToSpawn; //El prefab del arroz con wok
    [SerializeField] private Transform spawnPosition; //La posición en la que aparecerá el arroz con wok
    [SerializeField] private FireBehaviour fireBehaviour; //Clase del fuego que se encuentra sobre la mesa

    [SerializeField] private GameObject[] objectsToRendererPhase1;
    [SerializeField] private GameObject[] objectsToRendererPhase2;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable() //Se tiene que activar al encender del minijuego, así de esta forma se va a poder a hacer cada vez que se inicialice el minijuego. Esto significa que cada vez que se prenda el gameObject, el minijuego se activará desde el inicio. No implica que al desactivarlo se apaguen los elementos del minijuego
    {//Seteo de variables a sus valores iniciales
        actualPhase = 0;

        pointsOnFryPan = 0;
        eggsOnBowl = 0;

        errorCounters = 0;

        canCallAgain = true;

        StartCoroutine(InitWithDelay());
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
    }
    public override void PhaseTwo()
    {
        foreach (GameObject objects in objectsPhase1) //Desactivación de todos los gameObjects de la fase 1
        {
            objects.SetActive(false);
        }
        foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
        {
            ingredients.SetActive(false);
        }
        foreach (GameObject objects in objectsPhase2) //Activación de todos los gameObjects de la fase 2
        {
            objects.SetActive(true);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]); //Se cambia a los controles de la fase 2
    }

    public override void StopMinigame(bool succes)
    {
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
    }

    public override bool Error()
    {
        errorCounters++; //Se aumenta el contador de errores

        if (errorCounters > 3) //Al llegar al máximo de errores
        {
            StopMinigame(false); //Se para el juego, con la señal de que se fallo

            return true;
        }

        return false;
    }
    public void AddIngredientsToBowl() //Para la primera fase del minijuego en la que se deben de agregar huevos
    {
        eggsOnBowl++; //Se suma un huevo al contador 

        if (eggsOnBowl > 1) //Al llegar a los huevos necesarios
        {
            actualPhase++; //Se cambia de fase a la siguiente
            PhaseTwo();
        }
    }

    public void AddPointsOnRythm(int points) //Inputs: Z = 1 y X = -1. Para la barra de ritmo al presionar un input
    {
        if (points == 1) //Si es Z
        {
            barProgress.AddPoints(Mathf.Abs(points), true);
        }
        else //Si es X
        {
            barProgress.AddPoints(Mathf.Abs(points), false);
        }
    }

    public void RythmEvent() //Disparador del evento al estar en el punto ideal del ritmo
    {
        StartCoroutine(StartFryPanDrag());
    }

    private IEnumerator StartFryPanDrag() //Hace que se active el skill check
    {
        if (barProgress._isInEvent == false && canCallAgain == true) //Para que no se inicie varias veces
        {
            canCallAgain = false;

            float timeToAppaer = Random.Range(minTimeToAppaer, maxTimeToAppaer + 1); //Tiempo que será definido por un Random entre el tiempo minimo y máximo, el +1 es para que considere el último
            yield return new WaitForSeconds(timeToAppaer); //La corrutina espera el tiempo definido anteriormente

            barProgress._isInEvent = true;
            if (barProgress._isOnIdeal == true) //Si el punto ideal aún se mantiene 
            {
                canCallAgain = false;

                fryPanSkillCheck.gameObject.SetActive(true); //Se activa el gameObject de la skill check

                yield return new WaitForSeconds(timeToDisableFryPan); //Tiempo de vida de la skill check

                fryPanSkillCheck.gameObject.SetActive(false); //El gameObject de la skill check se desactiva
            }

            barProgress._isInEvent = false; //Ya se puede inicializar de vuelta

            yield return new WaitForSeconds(timeToAppaer);

            canCallAgain = true;
        }

        yield return null;
    }

    public void FryPanAddPoints() //Se llama cuando se cumple el skill check una vez
    {
        pointsOnFryPan++; //Se agrega un punto
        fryPanSkillCheck.gameObject.SetActive(false); //Se desactiva el gameObject del skill check
        barProgress._isInEvent = false;

        FeedbackSkillCheckComplete();

        if (pointsOnFryPan >= 3) //Si se cumplió con los puntos requeridos
        {
            StopMinigame(true); //Se para el minijuego con exito
        }
    }

    private void FeedbackSkillCheckComplete()
    {
        particles.Play();
        soundManager.ReproduceSound(0);
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
                if (eggs._ingredientsList.Count > 0)
                {
                    foreach (GameObject ingredients in eggs._ingredientsList) //Si quedo un huevo en la mesa sin usar se desactiva
                    {
                        ingredients.SetActive(false);
                    }
                }
                break;

            case 1:
                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(false);
                }
                break;
        }
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
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
    }
}
