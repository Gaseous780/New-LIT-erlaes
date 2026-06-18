using UnityEngine;

public class ScaleCompareTwoObjects : MonoBehaviour
{
    [SerializeField] private int axe;

    private Transform objectiveTransform;
    private Transform playerTransform;

    [SerializeField] private float maxScale;
    private Vector3 defaultScale;

    [SerializeField] private float amountToScale;

    private MeshRenderer model;

    private void Awake()
    {
        objectiveTransform = null;
        playerTransform = null;

        model = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        defaultScale = transform.localScale;
        model.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point") == true && objectiveTransform == null)
        {
            objectiveTransform = other.transform;
            return;
        }

        if (other.CompareTag("ReFryOther") == true && playerTransform == null)
        {
            playerTransform = other.transform;
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ReFryOther") == true && playerTransform != null && objectiveTransform != null)
        {
            SetScale();
            model.enabled = true;
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReFryOther") == true && playerTransform != null)
        {
            transform.localScale = defaultScale;
            model.enabled = false;
            return;
        }
    }


    public void SetScale()
    {
        transform.localScale = new Vector3(Mathf.Lerp(amountToScale, defaultScale.x, Mathf.Clamp(Vector3.Distance(playerTransform.position, objectiveTransform.position) / maxScale, 0,1)), defaultScale.y, defaultScale.z);

        switch (axe)
        {
            case 0:

                break;

            case 1:
                break;

            case 2:
                break;

            case 3:
                break;

            case 4:
                break;

            case 5:
                break;

            case 6:
                break;

            case 7:
                break;
        }
    }
}
