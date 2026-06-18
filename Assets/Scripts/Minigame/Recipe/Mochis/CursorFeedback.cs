using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFeedback : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = 6.3f;
        Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = position;
    }
    public void MoveCursor()
    {
        
    }
}
