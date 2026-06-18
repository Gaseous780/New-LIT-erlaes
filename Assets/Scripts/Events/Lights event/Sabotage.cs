using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

public class Sabotage : MonoBehaviour
{
    [Header("Tablero Interaction")]
    public GameObject entryPanel;

    [Header("References")]
    public GameObject saboteurPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public Light generalLight;
    public PanelManager panelManager;

    [Header("Settings")]
    public float movementSpeed = 4f;
    public float lightFadeDuration = 1.2f;

    [Header("Timers")]
    public float delayFirstSabotage = 60f;
    public float minWaitTime = 60f;
    public float maxWaitTime = 90f;

    private GameObject currentSaboteur;
    private bool sabotageInProgress = false;
    private Coroutine lightCoroutine;
    private Animator animator;
    public GameObject canvasElements;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip laughSound;
    [SerializeField] private AudioClip powerOffSound;
    private SoundManager soundManager;

    [SerializeField] private Interaction interaction;
    [SerializeField] private GameObject silhoutte;

    private void OnEnable()
    {
        float nextWait = Random.Range(minWaitTime, maxWaitTime);
        StartCoroutine(SabotageTimer(nextWait));
    }

    private void Start()
    {
        float nextWait = Random.Range(minWaitTime, maxWaitTime);
        StartCoroutine(SabotageTimer(nextWait));

        soundManager = GameManager.instance._soundManager;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TriggerSabotage();
        }
    }
    IEnumerator SabotageTimer(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        TriggerSabotage();
    }

    public void TriggerSabotage()
    {
        if (sabotageInProgress) return;

        sabotageInProgress = true;

        interaction.SetPipeline(5);
        silhoutte.SetActive(true);

        soundManager.ReproduceSound(laughSound);
        if (saboteurPrefab != null && spawnPoint != null)
        {
            currentSaboteur = Instantiate(saboteurPrefab, spawnPoint.position, spawnPoint.rotation);
            animator = currentSaboteur.GetComponentInChildren<Animator>();
            StartCoroutine(SabotageSequence());
        }
    }

    IEnumerator SabotageSequence()
    {
        animator.SetInteger("State", 1);
        while (Vector3.Distance(currentSaboteur.transform.position, targetPoint.position) > 0.1f)
        {
            currentSaboteur.transform.position = Vector3.MoveTowards(currentSaboteur.transform.position, targetPoint.position, movementSpeed * Time.deltaTime);
            yield return null;
        }

        currentSaboteur.transform.position = targetPoint.position;
        animator.SetInteger("State", 0);
        yield return new WaitForSeconds(0.4f);

        if (panelManager != null) panelManager.currentStep = PanelStep.ManualMode;
        canvasElements.SetActive(false);

        if (entryPanel != null)
        {
            entryPanel.GetComponent<Collider>().enabled = true;
        }

        yield return StartCoroutine(FadeLight(0f));

        soundManager.ReproduceSound(powerOffSound);
        Destroy(currentSaboteur, 0.5f);
        
    }

    public void RestoreLights()
    {
        StartCoroutine(FadeLight(1f));

        //silhoutte.SetActive(false);
        interaction.SetPipeline(0);
        sabotageInProgress = false;

        canvasElements.SetActive(true);
        if (entryPanel != null)
        {
            entryPanel.GetComponent<Collider>().enabled = false;
        }

        float nextWait = Random.Range(minWaitTime, maxWaitTime);
        StartCoroutine(SabotageTimer(nextWait));
    }

    IEnumerator FadeLight(float targetIntensity)
    {
        if (generalLight == null) yield break;

        if (lightCoroutine != null) StopCoroutine(lightCoroutine);

        float elapsed = 0;
        float startIntensity = generalLight.intensity;

        while (elapsed < lightFadeDuration)
        {
            elapsed += Time.deltaTime;
            generalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / lightFadeDuration);
            yield return null;
        }

        generalLight.intensity = targetIntensity;
        lightCoroutine = null;
    }
}
