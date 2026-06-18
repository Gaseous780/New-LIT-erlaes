using System.Collections.Generic;
using UnityEngine;

public class TemporNode : MonoBehaviour
{
    [SerializeField] private Dictionary<string, TemporNode> neighborsNodes; //El string es la posición del vecino según el input del jugador. Ejemplo: w (el de arriba), sd (abajo a la derecha) 

    [SerializeField] private bool isInitial = false;
    [SerializeField] private bool isEnd = false;

    private bool isTransitionable;
    [SerializeField] private bool hasBeenTransitioned;

    private TemporGrid grid;

    private Material defaultMaterial;
    private Material onTemporMaterial;
    private Material transitionalMaterial;
    private Material endMaterial;

    private MeshRenderer render;

    public Dictionary<string, TemporNode> _neighborsNodes => neighborsNodes;

    public bool _hasBeenTransitioned { get { return hasBeenTransitioned; } set { hasBeenTransitioned = value; } }
    public bool _isEnd { get { return isEnd; } set { isEnd = value; } }

    public void Awake()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        grid = GetComponentInParent<TemporGrid>();
        render = GetComponent<MeshRenderer>();
        onTemporMaterial = grid._temporMaterial;
        transitionalMaterial = grid._transitionableMaterial;
        endMaterial = grid._endMaterial;

        isTransitionable = false;
        hasBeenTransitioned = false;
    }

    private void Start()
    {
        neighborsNodes = new Dictionary<string, TemporNode>();

        RaycastHit hit; //Cada raycast que se va a realizar es para identificar si tiene un vecino en una de las 8 direcciones. De tenerlo se agrega al diccionario

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            neighborsNodes.Add("w", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
        {
            neighborsNodes.Add("a", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
        {
            neighborsNodes.Add("d", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, -transform.forward, out hit))
        {
            neighborsNodes.Add("s", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.left + transform.forward, out hit))
        {
            neighborsNodes.Add("wa", hit.collider.gameObject.GetComponent<TemporNode>());
            neighborsNodes.Add("aw", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.right + transform.forward, out hit))
        {
            neighborsNodes.Add("wd", hit.collider.gameObject.GetComponent<TemporNode>());
            neighborsNodes.Add("dw", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.left - transform.forward, out hit))
        {
            neighborsNodes.Add("sa", hit.collider.gameObject.GetComponent<TemporNode>());
            neighborsNodes.Add("as", hit.collider.gameObject.GetComponent<TemporNode>());
        }

        if (Physics.Raycast(transform.position, Vector3.right + -transform.forward, out hit))
        {
            neighborsNodes.Add("sd", hit.collider.gameObject.GetComponent<TemporNode>());
            neighborsNodes.Add("ds", hit.collider.gameObject.GetComponent<TemporNode>());
        }
    }

    public void SetNodeStatus(bool status, bool isEnd)
    {
        isTransitionable = status;
        this.isEnd = isEnd;

        if (isEnd == true)
        {
            render.material = endMaterial;
            return;
        }

        if (isTransitionable == true)
        {
            render.material = transitionalMaterial;
        }
        else
        {
            render.material = defaultMaterial;
        }
    }

    public void WasTransitioned(bool status)
    {
        if (isEnd == true)
        {
            hasBeenTransitioned = true;
            return;
        }

        hasBeenTransitioned = status;

        if (hasBeenTransitioned == false)
        {
            render.material = transitionalMaterial;
        }
        else
        {
            render.material = onTemporMaterial;
        }
    }
}
