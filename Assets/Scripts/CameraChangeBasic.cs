using UnityEngine;

public class CameraChangeBasic : MonoBehaviour
{
    [SerializeField] private GameObject cameraToDisable;
    [SerializeField] private GameObject cameraToEnable;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            ChangeCamera();
        }
    }

    public void ChangeCamera()
    {
        Debug.Log("jhg");
        if (cameraToDisable.activeSelf == true)
        {
            cameraToEnable.SetActive(true);
            cameraToDisable.SetActive(false);
        }
        else
        {
            cameraToEnable.SetActive(false);
            cameraToDisable.SetActive(true);
        }
    }
}
