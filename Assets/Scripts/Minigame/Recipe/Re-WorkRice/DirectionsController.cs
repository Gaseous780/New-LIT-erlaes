using System.Collections.Generic;
using UnityEngine;

public class DirectionsController : MonoBehaviour
{
    [SerializeField] private GameObject holderDirectionals;
    [SerializeField] private GameObject holderDeadSpaces;

    private List<DirectionFryPan> allDirectionals;
    private List<DeadSpaceBehaviour> allDeadSpaces;

    private void Awake()
    {
        allDirectionals = new List<DirectionFryPan>();
        allDeadSpaces = new List<DeadSpaceBehaviour>();
    }

    void Start()
    {
        GetDirections();
    }

    public void GetDirections()
    {
        foreach (Transform objectChild in holderDirectionals.transform)
        {
            if (objectChild.gameObject.GetComponent<DirectionFryPan>() != null) { allDirectionals.Add(objectChild.gameObject.GetComponent<DirectionFryPan>()); }
        }

        foreach (Transform objectChild in holderDeadSpaces.transform)
        {
            if (objectChild.gameObject.GetComponent<DeadSpaceBehaviour>() != null) { allDeadSpaces.Add(objectChild.gameObject.GetComponent<DeadSpaceBehaviour>()); }
        }

        gameObject.SetActive(false);
    }

    public void SetVisuals(int directionToRenderer)
    {
        foreach (DirectionFryPan directions in allDirectionals)
        {
            if (directions._directionToGive == directionToRenderer)
            {
                //directions.gameObject.GetComponent<MeshRenderer>().enabled = true;
                directions.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            else
            {
                //directions.gameObject.GetComponent<MeshRenderer>().enabled = false;
                directions.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }

        foreach (DeadSpaceBehaviour spaces in allDeadSpaces)
        {
            if (spaces._directions.Contains(directionToRenderer) == true)
            {
                spaces.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                spaces.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
