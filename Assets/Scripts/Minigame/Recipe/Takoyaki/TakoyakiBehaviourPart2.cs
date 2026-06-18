using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakoyakiBehaviourPart2 : MiniGamesBase, IMovePlayerNodes, IFryer
{
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usaen la fase 2

    [SerializeField] private FryerBehaviour fryer;

    private int[] pointsOnFry;
    private int totalFrys;

    private int finishValueOfFood;

    [SerializeField] private GameObject takoyakiToSpawn;
    [SerializeField] private Transform spawnPosition;

    [SerializeField] private ConsecuencesBase consecuences;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        actualPhase = 0;
        errorCounters = 0;

        totalFrys = 0;
        pointsOnFry = new int[3];
        finishValueOfFood = (int)valueOfFood;
        StartCoroutine(InitWithDelay());
    }

    public override IEnumerator InitWithDelay()
    {
        yield return new WaitForSeconds(0.3f);

        StartMinigame(); //Si no se inicia con un margen de tiempo dará null
    }

    public override void StartMinigame()
    {
        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);

        PhaseOne();
    }

    public override void PhaseOne()
    {
        foreach (GameObject objects in objectsPhase2)
        {
            objects.SetActive(true);
        }
    }

    public override void StopMinigame(bool succes)
    {
        for (int i = 0; i < pointsOnFry.Length - 1; i++)
        {
            switch (pointsOnFry[i])
            {
                case 2:
                    finishValueOfFood += (int)valueOfFood / 3;
                    break;
                case 3:
                    finishValueOfFood += (int)valueOfFood / 4;
                    break;
                case 4:
                    finishValueOfFood += (int)valueOfFood / 5;
                    break;
            }
        }

        if (succes == true)
        {
            GameObject takoyaki = Instantiate(takoyakiToSpawn, spawnPosition.position, spawnPosition.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
            takoyaki.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.Takoyaki, finishValueOfFood, "Takoyaki"); //Se accede al componente de comida para asignale sus valores.
            takoyaki.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction>()._heldObject = takoyaki;
        }
        else
        {
            consecuences.gameObject.SetActive(true);
            consecuences.EnableConsecuences();
        }

        GameManager.instance._miniGameManager.StopMiniGameExecute();
    }

    public override bool Error()
    {
        StopMinigame(false);

        return true;
    }

    public void FryActionDown()
    {
        fryer.HandDown();
    }

    public void FryActionUp()
    {
        float score = fryer.HandUp();

        if (score == default) { return; }

        if (score >= 5) { Error(); }

        pointsOnFry[totalFrys] = (int)score;
        totalFrys++;

        if (totalFrys > 2)
        {
            StopMinigame(true);
        }
    }

    public override void PauseMinigame()
    {
        switch (actualPhase)
        {
            case 0:
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
                foreach (GameObject objectOfMinigame in objectsPhase2)
                {
                    objectOfMinigame.SetActive(true);
                }
                break;
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);
    }

    public override bool Request (GameObject objectNeeded)
    {
        if (objectNeeded != null)
        {
            if (objectNeeded.GetComponent<FoodBehaviour>() != null)
            {
                if (objectNeeded.GetComponent<FoodBehaviour>()._food == requestFood[0])
                {
                    return true;
                }
            }
        }

        return false;
    }
}
