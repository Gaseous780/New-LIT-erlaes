using UnityEngine;
using UnityEngine.UI;

public class BeatBehaviour : MonoBehaviour
{
    [SerializeField] private Transform whisk;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Slider mixBar;
    [SerializeField] private float mixSpeed = 0.1f;
    [SerializeField] private float decaySpeed = 0.2f;
    [SerializeField] private float requiredRotation = 5f;
    [SerializeField] private int maxSuccesses = 3;
    private bool dragging;
    private Vector2 previousDirection;
    private float accumulatedRotation;
    private int completedMixes;
    private Camera cam;
    public System.Action onMixCompleted;
    [SerializeField] private Collider bowlArea; //CircleCollider
    private bool mixCompleted;
    private bool canMix = true;
    [SerializeField] private Collider whiskCollider;
    [SerializeField] private GameObject pivotObject;
    [SerializeField] private float rotationSpeed;
    private float whiskRadius;
    private float whiskHeight;

    [SerializeField] private Transform whiskHandle; // inicio del mango
    [SerializeField] private Transform whiskTip;    // punta del batidor

    private float forceMultiplier = 1f;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip[] mixSounds;
    [SerializeField] private float intervalBetweenMixSound;
    [SerializeField] private AudioClip[] progresSounds;
    [SerializeField] private float intervalBetweenProgresSound;
    [SerializeField] private AudioClip succesPointSound;
    private SoundManager soundManager;

    [SerializeField] private GameObject rendererOn;

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    private void OnEnable()
    {
        rendererOn.SetActive(false);
    }

    private void OnDisable()
    {
        rendererOn.SetActive(true);
    }

    private void Awake()
    {
        cam = Camera.main;

        mixBar.maxValue = 10f;
        mixBar.value = 0;
        whiskHeight = whisk.position.y;
        whiskRadius = Vector3.Distance(
        whisk.position,
        centerPoint.position) - 1f;
    }

    private void Update()
    {
        HandleDragging();

        DecayBar();
    }

    private void HandleDragging()
    {
        RaycastHit hit;
        if (!canMix)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider == whiskCollider)
            {
                dragging = true;

                Vector3 handleToTip = whiskTip.position - whiskHandle.position;
                Vector3 handleToClick = hit.point - whiskHandle.position;

                float gripPercent =
                    Vector3.Dot(handleToClick, handleToTip.normalized)
                    / handleToTip.magnitude;

                gripPercent = Mathf.Clamp01(gripPercent);

                forceMultiplier = Mathf.Lerp(0.5f, 2f, gripPercent);

                Vector3 mousePos = GetMouseWorldPosition();

                previousDirection = new Vector2(
                    mousePos.x - centerPoint.position.x,
                    mousePos.z - centerPoint.position.z
                );
            }
        }

        if (dragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Vector2 currentDirection = new Vector2(
    mousePos.x - centerPoint.position.x,
    mousePos.z - centerPoint.position.z
);

            soundManager.ReproduceChainSounds(mixSounds, intervalBetweenMixSound);

            float mouseDistance = currentDirection.magnitude;

            if (mouseDistance < whiskRadius * 0.7f)
            {
                previousDirection = currentDirection;
                return;
            }

            float angle =
                Vector2.SignedAngle(previousDirection, currentDirection);

            if (Mathf.Abs(angle) > 200f)
            {
                previousDirection = currentDirection;
                return;
            }

            accumulatedRotation += Mathf.Abs(angle);

            if (accumulatedRotation >= requiredRotation)
            {
                accumulatedRotation = 0;
                AddProgress();
            }

            previousDirection = currentDirection;
            Vector3 dir = mousePos - centerPoint.position;
            dir.y = 0f;
            float distanceToCenter = dir.magnitude;

            if (distanceToCenter < whiskRadius * 0.7f)
                return;
            if (dir.sqrMagnitude < 0.001f)
                return;

            dir.Normalize();

            Vector3 newPos =
                centerPoint.position +
                dir * whiskRadius;

            newPos.y = whiskHeight;

            whisk.position = Vector3.Lerp(
    whisk.position,
    newPos,
    5f * Time.deltaTime
);
        }
        whisk.LookAt(centerPoint);

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (!dragging)
            return;

        Ray ray2 = cam.ScreenPointToRay(Input.mousePosition);


        if (!Physics.Raycast(ray2, out hit) || hit.collider != bowlArea)
        {
            return;
        }


    }

    private void RotateWhisk(Vector2 dir)
    {
        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //whisk.rotation = Quaternion.Euler(0, 0, angle - 90f);

        transform.RotateAround(pivotObject.transform.position, new Vector3(0, -angle, 0), (rotationSpeed * angle) * Time.deltaTime);
    }

    private void AddProgress()
    {
        if (mixCompleted)
        {
            return;
        }

        //mixBar.value += mixSpeed * Time.deltaTime * 60f;
        mixBar.value += mixSpeed * forceMultiplier * Time.deltaTime * 60f;

        if (rendererOn.activeSelf == false) { rendererOn.SetActive(true); }

        soundManager.ReproduceChainSounds(progresSounds, intervalBetweenProgresSound);

        if (mixBar.value >= mixBar.maxValue)
        {
            mixBar.value = mixBar.maxValue;

            mixCompleted = true;

            onMixCompleted?.Invoke();

            soundManager.ReproduceSound(succesPointSound);
            soundManager.CancelChain(mixSounds[0]);
            soundManager.CancelChain(progresSounds[0]);
        }
    }
    public void SetMixState(bool state)
    {
        canMix = state;
    }
    private void DecayBar()
    {
        if (dragging)
            return;

        if (mixBar.value > 0)
        {
            mixBar.value -= decaySpeed * Time.deltaTime;
            soundManager.CancelChain(mixSounds[0]);
            soundManager.CancelChain(progresSounds[0]);

            if (mixBar.value < 0)
                mixBar.value = 0;
        }
    }

    public void SkillCheckSuccess()
    {
        completedMixes++;

        mixBar.value = 0;

        if (completedMixes >= maxSuccesses)
        {
            FinishMinigame();
        }
    }
    public void ResetMix()
    {
        mixBar.value = 0;

        accumulatedRotation = 0;

        dragging = false;

        mixCompleted = false;
    }
    private void FinishMinigame()
    {
    }
    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, centerPoint.position);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return whisk.position;
    }
}