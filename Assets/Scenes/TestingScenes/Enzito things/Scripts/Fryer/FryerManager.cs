using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace TempuraKitchen.Minigames.WASDGrid
{
    public class FryerManager : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Transform pointerTransform;
        [SerializeField] private Transform targetZoneTransform;
        [SerializeField] private Image targetZoneImage;

        [Header("UI Text Components")]
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text successCounterText;

        [Header("Game Settings")]
        [SerializeField] private int totalRequiredSuccesses = 5;
        [SerializeField] private float baseRotationSpeed = 200f;
        [SerializeField] private float zoneSizeDegrees = 40f;

        private int currentSuccessCount = 0;
        private float currentSpeed;
        private bool isMinigameActive = false;
        private bool canInputInThisCheck = true;
        private bool isCountingDown = false;
        private bool isRotatingClockwise = true;

        [SerializeField] private MiniGamesBase minigameAchieved;
        [SerializeField] private float timeWait = 2f;

        [Header("Sounds Section")]
        [SerializeField] private AudioClip succesSound;
        [SerializeField] private AudioClip failSound;
        [SerializeField] private AudioClip[] counterSound;
        [SerializeField] private AudioClip goSound;
        private SoundManager soundManager;

        private void OnEnable()
        {
            StartCoroutine(Wait());
        }

        private void OnDisable()
        {
            GenerateNewSkillCheck();
            StopAllCoroutines();
        }

        private void Start()
        {
            soundManager = GameManager.instance._soundManager;
        }

        private IEnumerator Wait()
        {
            if (countdownText != null) countdownText.text = string.Empty;
            yield return new WaitForSeconds(timeWait);

            StartMinigame();
        }

        public void StartMinigame()
        {
            currentSuccessCount = 0;
            currentSpeed = baseRotationSpeed;
            isMinigameActive = false;
            canInputInThisCheck = false;
            isRotatingClockwise = true;

            UpdateSuccessCounterUI();

            if (countdownText != null) countdownText.gameObject.SetActive(false);

            StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountingDown = true;
            pointerTransform.localEulerAngles = Vector3.zero;

            GenerateNewSkillCheck();

            for (int i = 3; i > 0; i--)
            {
                if (countdownText != null)
                {
                    countdownText.gameObject.SetActive(true);
                    countdownText.text = i.ToString();

                    if (soundManager == null) { soundManager = GameManager.instance._soundManager; }
                    soundManager.ReproduceSound(counterSound[i]);
                    
                }
                yield return new WaitForSeconds(1f);
            }

            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
                soundManager.ReproduceSound(goSound);
            }

            isMinigameActive = true;
            canInputInThisCheck = true;
            isCountingDown = false;
        }

        private void Update()
        {
            if (!isMinigameActive || isCountingDown) return;

            float direction = isRotatingClockwise ? -1f : 1f;
            pointerTransform.Rotate(Vector3.forward, direction * currentSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) && canInputInThisCheck)
            {
                CheckPlayerInput();
            }
        }

        private void GenerateNewSkillCheck()
        {
            canInputInThisCheck = true;

            if (targetZoneImage != null)
            {
                targetZoneImage.fillAmount = zoneSizeDegrees / 360f;
            }

            float randomAngle = Random.Range(0f, 360f);
            targetZoneTransform.localEulerAngles = new Vector3(0, 0, randomAngle);
        }

        private void CheckPlayerInput()
        {
            canInputInThisCheck = false;

            float pointerAngle = (360f - (pointerTransform.localEulerAngles.z % 360f)) % 360f;
            float zoneRotation = (360f - (targetZoneTransform.localEulerAngles.z % 360f)) % 360f;
            float visualZoneCenter = (zoneRotation + (zoneSizeDegrees / 2f)) % 360f;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(pointerAngle, visualZoneCenter));

            bool isSuccess = angleDifference <= (zoneSizeDegrees / 2f);

            if (isSuccess)
            {
                soundManager.ReproduceSound(succesSound);

                currentSuccessCount++;
                UpdateSuccessCounterUI();

                if (minigameAchieved is ReFryerBehaviour fryerBehaviour)
                {
                    fryerBehaviour.TempurasJump();
                }

                if (currentSuccessCount >= totalRequiredSuccesses)
                {
                    WinMinigame();
                }
                else
                {
                    currentSpeed += 30f;
                    isRotatingClockwise = !isRotatingClockwise;
                    GenerateNewSkillCheck();
                }
            }
            else
            {
                FailMinigame();
            }
        }

        private void UpdateSuccessCounterUI()
        {
            if (successCounterText != null)
            {
                successCounterText.text = $"{currentSuccessCount} / {totalRequiredSuccesses}";
            }
        }

        private void WinMinigame()
        {
            isMinigameActive = false;

            if (minigameAchieved is ReFryerBehaviour fryerBehaviour)
            {
                gameObject.SetActive(false);
                fryerBehaviour.EndWithWinAnimation();
            }
            else
            {
                minigameAchieved.StopMinigame(true);
            }
        }

        private void FailMinigame()
        {
            soundManager.ReproduceSound(failSound);

            isMinigameActive = false;
            if (minigameAchieved.Error() == true) { return; }
            StartCoroutine(CountdownCoroutine());
        }
    }
}