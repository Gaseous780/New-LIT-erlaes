using UnityEngine;

public class RotationFood : MonoBehaviour
{
    [SerializeField] private float speedRotation = 1;

    [SerializeField] private int axes;

    private void Update()
    {
        switch (axes)
        {
            case 1:
                transform.Rotate(0, 0, speedRotation * Time.deltaTime);
                break;

            case 2:
                transform.Rotate(0, speedRotation * Time.deltaTime, 0);
                break;

            case -1:
                transform.Rotate(0, 0, -speedRotation * Time.deltaTime);
                break;

            default:
                transform.Rotate(speedRotation * Time.deltaTime, 0, 0);
                break;
        }
    }
}
