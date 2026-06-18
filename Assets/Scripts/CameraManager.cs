using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private CinemachineBrain brain;

    [SerializeField] CinemachineVirtualCamera worldCamera;
    [SerializeField] CinemachineVirtualCamera transitionCameraToWorld;
    [SerializeField] CinemachineVirtualCamera transitionCameraToMinigame;
    [SerializeField] CinemachineVirtualCamera miniGameCamera;
    [SerializeField] CinemachineVirtualCamera panelTransitionCamera;
    [SerializeField] CinemachineVirtualCamera panelCamera;

    [SerializeField] private CinemachineVirtualCamera kitchenCamera; //Camaras de mundo
    [SerializeField] private CinemachineVirtualCamera exteriorCamera;

    private float defaultBlendingTime;
    private int waitingToChange; // 0 = Inactivo, 1 = De mundo a minijuego, 2 = De minijuego a mundo

    private int lastCamera = 0;

    public CinemachineBrain _brain => brain;

    private void Awake()
    {
        waitingToChange = 0;

        brain = GetComponent<CinemachineBrain>();

        defaultBlendingTime = brain.m_DefaultBlend.BlendTime;
    }

    private void Update()
    {
        switch (waitingToChange)
        {
            case 1:
                if (brain.IsBlending == false)
                {
                    waitingToChange = 0;

                    brain.m_DefaultBlend.m_Time = 0;

                    miniGameCamera.gameObject.SetActive(true);
                    transitionCameraToMinigame.gameObject.SetActive(false);
                }
                break;

            case 2:
                if (brain.IsBlending == false)
                {
                    waitingToChange = 0;

                    brain.m_DefaultBlend.m_Time = 0;


                    CameraUsed ().gameObject.SetActive(true);
                    //worldCamera.gameObject.SetActive(true);
                    transitionCameraToWorld.gameObject.SetActive(false);
                }
                break;

            case 3:
                if (brain.IsBlending == false)
                {
                    waitingToChange = 0;

                    brain.m_DefaultBlend.m_Time = 0;

                    panelCamera.gameObject.SetActive(true);
                    transitionCameraToMinigame.gameObject.SetActive(false);
                }
                break;

            case 4:
                if (brain.IsBlending == false)
                {
                    waitingToChange = 0;

                    brain.m_DefaultBlend.m_Time = 0;

                    CameraUsed().gameObject.SetActive(true);
                    //worldCamera.gameObject.SetActive(true);
                    panelTransitionCamera.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void ChangeCamera(int cameraToChange) // 0 = De la camara del mundo a la de los minijuegos, 1 = De la camara de los minijuegos a la del mundo
    {
        brain.m_DefaultBlend.m_Time = defaultBlendingTime;

        switch (cameraToChange)
        {
            case 0:
                transitionCameraToMinigame.gameObject.SetActive(true);
                worldCamera.gameObject.SetActive(false);
                kitchenCamera.gameObject.SetActive(false);
                exteriorCamera.gameObject.SetActive(false);

                StartCoroutine(WaitingToCamera(1));
                break;

            case 1:
                transitionCameraToWorld.gameObject.SetActive(true);
                miniGameCamera.gameObject.SetActive(false);

                StartCoroutine(WaitingToCamera(2));
                break;

            case 2:
                transitionCameraToMinigame.gameObject.SetActive(true);
                worldCamera.gameObject.SetActive(false);
                kitchenCamera.gameObject.SetActive(false);
                exteriorCamera.gameObject.SetActive(false);

                StartCoroutine(WaitingToCamera(3));
                break;
            case 3:
                panelTransitionCamera.gameObject.SetActive(true);
                panelCamera.gameObject.SetActive(false);

                StartCoroutine(WaitingToCamera(4));
                break;
        }
    }

    public void ChangeCameraSimple(int cameraToChange)
    {
        brain.m_DefaultBlend.m_Time = defaultBlendingTime;

        switch (cameraToChange) //0 = A principal, 1 = A Cocina, 2 = A Exterior
        {
            case 0:
                worldCamera.gameObject.SetActive(true);
                kitchenCamera.gameObject.SetActive(false);
                lastCamera = 0;
                break;
            case 1:
                kitchenCamera.gameObject.SetActive(true);
                worldCamera.gameObject.SetActive(false);
                exteriorCamera.gameObject.SetActive(false);
                lastCamera = 1;
                break;

            case 2:
                exteriorCamera.gameObject.SetActive(true);
                kitchenCamera.gameObject.SetActive(false);
                lastCamera = 2;
                break;
        }
    }

    private IEnumerator WaitingToCamera(int typeOfWaiting)
    {
        yield return new WaitForSeconds(0.5f);

        waitingToChange = typeOfWaiting;

    }

    private CinemachineVirtualCamera CameraUsed()
    {
        switch (lastCamera)
        {
            case 0:
                return worldCamera;

            case 1:
                return kitchenCamera;

            case 2:
                return exteriorCamera;

            default:
                return worldCamera;
        }
    }
}