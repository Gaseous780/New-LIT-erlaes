using System.Collections;
using UnityEngine;

public class ReArrowBehaviour : MonoBehaviour
{
    private Vector3 positionOriginal;
    private float height;
    [SerializeField] private Transform positionToGo;

    [SerializeField] private float speed;
    private float progress;

    private GameObject player;

    private bool waitMovement;
    [SerializeField] private float waitTimeToDestroy = 0.5f;

    [SerializeField] private MiniGamesBase minigame;
    [SerializeField] private int direction;

    [SerializeField] private GameObject circleFeedback;

    private void Awake()
    {
        positionOriginal = transform.position;
        height = positionOriginal.y;
    }

    private void OnEnable()
    {
        transform.position = positionOriginal;
        waitMovement = false;

        circleFeedback.SetActive(true);

        IDancer dancer = minigame as IDancer;
        dancer.EnableArrowInfo(direction);
    }

    private void OnDisable()
    {
        progress = 0f;
        circleFeedback.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitMovement == false) { MoveArrow(); }
    }

    private void MoveArrow()
    {
        transform.position = Vector3.Lerp(positionOriginal, new Vector3(positionToGo.position.x, height, positionToGo.position.z), progress);
        progress += speed * Time.deltaTime;

        if (progress >= 1)
        {
            waitMovement = true;

            StartCoroutine(Waiting());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag ("Point") == true)
        {
            if (other.gameObject.GetComponent<PointOn>()._player != null)
            {
                player = other.gameObject.GetComponent<PointOn>()._player;
            }
            else
            {
                player = null;
            }
        }

        if (other.CompareTag("ReFryOther") == true && waitMovement == true)
        {
            DisableArrow();
            StopAllCoroutines();
        }
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(waitTimeToDestroy);

        DisableArrow();

    }

    private void DisableArrow()
    {
        IDancer dancer = minigame as IDancer;
        dancer.DisableArrowInfo(direction);

        gameObject.SetActive(false);
        progress = 0;
        circleFeedback.SetActive(false);

        if (player != null)
        {
            player.GetComponent<PointFry>().AddPoint(true);
        }
        else
        {
            minigame.Error();
        }
    }
}
