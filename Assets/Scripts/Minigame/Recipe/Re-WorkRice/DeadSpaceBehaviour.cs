using System.Collections.Generic;
using UnityEngine;

public class DeadSpaceBehaviour : MonoBehaviour
{
    [SerializeField] private List <int> directions;

    public List <int> _directions => directions;

    private void OnTriggerEnter(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null) { return; }

        if (directions.Contains(fry._directionToGo) == false && fry._isMoving == true && fry._isOnFirstBase == false && fry._isSafe == false && fry._isOnSecondBase == false)
        {
            fry.Succes(false);
            fry.SetReset(true);
            Debug.Log("Fallo deadspace normal");
        }
        else if (directions.Contains(fry._directionToGo) == true)
        {
            fry._isOnFirstBase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null) { return; }

        if (directions.Contains(fry._directionToGo) == true)
        {
            fry._isOnFirstBase = false;
        }

        if (fry._isOnSecondBase == false && fry._isOnFirstBase == false && fry._isSafe == false)
        {
            fry.Succes(false);
            fry.SetReset(true);
            Debug.Log("Fallo deadspace salida");
        }
    }
}
