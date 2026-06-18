using System.Collections;
using TMPro;
using UnityEngine;

public class TakoyakiBehaviourPart1 : MiniGamesBase, IMovePlayerNodes, IFryer
{
    [SerializeField] private GameObject[] objectsPhase1; //GameObjects que se usaen la fase 1
    [SerializeField] private GameObject[] objectsPhase2; //GameObjects que se usaen la fase 2

    [SerializeField] private PlayerOnNode playerNode;

    [SerializeField] private TemporGrid[] posiblesGrids;
    private TemporGrid actualGrid;

    private int counterCompleteTempor;
    private int temporRuning;

    [SerializeField] private FryerBehaviour fryer;

    private int[] pointsOnFry;
    private int totalFrys;

    private int finishValueOfFood;

    [SerializeField] private TwinkleBehaviour flag;
    [SerializeField] private TextMeshProUGUI counterText;

    [SerializeField] private GameObject takoyakiToSpawn;
    [SerializeField] private Transform spawnPosition;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        actualPhase = 0;
        errorCounters = 0;

        counterCompleteTempor = 0;
        temporRuning = 1;
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
        foreach (GameObject objects in objectsPhase1)
        {
            objects.SetActive(true);
        }

        UpdateUI();

        ChooseGrid();

        actualGrid.SetPlayerOnInitial(playerNode);

        flag._positionToGet = actualGrid._endNode.transform;
        flag.gameObject.SetActive(true);
        flag.StartInit();
    }

    public override void PhaseTwo()
    {
        foreach (GameObject objects in objectsPhase1)
        {
            objects.SetActive(false);
        }

        foreach (GameObject objects in objectsPhase2)
        {
            objects.SetActive(true);
        }

        miniGameControllers.ChangeControl(controlsToUse[actualPhase]);

        UpdateUI();
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
            takoyaki.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.TakoyakiIncomplete, finishValueOfFood, "TakoyakiIncomplete"); //Se accede al componente de comida para asignale sus valores.
            takoyaki.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction> ()._heldObject = takoyaki;
        }

        GameManager.instance._miniGameManager.StopMiniGameExecute();
    }

    public void ChooseGrid()
    {
        temporRuning = 1;

        int grid = Random.Range(0, posiblesGrids.Length);

        actualGrid = posiblesGrids[grid];
        ActivateGrid();
    }

    public void ActivateGrid()
    {
        actualGrid.SetTransitionalsNodes(true);
    }

    public void DesactivateGrid()
    {
        actualGrid.SetTransitionalsNodes(false);
    }

    public void MovePlayer(string input)
    {
        if (playerNode._onNode._neighborsNodes.ContainsKey(input) == true && actualGrid._transitionalsNodes.Contains(playerNode._onNode._neighborsNodes[input]))
        {
            if (actualGrid._transitionalsNodes.IndexOf(playerNode._onNode._neighborsNodes[input]) != temporRuning) { return; }

            temporRuning++;

            TemporNode endNode = null;

            if (playerNode._onNode._neighborsNodes[input] == actualGrid._endNode)
            {
                endNode = actualGrid._endNode;
            }

            playerNode.MoveTo(playerNode._onNode._neighborsNodes[input]);

            if (actualGrid.CheckPlayerNodePosition(playerNode._onNode) == actualGrid._counterArrow && endNode == null)
            {
                actualGrid.EnableArrow();
            }

            if (actualGrid.CheckComplete() == true && endNode != null)
            {
                if (counterCompleteTempor < 2)
                {
                    counterCompleteTempor++;
                    UpdateUI();
                    DesactivateGrid();
                    PhaseOne();
                }
                else
                {
                    DesactivateGrid();
                    StopMinigame(true);
                }
            }
        }
    }

    public void FryActionDown()
    {
        fryer.HandDown();
    }

    public void FryActionUp()
    {
        float score = fryer.HandUp();

        pointsOnFry[totalFrys] = (int)score;
        totalFrys++;
        UpdateUI();

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
                foreach (GameObject objectOfMinigame in objectsPhase1)
                {
                    objectOfMinigame.SetActive(false);
                }

                DesactivateGrid();
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

                PhaseOne();
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

    public void UpdateUI() //Para actualizar el contador en la UI
    {
        switch (actualPhase)
        {
            case 0:
                counterText.text =  counterCompleteTempor.ToString() + "/3";
                break;

            case 1:
                counterText.text = totalFrys.ToString() + "/3";
                break;
        }
    }
}
