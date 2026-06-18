using Unity.VisualScripting;
using UnityEngine;

public class MovementConstantBehaviour : MonoBehaviour
{
    [SerializeField] private float timeToMove;
    private float timing;

    [SerializeField] private Vector3 amongToMove;
    [SerializeField] private Vector3 amongToScale;

    private bool hasMove;

    private Vector3 defaultScale;

    [SerializeField] private bool isScaling = false;

    private void Awake()
    {
        hasMove = false;

        defaultScale = transform.localScale;
    }

    void Update()
    {
        timing += Time.deltaTime;

        if (isScaling == false)
        {
            Moving();
        }
        else
        {
            Scaling();
        }
    }

    private void Moving()
    {
        if (timing >= timeToMove)
        {
            timing = 0;
            if (hasMove == false)
            {
                transform.position += amongToMove;
                hasMove = true;
            }
            else
            {
                transform.position -= amongToMove;
                hasMove = false;
            }
        }
    }

    private void Scaling()
    {
        if (timing >= timeToMove)
        {
            timing = 0;
            if (hasMove == false)
            {
                transform.localScale = amongToScale;
                hasMove = true;
            }
            else
            {
                transform.localScale = defaultScale;
                hasMove = false;
            }
        }
    }
}
