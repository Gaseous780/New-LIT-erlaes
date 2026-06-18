using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class FireExtinguisherBehaviour : ItemsBase
{
    [SerializeField] private float distanceOfExtinguish = 3;
    [SerializeField] private float rayRadius = 3f;
    [SerializeField] private float up = 1.2f;
    [SerializeField] private float forward = 0.5f;
    [Header("References")]
    private ParticleSystem particles;

    [SerializeField] private Transform playerTransform;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] shootSound;
    [SerializeField] private float intervalBetweenSound;
    [SerializeField] private AudioClip[] clientsScreamsSounds;
    private bool dontScream;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();

        dontScream = false;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void OnGrabbed()
    {
        base.OnGrabbed();
    }

    public override void OnDrop()
    {
        base.OnDrop();
    }

    public override void Use()
    {
        particles.transform.forward = playerTransform.forward;
        particles.Emit(1);
        soundManager.ReproduceChainSounds(shootSound, intervalBetweenSound);
        RaycastHit ray;
        Vector3 origin = playerTransform.position + Vector3.up * up - Vector3.forward * forward;

        if (Physics.SphereCast(origin, rayRadius, playerTransform.forward, out ray, distanceOfExtinguish))
        {
            GameObject hitObject = ray.collider.gameObject;
            FireBehaviour fire = hitObject.GetComponent<FireBehaviour>();
            //FireBehaviour fire = ray.collider.GetComponentInParent<FireBehaviour>();

            ///////////Debug.Log(ray.collider.name);

            if (fire != null)
            {
                fire.ExtinguishFire();
            }
            ClientBehaviour client = hitObject.GetComponent<ClientBehaviour>();
            if (client != null)
            {
                if (dontScream == false)
                {
                    int scream = Random.Range(0, clientsScreamsSounds.Length - 1);
                    soundManager.ReproduceSound(clientsScreamsSounds[scream]);

                    dontScream = true;

                    StartCoroutine(EnableScream());
                }

                if (client.IsAngry || client.IsBroken)
                {
                    client.KillShakuza();
                }
                else
                {
                    Rigidbody rb = client.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    if (rb != null)
                    {
                        Vector3 pushDirection = playerTransform.forward;
                        float pushForce = 2f;

                        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    }

                    client.Invoke("DestroyClient", 0.2f);
                    return;
                }
            }
        }
    }

    public override void CancelUse()
    {
        soundManager.CancelChain(shootSound[0]);
    }

    private IEnumerator EnableScream()
    {
        yield return new WaitForSeconds(3f);

        dontScream = false;
    }
}
