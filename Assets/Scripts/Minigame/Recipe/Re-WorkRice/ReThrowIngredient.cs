using UnityEngine;

public class ReThrowIngredient : MonoBehaviour, ICanBeGrabbed
{
    private Vector3 originalPosition;
    private float positionY;

    [SerializeField] private MiniGamesBase minigameToAddThrow;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float positionYParticles;
    [SerializeField] private float positionGrabbed;

    [SerializeField] private float amountThrow;

    private bool canAdd;
    private bool isGrab;

    [SerializeField] private GameObject model;
    [SerializeField] private float positionYModel;
    [SerializeField] private float positionYModelGrabbed;

    [SerializeField] private float speedRotation;
    [SerializeField]private float rotationAdded;
    [SerializeField] private float amountNeededToRotate;
    private bool rotateInverse;

    private Quaternion baseRotationModel;

    [Header ("Sounds Section")]
    [SerializeField] private AudioClip soundGrab;
    [SerializeField] private AudioClip throwIngredient;
    private SoundManager soundManager;

    private void Awake()
    {
        originalPosition = transform.position;
        positionY = originalPosition.y;

        baseRotationModel = model.transform.rotation;
    }

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    private void OnEnable()
    {
        canAdd = false;
        isGrab = false;

        transform.position = originalPosition;
        model.GetComponent<MeshRenderer>().enabled = true;

        Drop();
    }

    private void OnDisable()
    {
        model.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        particles.transform.position = new Vector3 (transform.position.x, positionYParticles, transform.position.z);
        if (isGrab == false) { model.transform.position = new Vector3(transform.position.x, positionYModel, transform.position.z); }
        else { model.transform.position = new Vector3(transform.position.x, positionYModelGrabbed, transform.position.z); }
    }

    public void Grab (Vector3 position)
    {
        if (isGrab == false) { soundManager.ReproduceSound(soundGrab); }

        isGrab = true;

        if (canAdd == true)
        {
            if (rotateInverse == false)
            {
                model.transform.Rotate(0, speedRotation * Time.deltaTime, 0);
                rotationAdded += speedRotation * Time.deltaTime;

                if (rotationAdded >= amountNeededToRotate)
                {
                    rotateInverse = true;
                }
            }
            else
            {
                model.transform.Rotate(0, -speedRotation * Time.deltaTime, 0);
                rotationAdded -= speedRotation * Time.deltaTime;

                if (rotationAdded <= -amountNeededToRotate)
                {
                    rotateInverse = false;
                }
            }

            if (particles.isPlaying == false) 
            {
                particles.Play();
                soundManager.ReproduceSound(throwIngredient);
            }
            IAddedGradualIngredient gradualMinigame = minigameToAddThrow as IAddedGradualIngredient;
            gradualMinigame.AddThrowIngredient(amountThrow * Time.deltaTime);
        }
        else
        {
            particles.Stop();
        }
    }

    public void Drop()
    {
        isGrab = false;

        particles.Stop();
        transform.position = originalPosition;

        model.transform.rotation = baseRotationModel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Bowl>() != null)
        {
            canAdd = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Bowl>() != null)
        {
            canAdd = false;
        }
    }
}
