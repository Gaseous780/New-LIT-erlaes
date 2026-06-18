using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarBehaviour : MonoBehaviour
{
    [Header ("Points")]
    [SerializeField] private float maxPoints;

    private float actualPoints;

    [SerializeField] private float idealPointsMin;
    [SerializeField] private float idealPointsMax;

    [SerializeField] private float scaleMax = 7.5f;
    private float scaleMin;
    private float idealMaxPositionOnImage;
    private float idealMinPositionOnImage;

    [Header ("Movement of the bar")]
    [SerializeField] private float constantSubstract;
    [SerializeField] private int pointAdding;
    [SerializeField] private float movementBar;


    [Header ("Bar positions adjustment")]
    [SerializeField] private float maxPosition;
    [SerializeField] private float minPosition;

    private RectTransform rectTransform;

    [Header ("References")]
    [SerializeField] private MiniGamesBase minigameToSkill;
    [SerializeField] private Image zImage;
    [SerializeField] private Image xImage;
    [SerializeField] private Image idealBar;

    [SerializeField] private Image barComplete;

    [Header("Rythm Buttons")]
    [SerializeField] private float TimeToGetButton = 1f;

    [Header ("FeedBack")]
    [SerializeField] private GameObject fryPanObject;
    [SerializeField] private float amountToMoveFry = 2;
    private Vector3 fryPanOriginalPosition;

    [SerializeField] private float speedFryMove;
    private bool isMovingFry;
    private float amountToMove;
    private float progressToMove;

    private bool nextIsZ;
    private bool isOnTimeToPulse;

    private bool isInEvent;
    private bool isOnIdeal;

    public float _maxPoints => maxPoints;
    public bool _isInEvent { get { return isInEvent; } set { isInEvent = value; } }
    public bool _isOnIdeal => isOnIdeal;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        scaleMin = 0f;

        fryPanOriginalPosition = fryPanObject.transform.position;
    }

    private void Start()
    {
        //barComplete = GetComponentInParent<Image>(); Por algún motivo se tira a si mismo, revisar después

        Vector3[] anchors = new Vector3[4];

        barComplete.rectTransform.GetLocalCorners(anchors);

        idealMaxPositionOnImage = anchors[3].x;
        idealMinPositionOnImage = anchors[1].x;

        idealBar.rectTransform.localPosition = new Vector3(-Mathf.Lerp(idealMaxPositionOnImage, idealMinPositionOnImage, Mathf.Lerp(idealPointsMax, idealPointsMin, 0.5f) / 100), idealBar.rectTransform.localPosition.y , idealBar.rectTransform.localPosition.z);
        idealBar.rectTransform.localScale = new Vector3(Mathf.Lerp(scaleMin, scaleMax, Mathf.Abs(idealPointsMax - idealPointsMin) / 100), idealBar.rectTransform.localScale.y, idealBar.rectTransform.localScale.z);
    }

    private void OnEnable()
    {
        progressToMove = 0;

        isInEvent = false;
        isOnIdeal = false;

        actualPoints = 0;

        zImage.color = new Color (1,1,1,0.5f);
        xImage.color = new Color (1,1,1,0.5f);

        StartCoroutine(DeterminateButton());
    }

    private void OnDisable()
    {
        ReturnFryToOriginalPosition();
    }

    public void Update()
    {
        UpdatePositionBar();

        if (actualPoints > 1 && isInEvent == false)
        {
            actualPoints -= constantSubstract * Time.deltaTime;

            if (actualPoints > idealPointsMin && actualPoints < idealPointsMax)
            {
                isOnIdeal = true;
                if (isInEvent == false) { OnPoint(); }
            }
            else
            {
                isOnIdeal = false;
            }
        }

        if (isMovingFry == true)
        {
            if (progressToMove <= 1)
            {
                fryPanObject.transform.position = new Vector3 (Mathf.Lerp(fryPanObject.transform.position.x, fryPanObject.transform.position.x + amountToMove, progressToMove),fryPanObject.transform.position.y, fryPanObject.transform.position.z);
                progressToMove += speedFryMove * Time.deltaTime;
                progressToMove = Mathf.Clamp(progressToMove, 0, 1.03f);
            }
            else
            {
                isMovingFry = false;
                progressToMove = 0;
            }
        }
    }

    private IEnumerator DeterminateButton()
    {
        isOnTimeToPulse = false;

        yield return new WaitForSeconds(TimeToGetButton);

        int whosNext = Random.Range(0, 2); //0 es Z y 1 es X

        if (whosNext == 0)
        {
            nextIsZ = true;
            zImage.color = Color.white;
        }
        else
        {
            nextIsZ = false;
            xImage.color = Color.white;
        }

        isOnTimeToPulse = true;
        yield return null;
    }

    public void AddPoints (int pointsToAdd, bool isZ)
    {
        if (isInEvent == false)
        {
            zImage.color = new Color(1, 1, 1, 0.5f);
            xImage.color = new Color(1, 1, 1, 0.5f);

            if (actualPoints < maxPoints && isZ == nextIsZ && isOnTimeToPulse == true)
            {
                if (isZ == true)
                {
                    MoveFryPan(1);
                }
                else
                {
                    MoveFryPan(0);
                }
                pointsToAdd *= pointAdding;
                actualPoints += pointsToAdd;

                if (actualPoints >= 99)
                {
                    actualPoints = 99;
                }

                nextIsZ = !isZ; //El siguiente es el contrario pulsado
                isOnTimeToPulse = false;

                StartCoroutine(NextButtonRythm());
            }
            else if (isZ != nextIsZ)
            {
                if (minigameToSkill.Error() == false) { StartCoroutine(DeterminateButton()); }
                ReturnFryToOriginalPosition();
            }
        }
    }

    private IEnumerator NextButtonRythm()
    {
        yield return new WaitForSeconds(TimeToGetButton);

        isOnTimeToPulse = true;

        if (nextIsZ == true)
        {
            zImage.color = Color.white;
        }
        else
        {
            xImage.color = Color.white;
        }

        yield return null;
    }

    private void UpdatePositionBar()
    {
        rectTransform.localPosition = new Vector3(Mathf.Lerp(minPosition, maxPosition, actualPoints/100),rectTransform.localPosition.y, rectTransform.position.z);
    }

    private void OnPoint()
    {
        IRythm rythmMinigame = minigameToSkill as IRythm;
        rythmMinigame.RythmEvent();
    }

    private void MoveFryPan(int directionToMove)
    {
        if (isMovingFry == true) { transform.position = new Vector3 (fryPanOriginalPosition.x + -directionToMove, transform.position.y, transform.position.z); }

        if (fryPanObject.transform.position != fryPanOriginalPosition)
        {
            if (directionToMove == 0) //Derecha
            {
                amountToMove = amountToMoveFry;
            }
            else if (directionToMove == 1) //Izquierda
            {
                amountToMove = -amountToMoveFry;
            }
        }
        else
        {
            if (directionToMove == 0) //Derecha
            {
                amountToMove = amountToMoveFry / 2;
            }
            else if (directionToMove == 1) //Izquierda
            {
                amountToMove = -amountToMoveFry / 2;
            }
        }

        isMovingFry = true;
    }

    private void ReturnFryToOriginalPosition()
    {
        fryPanObject.transform.position = fryPanOriginalPosition;
    }
}
