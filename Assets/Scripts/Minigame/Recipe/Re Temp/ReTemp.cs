using System.Collections;
using UnityEngine;

public class ReTemp : MiniGamesBase, IReachPoint
{
    [SerializeField] private GameObject[] phaseOneObjects;

    [SerializeField] private GameObject takoyakiToSpawn;
    [SerializeField] private Transform spawnPosition;

    private void OnEnable()
    {
        StartCoroutine(InitWithDelay());
    }

    public override IEnumerator InitWithDelay()
    {
        PhaseOne();

        yield return null;
    }

    public override void PhaseOne()
    {
        foreach (GameObject objectPhase1 in phaseOneObjects)
        {
            objectPhase1.SetActive(true);
        }
    }

    public override void StopMinigame(bool succes)
    {
        foreach (GameObject objectPhase1 in phaseOneObjects)
        {
            objectPhase1.SetActive(false);
        }

        if (succes == true)
        {
            GameObject takoyaki = Instantiate(takoyakiToSpawn, spawnPosition.position, spawnPosition.rotation); //Se crea la comida realizada en el minijuego en el mundo del juego, con el transform del spawn
            takoyaki.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.TakoyakiIncomplete, 0, "TakoyakiIncomplete"); //Se accede al componente de comida para asignale sus valores.
            takoyaki.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction>()._heldObject = takoyaki;
        }

        GameManager.instance._miniGameManager.StopMiniGameExecute();
    }

    public void Complete()
    {
        StopMinigame(true);
    }

    public override void PauseMinigame()
    {
        switch (actualPhase)
        {
            case 0:
                foreach (GameObject objectOfMinigame in phaseOneObjects)
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
                foreach (GameObject objectOfMinigame in phaseOneObjects)
                {
                    objectOfMinigame.SetActive(true);
                }

                PhaseOne();
                break;
        }
    }

    public override bool Request(GameObject objectNeeded)
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
