using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction moveAction;
    public float speed = 5f;
    public bool canMove = true;

    private Transform cameraTransform;
    private Quaternion lockedCameraRotation;
    private bool isUsingLockedRotation = false;

    private Vector2 initialInputAtLock;

    private SoundManager soundManager;

    [SerializeField] private ParticleSystem runParticles;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private float intervalTimeBetweenSounds;

    private void Awake()
    {
        moveAction.Enable();
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || !canMove || cameraTransform == null) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        //si se suelta la tecla, se desbloquea el movimiento
        if (moveInput.magnitude < 0.1f)
        {
            isUsingLockedRotation = false;
            soundManager.CancelChain(walkSounds[0]);
            runParticles.Stop();
            return;
        }

        //esto es para que se normalice el movimiento, si pasa de cierto angulo vuelve a la normalidad, en este caso 10 grados, asi se siente bien
        if (isUsingLockedRotation)
        {
            float inputDifference = Vector2.Angle(initialInputAtLock, moveInput);
            if (inputDifference > 10f) 
            {
                isUsingLockedRotation = false;
            }
        }

        Vector3 forward, right;

        if (isUsingLockedRotation)
        {
            forward = lockedCameraRotation * Vector3.forward;
            right = lockedCameraRotation * Vector3.right;
        }
        else
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
        }

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;
        //soundManager.ReproduceChainSounds(1, 7, 0.48f);
        soundManager.ReproduceChainSounds(walkSounds, intervalTimeBetweenSounds);
        if (runParticles.isPlaying == false) { runParticles.Play(); }

        if (moveDirection != Vector3.zero) RotatePlayer(moveDirection);
    }

    public void LockMovementDirection()
    {
        Vector2 currentInput = moveAction.ReadValue<Vector2>();
        if (currentInput.magnitude > 0.1f)
        {
            lockedCameraRotation = cameraTransform.rotation;
            initialInputAtLock = currentInput;
            isUsingLockedRotation = true;
        }
    }

    void RotatePlayer(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}