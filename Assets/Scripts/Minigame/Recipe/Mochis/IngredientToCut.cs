using System.Collections.Generic;
using UnityEngine;

public class IngredientToCut : MonoBehaviour
{
    [SerializeField] private bool isInScene;

    private Rigidbody rb;
    private float gravity;

    [SerializeField] private GameObject restCut;

    private List <GameObject> restCutList;

    public bool _isInScene => isInScene;
    public List <GameObject> _restCutList => restCutList;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        gravity = rb.linearVelocity.z;

        restCutList = new List <GameObject>();
    }

    private void Update()
    {
        if (isInScene == true)
        {
            gravity = rb.linearVelocity.z - (9.8f * Time.deltaTime);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, gravity);
        }
    }

    private void OnBecameVisible() //Evento que se llama cuando el GameObject es visible por la cámara
    {
        isInScene = true;
    }

    private void OnBecameInvisible() //Evento que se llama cuando el GameObject ya no se ve por la cámara
    {
        isInScene = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gravity = 0;

        gameObject.SetActive(false); //TEMPORAL HASTA ARREGLAR EL POOL
    }

    public void CutIngredient()
    {
        if (restCut != null)
        {
            GameObject rest = Instantiate(restCut, transform.position, Quaternion.identity);

            foreach (Transform t in rest.transform)
            {
                t.gameObject.GetComponent<Rigidbody>().linearVelocity = rb.linearVelocity;
                Vector3 dir = t.transform.position - transform.position;
                t.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(dir.normalized, transform.position, ForceMode.Impulse);
            }

            restCutList.Add(rest);
        }
        gameObject.SetActive(false);
        OnBecameInvisible();
    }

}
