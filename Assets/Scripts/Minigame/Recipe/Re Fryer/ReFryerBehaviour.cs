using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReFryerBehaviour : MiniGamesBase
{
    [Header("Phase Setup")]
    [SerializeField] private GameObject[] phaseOneObjects;

    [Header("Connected Systems")]
    [SerializeField] private TempuraKitchen.Minigames.WASDGrid.FryerManager fryerManagerScript;
    [SerializeField] private FireBehaviour fireBehaviour;

    [Header("Animation Settings")]
    [SerializeField] private Transform basketTransform;
    [SerializeField] private GameObject[] tempuraPrefabs;
    [SerializeField] private Transform tempuraSpawnPoint;

    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float delayBetweenDrops = 0.3f;
    [SerializeField] private float timeWaitingToSettle = 0.8f;
    [SerializeField] private float basketLowerDuration = 1.2f;
    [SerializeField] private float basketLowerDistance = 0.5f;
    [SerializeField] private float jumpForce = 2.5f;

    [Header("End Reward Setup")]
    [SerializeField] private GameObject takoyakiToSpawn;
    [SerializeField] private Transform spawnPosition;

    private int errorCounter;
    private Vector3 basketInitialLocalPos;
    private List<GameObject> spawnedTempuras = new List<GameObject>();

    private void Awake()
    {
        if (basketTransform != null)
        {
            basketInitialLocalPos = basketTransform.localPosition;
        }
    }

    private void OnEnable()
    {
        errorCounter = 0;

        if (basketTransform != null) basketTransform.localPosition = basketInitialLocalPos;
        if (fryerManagerScript != null) fryerManagerScript.gameObject.SetActive(false);

        ClearSpawnedTempuras();

        StartCoroutine(InitWithDelay());
    }

    public override IEnumerator InitWithDelay()
    {
        PhaseOne();

        yield return StartCoroutine(AnimateIntroduction());

        if (fryerManagerScript != null)
        {
            fryerManagerScript.gameObject.SetActive(true);
        }
    }

    public override void PhaseOne()
    {
        foreach (GameObject objectPhase1 in phaseOneObjects)
        {
            objectPhase1.SetActive(true);
        }
    }

    private IEnumerator AnimateIntroduction()
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < tempuraPrefabs.Length; i++)
        {
            if (tempuraPrefabs[i] != null)
            {
                Vector3 spawnPos = tempuraSpawnPoint != null ? tempuraSpawnPoint.position : basketTransform.position + Vector3.up * 1.5f;
                Vector3 randomSpawnOffset = new Vector3(Random.Range(-0.05f, 0.05f), 0, Random.Range(-0.05f, 0.05f));

                GameObject tempuraClone = Instantiate(tempuraPrefabs[i], spawnPos + randomSpawnOffset, Quaternion.identity);
                spawnedTempuras.Add(tempuraClone);

                Rigidbody rb = tempuraClone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
            }

            yield return new WaitForSeconds(delayBetweenDrops);
        }

        yield return new WaitForSeconds(timeWaitingToSettle);

        foreach (GameObject tempura in spawnedTempuras)
        {
            if (tempura != null)
            {
                tempura.transform.SetParent(basketTransform);
            }
        }

        if (basketTransform != null)
        {
            Vector3 startLocalPos = basketTransform.localPosition;
            Vector3 targetLocalPos = startLocalPos + Vector3.back * basketLowerDistance;

            float elapsed = 0f;
            while (elapsed < basketLowerDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / basketLowerDuration;
                basketTransform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            basketTransform.localPosition = targetLocalPos;
        }
    }

    public void TempurasJump()
    {
        foreach (GameObject tempura in spawnedTempuras)
        {
            if (tempura != null)
            {
                Rigidbody rb = tempura.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 randomForceOffset = new Vector3(Random.Range(-0.1f, 0.1f), 1f, Random.Range(-0.1f, 0.1f));
                    rb.AddForce(randomForceOffset.normalized * jumpForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void EndWithWinAnimation()
    {
        StartCoroutine(AnimateOutroAndWin());
    }

    private IEnumerator AnimateOutroAndWin()
    {
        //foreach (GameObject tempura in spawnedTempuras)
        //{
        //    if (tempura != null)
        //    {
        //        Rigidbody rb = tempura.GetComponent<Rigidbody>();
        //        if (rb != null) rb.isKinematic = true;
        //    }
        //}

        if (basketTransform != null)
        {
            Vector3 startLocalPos = basketTransform.localPosition;
            Vector3 targetLocalPos = basketInitialLocalPos;

            float elapsed = 0f;
            while (elapsed < basketLowerDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / basketLowerDuration;
                basketTransform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            basketTransform.localPosition = targetLocalPos;
        }

        yield return new WaitForSeconds(0.2f);

        StopMinigame(true);
    }

    public override void StopMinigame(bool succes)
    {
        foreach (GameObject objectPhase1 in phaseOneObjects)
        {
            objectPhase1.SetActive(false);
        }

        if (fryerManagerScript != null) fryerManagerScript.gameObject.SetActive(false);

        ClearSpawnedTempuras();

        if (succes == true)
        {
            GameObject takoyaki = Instantiate(takoyakiToSpawn, spawnPosition.position, spawnPosition.rotation);
            takoyaki.GetComponent<FoodBehaviour>().SetFood(TypeOfFoods.Takoyaki, 0, "Takoyaki");
            takoyaki.GetComponent<Collider>().enabled = false;
            GameManager.instance._player.GetComponent<Interaction>()._heldObject = takoyaki;
        }
        else
        {
            fireBehaviour.gameObject.SetActive(true);
            fireBehaviour.EnableConsecuences();
        }

        GameManager.instance._miniGameManager.StopMiniGameExecute();
    }

    private void ClearSpawnedTempuras()
    {
        foreach (GameObject tempura in spawnedTempuras)
        {
            if (tempura != null) Destroy(tempura);
        }
        spawnedTempuras.Clear();
    }

    public override void PauseMinigame()
    {
        if (fryerManagerScript != null) fryerManagerScript.gameObject.SetActive(false);
        foreach (GameObject objectOfMinigame in phaseOneObjects)
        {
            objectOfMinigame.SetActive(false);
        }
    }

    public override void ResumeMinigame()
    {
        foreach (GameObject objectOfMinigame in phaseOneObjects)
        {
            objectOfMinigame.SetActive(true);
        }
        if (fryerManagerScript != null) fryerManagerScript.gameObject.SetActive(true);
    }

    public override bool Request(GameObject objectNeeded)
    {
        if (objectNeeded != null && objectNeeded.GetComponent<FoodBehaviour>() != null)
        {
            if (objectNeeded.GetComponent<FoodBehaviour>()._food == requestFood[0])
            {
                return true;
            }
        }
        return false;
    }
}