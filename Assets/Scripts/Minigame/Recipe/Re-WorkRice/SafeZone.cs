using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null) { return; }

        fry._isSafe = true;
    }

    private void OnTriggerExit(Collider other)
    {
        ReFryPanBehaviour fry = other.GetComponent<ReFryPanBehaviour>();

        if (fry == null) { return; }

        fry._isSafe = false;
    }
}
