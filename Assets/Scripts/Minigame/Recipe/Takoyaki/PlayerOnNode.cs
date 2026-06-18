using UnityEngine;

public class PlayerOnNode : MonoBehaviour
{
    [SerializeField] private TemporNode onNode;

    private bool isMoving;
    private Vector3 positionToMove;
    private float progres;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float speedRotation = 5f;

    [SerializeField] private GameObject camaron;
    [SerializeField] private GameObject visual;

    public TemporNode _onNode => onNode;

    private void Awake()
    {
        isMoving = false;
        positionToMove = Vector3.zero;
        progres = 0;
    }

    private void Update()
    {
        if (isMoving == true && positionToMove != Vector3.zero)
        {
            camaron.transform.position = new Vector3(Mathf.Lerp(camaron.transform.position.x, positionToMove.x, progres), camaron.transform.position.y, Mathf.Lerp(camaron.transform.position.z, positionToMove.z, progres));

            progres += speed * Time.deltaTime;
            progres = Mathf.Clamp(progres, 0f, 1.1f);
            camaron.transform.Rotate(speedRotation * Time.deltaTime, 0, 0);

            if (progres >= 0.2f)
            {
                isMoving = false;
                positionToMove = Vector3.zero ;
                progres = 0;
            }
        }
    }

    public void MoveTo (TemporNode nodeToMove)
    {
        if (nodeToMove._hasBeenTransitioned == false)
        {
            nodeToMove.WasTransitioned(true);
        }
        else
        {
            nodeToMove.WasTransitioned(false);
        }

        SetOnNode(nodeToMove);

        if (isMoving == true)
        {
            camaron.transform.position = new Vector3 (positionToMove.x, camaron.transform.position.y, positionToMove.z);
        }

        visual.transform.position = new Vector3 (nodeToMove.transform.position.x, transform.position.y, nodeToMove.transform.position.z);
        positionToMove = new Vector3(nodeToMove.transform.position.x, transform.position.y, nodeToMove.transform.position.z);

        //SettingForward(positionToMove);

        isMoving = true;
    }

    public void SetOnNode(TemporNode nodeToMove)
    {
        onNode = nodeToMove;
    }

    public void InstantMove (TemporNode nodeToMove)
    {
        if (nodeToMove._hasBeenTransitioned == false)
        {
            nodeToMove.WasTransitioned(true);
        }
        else
        {
            nodeToMove.WasTransitioned(false);
        }

        SetOnNode(nodeToMove);

        isMoving = false;
        camaron.transform.position = new Vector3(nodeToMove.transform.position.x, camaron.transform.position.y, nodeToMove.transform.position.z);
        visual.transform.position = new Vector3(nodeToMove.transform.position.x, visual.transform.position.y, nodeToMove.transform.position.z);
    }

    public void SettingForward (Vector3 positionToGo)
    {
        if (camaron.transform.position.x > positionToGo.x && camaron.transform.position.z == positionToGo.z)
        {
            camaron.transform.forward = Vector3.up;
            return;
        }
        else if (camaron.transform.position.x < positionToGo.x && camaron.transform.position.z == positionToGo.z)
        {
            camaron.transform.forward = Vector3.down;
            return;
        }
        else if (camaron.transform.position.x == positionToGo.x && camaron.transform.position.z < positionToGo.z)
        {
            camaron.transform.forward = Vector3.right;
            return;
        }
        else if (camaron.transform.position.x == positionToGo.x && camaron.transform.position.z > positionToGo.z)
        {
            camaron.transform.forward = Vector3.left;
            return;
        }
        else if (camaron.transform.position.x > positionToGo.x && camaron.transform.position.z < positionToGo.z)
        {
            camaron.transform.forward = Vector3.left + Vector3.down;
            return;
        }
        else if (camaron.transform.position.x < positionToGo.x && camaron.transform.position.z < positionToGo.z)
        {
            camaron.transform.forward = Vector3.right + Vector3.down;
            return;
        }
        else if (camaron.transform.position.x > positionToGo.x && camaron.transform.position.z > positionToGo.z)
        {
            camaron.transform.forward = Vector3.left + Vector3.up;
            return;
        }
        else if (camaron.transform.position.x < positionToGo.x && camaron.transform.position.z > positionToGo.z)
        {
            camaron.transform.forward = Vector3.right + Vector3.up;
            return;
        }
    }
}
