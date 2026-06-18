using UnityEngine;

public class DirectionFryPan : MonoBehaviour
{
    [SerializeField] private int directionToGive; //0 = Arriba, 1 = Abajo, 2 = Derecha, 3 = Izquierda, 4 = Diagonal superior derecha, 5 = Diagonal superior izquierda, 6 = Diagonal inferior derecha, 7 = Diagonal inferior izquierda

    private Vector3 maxPositionToComplete; //A la hora de usar las posiciones y escalas se trabajaran con las locales. No usar las normales como .position, sino .localPosition

    public int _directionToGive => directionToGive;

    private void Awake()
    {
        DefineDirections();
    }

    private void DefineDirections() //Se debe de llamarse solo una vez a la hora de crear por primera vez las direcciones
    {
        switch (directionToGive)
        {
            case 0:
                maxPositionToComplete = new Vector3(0, 0, transform.position.z + transform.localScale.z / 2);
                break;

            case 1:
                maxPositionToComplete = new Vector3(0, 0, transform.position.z - transform.localScale.z / 2);
                break;

            case 2:
                maxPositionToComplete = new Vector3(transform.localPosition.x + transform.localScale.x, 0, 0);
                break;

            case 3:
                maxPositionToComplete = new Vector3(transform.localPosition.x - transform.localScale.x, 0, 0);
                break;

            case 4:
                maxPositionToComplete = new Vector3(transform.localPosition.z + transform.localScale.z / 2, 0, transform.localPosition.x + transform.localScale.x / 2);
                break;

            case 5:
                maxPositionToComplete = new Vector3(transform.localPosition.z + transform.localScale.z / 2, 0, transform.localPosition.x - transform.localScale.x / 2);
                break;

            case 6:
                maxPositionToComplete = new Vector3(transform.localPosition.z - transform.localScale.z / 2, 0, transform.localPosition.x + transform.localScale.x / 2);
                break;

            case 7:
                maxPositionToComplete = new Vector3(transform.localPosition.z - transform.localScale.z / 2, 0, transform.localPosition.x - transform.localScale.x / 2);
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null) { return; }

        if (fry._directionToGo == directionToGive)
        {
            fry._isOnSecondBase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null || fry._isOnFirstBase == true || fry._directionToGo != directionToGive) { return;}

        if (HasComplete(other.transform.localPosition, fry) == true)
        {
            fry.Succes(true);
        }
        else
        {
            fry.Succes(false);
            Debug.Log(fry._isOnFirstBase + "\n" + fry._isOnSecondBase + "\n" + gameObject);
        }

        fry._isOnSecondBase = false;
        fry.SetReset(true);
    }

    private bool HasComplete(Vector3 positionOfFry, ReFryPanBehaviour fry)
    {
        switch (directionToGive)
        {
            case 0:
                if (positionOfFry.z > transform.localPosition.z + transform.localScale.z / 4)
                {
                    return true;
                }
                break;

            case 1:
                if (positionOfFry.z < transform.localPosition.z - transform.localScale.z / 4)
                {
                    return true;
                }
                break;

            case 2:
                if (positionOfFry.x > transform.localPosition.x + transform.localScale.x / 4)
                {
                    return true;
                }
                break;

            case 3:
                if (positionOfFry.x < transform.localPosition.x - transform.localScale.x / 4)
                {
                    return true;
                }
                break;

            case 4:
                if (positionOfFry.z >= transform.localPosition.z && positionOfFry.x >= transform.localPosition.x)
                {
                    return true;
                }
                break;

            case 5:
                if (positionOfFry.z >= transform.localPosition.z && positionOfFry.x <= transform.localPosition.x)
                {
                    return true;
                }
                break;

            case 6:
                if (positionOfFry.z <= transform.localPosition.z && positionOfFry.x >= transform.localPosition.x)
                {
                    return true;
                }
                break;

            case 7:
                if (positionOfFry.z <= transform.localPosition.z && positionOfFry.x <= transform.localPosition.x)
                {
                    return true;
                }
                break;
        }

        return false;
    }
}
