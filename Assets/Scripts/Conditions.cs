using UnityEngine;

public class Conditions : MonoBehaviour
{
    [SerializeField] private float clientsNeededToWin = 1;

    [SerializeField] private int winScene = 2;
    [SerializeField] private int DefeatScene = 3;

    [SerializeField] private int maxFails = 25;
    private int currentFails = 0;
    public static Conditions instance;
    public float _clientsNeeded => clientsNeededToWin;

  
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            WinCondition();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            DefeatCondition();
        }
    }

    public void AddFail()
    {
        currentFails++;
        if (currentFails >= maxFails)
        {
            DefeatCondition();
        }
    }
    public void WinCondition() 
    { 
        GameManager.instance._sceneManager.LoadScene(winScene);
    }
    public void DefeatCondition()
    {
        GameManager.instance._sceneManager.LoadScene(DefeatScene);
    }
}
