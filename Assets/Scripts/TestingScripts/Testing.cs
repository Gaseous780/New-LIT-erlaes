
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] ReFryPanBehaviour s;
    [SerializeField] int aImt;

    void Start()
    {
        
    }


    private void Update() //No olvidar borrar
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void Do()
    {
        s.DefineDirection(aImt);
    }
}
