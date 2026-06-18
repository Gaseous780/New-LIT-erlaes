using System.Collections;
using UnityEngine;

public class CameraWorldChange : MonoBehaviour
{
    [SerializeField] private int cameraToChange;

    private bool decreaseCamera;
    private bool timeEnable;

    [SerializeField] private Transform[] tps; //0 para el -- y 1 ++ 

    [SerializeField] private CameraWorldChange brother;

    private void Awake()
    {
        decreaseCamera = false;
        timeEnable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeEnable == false && other.CompareTag ("Player") == true)
        {
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.LockMovementDirection();
            }
            ChangeValues(other.gameObject);
            Camera.main.GetComponent<CameraManager>().ChangeCameraSimple(cameraToChange);

            if (cameraToChange == 1 && TutorialManager.instance != null)
            {
                TutorialManager.instance.OnActionTriggered(TutorialManager.TutorialStep.TakeOrder);
            }

            timeEnable = true;
            StartCoroutine(EnableCamera());
        }
    }

    private IEnumerator EnableCamera()
    {
        yield return new WaitForSeconds(0.1f);

        timeEnable = false;
    }

    public void ChangeValues(GameObject player = null)
    {
        if (decreaseCamera == true)
        {
            cameraToChange--;
            decreaseCamera = false;
            if (player != null)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, tps[0].position.z);
            }
        }
        else
        {
            cameraToChange++;
            decreaseCamera = true;
            if (player != null)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, tps[1].position.z);
            }
        }
    }
}
