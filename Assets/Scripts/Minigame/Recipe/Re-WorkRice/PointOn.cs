using UnityEngine;

public class PointOn : MonoBehaviour
{
    [SerializeField] private int direction;

    private GameObject player;

    public GameObject _player => player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag ("ReFryOther") == true)
        {
            //other.gameObject.GetComponent<ReFryVariant>().GiveDirection(direction);
            player = other.gameObject;
            //Debug.Log("Tomo");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReFryOther") == true)
        {
            //other.gameObject.GetComponent<ReFryVariant>().GiveDirection(-1);
            player = null;
        }
    }
}
