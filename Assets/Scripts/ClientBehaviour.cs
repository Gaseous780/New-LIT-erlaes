using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class ClientBehaviour : MonoBehaviour
{
    [SerializeField] private Image happinessFill; 
    [SerializeField] private GameObject happinessCanvas;
    [Header("Aumento de felicidad clientes")]
    [SerializeField] private float maxTime = 700f;
    private float currentTime;
    private bool isOnTable = false;
    private bool timerStarted = false;
    private ClientSpawner spawner;
    [SerializeField] private float normalSpeed = 2f;
    private float currentSpeed;
    [SerializeField] private float floorSpeed = 6f;
    [SerializeField] private int maxTimeThinking = 5;
    [SerializeField] private int minTimeThinking = 2;

    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float maxDistance = 5f;
    private Vector3 originPosition;
    public bool IsBroken => isBroken;
    public bool IsAngry => isAngry;

    [SerializeField] private float angryThreshold = 0.3f; // 30%
    private bool isAngry = false;
    [SerializeField] private float stopDistance = 30f;

    [SerializeField] private TypeOfFoods[] foodCanAsk;
    private TypeOfFoods foodAsked;
    private int canTakeOrder; // 0 = Aun no llego a la mesa, 1 = Esta pensando en que pedir, 2 = Ya tiene la orden pensada, 3 = La orden fue recibida
    private SpriteRenderer thinkSprite;

    private TablePoint table;
    private int tableNumber;

    [SerializeField] private bool isShakuza = false;
    [SerializeField] private float shakuzaMultiplier = 3f;
    [SerializeField] private float distanceMargin = 0.3f;
    private bool isBroken = false;
    [SerializeField] private GameObject dishReplacementPrefab;

    private Animator animator;
    private int animationState; //Para cambiar entre cada animación (solo en cuenta acciones del cliente normal en este comentario). 0 = Idle, 1 = Caminar, 2 = Sentado, 3 = Comiendo

    public static bool clientsBlocked = false;
    private Collider col;
    public bool _isOnTable => isOnTable;
    public int _tableNumber { get { return tableNumber; } set { tableNumber = value; } }
    public int _canTakeOrder => canTakeOrder;
    public TypeOfFoods _foodAsked => foodAsked;
    public TablePoint _table { get { return table; } set { table = value; } }
    [SerializeField] private float queueSpeed = 3f;
    private bool isFollowingPlayer = false;
    [SerializeField] private float followStopDistance = 2f;
    public bool externalMovement;

    [SerializeField] private Renderer renderer1;
    private MaterialPropertyBlock mpb;
    public enum ClientType
    {
        Normal,
        Shakuza,
        OldWoman
    }

    [SerializeField] private ClientType clientType;

    [SerializeField] private float eatingTime = 10f;
    [SerializeField] private Transform door;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool isEating = false;
    private float currentEatingTime;
    private bool isLeaving = false;

    [SerializeField] private Sprite[] foodSprites;
    [SerializeField] private SpriteRenderer foodSpriteRenderer;

    [SerializeField] private GameObject normalModel;
    [SerializeField] private GameObject shakuzaModel;
    [SerializeField] private GameObject oldWomanModel;

    [SerializeField] private FeedbackController feedbackController;
    [SerializeField] private float oldWomanFollowMultiplier = 0.5f;
    public bool isInQueue = true;
    private NavMeshAgent agent;

    private bool isSelected;

    public bool _isSelected { get { return isSelected; } set { isSelected = value; } }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        currentTime = maxTime;
        timerStarted = true;
        thinkSprite = GetComponentInChildren<SpriteRenderer>();
        thinkSprite.gameObject.SetActive(false);
        if (clientType == ClientType.OldWoman)
        {
            currentSpeed = normalSpeed * 0.5f;
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
        col = GetComponent<Collider>();
        GameObject doorObj = GameObject.FindGameObjectWithTag("door");
        if (doorObj != null)
        {
            door = doorObj.transform;
        }

        ChangeAnimation(0);

        feedbackController = GameManager.instance._feedbackController;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = followSpeed;
        agent.stoppingDistance = 3f;
        agent.autoBraking = false;
        agent.updateRotation = true;
        agent.updateUpAxis = false;
        agent.isStopped = false;
        agent.updatePosition = true;
    }

    void UpdateModel()
    {
        normalModel.SetActive(false);
        shakuzaModel.SetActive(false);
        oldWomanModel.SetActive(false);

        switch (clientType)
        {
            case ClientType.Normal:
                normalModel.SetActive(true);
                break;

            case ClientType.Shakuza:
                shakuzaModel.SetActive(true);
                break;

            case ClientType.OldWoman:
                oldWomanModel.SetActive(true);
                break;
        }
    }

    public void MakeNormal()
    {
        clientType = ClientType.Normal;
        UpdateModel();
        currentSpeed = normalSpeed;
        queueSpeed = 3f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isShakuza || (!isAngry && !isBroken)) return;

        DishReference dish = other.GetComponentInParent<DishReference>();

        animator.SetTrigger("Attack");

        if (dish != null)
        {
            ReplaceDish(dish);
        }
    }

    private IEnumerator ReturnAngryAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        animator.ResetTrigger("Attack");
    }

    public void StartFollowing(Transform target)
    {
        player = target;
        isFollowingPlayer = true;

        ChangeAnimation(1);
    }
    public void StopFollowing()
    {
        isFollowingPlayer = false;
    }
    public void MakeOldWoman()
    {
        clientType = ClientType.OldWoman;
        UpdateModel();
        queueSpeed = 0.3f;
        minTimeThinking *= 2;
        maxTimeThinking *= 2;
    }
    bool HasReachedPlayer()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= followStopDistance;
    }
    public float GetQueueSpeed()
    {
        return queueSpeed;
    }
    void Awake()
    {
        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        animationState = 0;

        mpb = new MaterialPropertyBlock();

        isSelected = false;
    }

    void ReplaceDish(DishReference dish)
    {
        if (dish.replacementObject != null)
        {
            dish.replacementObject.SetActive(true);
        }

        Destroy(dish.gameObject);
    }
    public void SetSpawner(ClientSpawner sp)
    {
        spawner = sp;
        foodAsked = default;
        canTakeOrder = 0;
    }
    public void MakeShakuza()
    {
        clientType = ClientType.Shakuza;
        UpdateModel();
        isShakuza = true;
    }
    public void SetInteractable(bool value)
    {
        if (col != null)
            col.enabled = value;
    }

    private void Update()
    {
        if (isSelected == true)
        {
            renderer1.GetPropertyBlock(mpb);

            mpb.SetFloat("_isSelected", 1);

            renderer1.SetPropertyBlock(mpb);
        }
        else
        {
            renderer1.GetPropertyBlock(mpb);

            mpb.SetFloat("_isSelected", 0);

            renderer1.SetPropertyBlock(mpb);
        }

        if (isEating)
        {
            currentEatingTime -= Time.deltaTime;
            if (timerText != null)
            {
                int timeLeft = Mathf.CeilToInt(currentEatingTime);
                timerText.text = timeLeft.ToString();

                if (timeLeft <= 3)
                {
                    timerText.color = Color.red;
                }
            }

            if (currentEatingTime <= 0)
            {
                if (timerText != null)
                    timerText.gameObject.SetActive(false);

                isEating = false;
                GoToDoor();
            }

            return;
        }
        if (isLeaving && door != null)
        {
            Vector3 dir = door.position - transform.position;
            dir.y = 0;

            if (dir.magnitude > 0.2f)
            {
                dir = dir.normalized;
                transform.position += dir * currentSpeed * Time.deltaTime;
                transform.forward = dir;
            }
            else
            {
                DestroyClient();
            }

            return;
        }
        if (!isBroken)
        {
            if (externalMovement && !isShakuza)
            {
                return;
            }
            if (!timerStarted || canTakeOrder == 1)
                return;
            if(isFollowingPlayer && clientType == ClientType.OldWoman)
            {
                float speed = followSpeed;

                if (clientType == ClientType.OldWoman)
                {
                    speed *= oldWomanFollowMultiplier;
                }
            }

            if (isFollowingPlayer && player != null && !(isShakuza && (isAngry || isBroken)))
            {
                if (!agent.enabled)
                    agent.enabled = true;

                float idealDistance = 3f;
                float tolerance = 0.5f;

                Vector3 toPlayer = player.position - transform.position;
                float dist = toPlayer.magnitude;

                Vector3 target;

                if (dist < idealDistance - tolerance)
                {
                    Vector3 fleeDir = (transform.position - player.position).normalized;
                    Vector3 rawTarget = transform.position + fleeDir * 4f;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(rawTarget, out hit, 3f, NavMesh.AllAreas))
                        target = hit.position;
                    else
                        target = transform.position;

                    ChangeAnimation(1);
                }
                else if (dist > idealDistance + tolerance)
                {
                    target = player.position;

                    ChangeAnimation(1);
                }
                else
                {
                    agent.ResetPath();
                    target = transform.position;

                    ChangeAnimation(0);
                }

                agent.SetDestination(target);

                if (agent.velocity.sqrMagnitude > 0.01f)
                    transform.forward = agent.velocity.normalized;

                return;
            }
            float multiplier = isShakuza ? shakuzaMultiplier : 1f;
            currentTime -= Time.deltaTime * currentSpeed * multiplier;
            float percent = Mathf.Clamp01(currentTime / maxTime);
            if (happinessFill != null)
            {
                happinessFill.fillAmount = percent;
            }

            if (clientType == ClientType.OldWoman)
            {
                float offsetX = Mathf.Sin(Time.time * 25f) * 2f;

                transform.position += new Vector3(offsetX, 0, 0) * Time.deltaTime;
            };

            if (percent <= 0f)
            {
                if (isShakuza && !isAngry)
                {
                    BecomeAngryShakuza();
                }
                else if (!isShakuza)
                {
                    Conditions.instance.AddFail();
                    DestroyClient();
                }
            }
            if (percent <= 0.20f) //AQUIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII
            {
                renderer1.GetPropertyBlock(mpb);

                mpb.SetFloat("_isAngry", 1);
                Debug.Log(mpb.GetFloat("_isAngry" + "11111111111111"));

                renderer1.SetPropertyBlock(mpb);
            }
            else
            {
                renderer1.GetPropertyBlock(mpb);

                mpb.SetFloat("_isAngry", 0);

                renderer1.SetPropertyBlock(mpb);
            }
        }
        if ((isAngry || isBroken) && isShakuza && player != null)
        {
            if (!agent.enabled)
                agent.enabled = true;

            float idealDistance = 3f;
            float tolerance = 0.5f;

            Vector3 toPlayer = player.position - transform.position;
            float dist = toPlayer.magnitude;

            Vector3 target;

            if (dist < idealDistance - tolerance)
            {
                Vector3 fleeDir = (transform.position - player.position).normalized;
                Vector3 rawTarget = transform.position + fleeDir * 4f;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(rawTarget, out hit, 3f, NavMesh.AllAreas))
                    target = hit.position;
                else
                    target = transform.position;

                ChangeAnimation(1);
            }
            else if (dist > idealDistance + tolerance)
            {
                target = player.position;

                ChangeAnimation(1);
            }
            else
            {
                agent.ResetPath();
                target = transform.position;

                ChangeAnimation(0);
            }

            agent.SetDestination(target);

            if (agent.velocity.sqrMagnitude > 0.01f)
                transform.forward = agent.velocity.normalized;

            return;
        }

    }
    void GoToDoor()
    {
        isLeaving = true;
        ChangeAnimation(1);
        if (isOnTable)
        {
            table.busy = false;
            table._tableOrder._clientOnTable = null;
        }
    }
    void BecomeAngryShakuza()
    {
        externalMovement = true;
        if (isAngry) return;
        ClientBehaviour.clientsBlocked = true;
        isAngry = true;
        FireUI.instance.ShowFire();
        if (spawner != null)
        {
            spawner.RemoveClient(gameObject);
        }
        transform.position += Vector3.right * 2f;
        if (player == null)
        {
            GameObject playerpos = GameObject.FindGameObjectWithTag("Player");
            if (playerpos != null)
                player = playerpos.transform;
        }
        originPosition = transform.position;
        ChangeAnimationYakuza(animationState);
    }
    public void KillShakuza()
    {
        agent.enabled = false;
        if (!isShakuza) return;
        ClientBehaviour.clientsBlocked = false;
        DestroyClient();
    }

    public void OnFloor()
    {
        isInQueue = false;
        if (isOnTable)
        {
            return; 
        }
        currentSpeed = floorSpeed;

        ChangeAnimation(0);
    }
    public bool CanBeSeated()
    {
        if (clientType != ClientType.OldWoman) return true;

        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= 1.5f;
    }
    public void OnTable()
    {
        isInQueue = false;
        if (isBroken) return;
        isOnTable = true;
        if (happinessCanvas != null) 
            happinessCanvas.SetActive(false); 

        IncreaseHappinessBar(maxTime);
        canTakeOrder = 1;

        ChangeAnimation(2);

        StartCoroutine(ThinkOrder());
    }

    void DestroyClient()
    {
        StopAllCoroutines();

        if (spawner != null)
        {
            spawner.RemoveClient(gameObject);
        }
        if (thinkSprite != null)
            thinkSprite.gameObject.SetActive(false);
        if (isOnTable == true)
        {
            table.busy = false;
            table._tableOrder._clientOnTable = null;

            table._tableOrder._cleandCode.ResetPaint();

            if (GameManager.instance._miniGameManager._orderManager._clientOrders.ContainsKey(gameObject) == true)
            {
                GameManager.instance._UIcontroller.RemoveFoodFromTasks(gameObject);
                GameManager.instance._miniGameManager._orderManager.RemoveOrder(gameObject);
            }
        }
            Destroy(gameObject);
    }

    private IEnumerator ThinkOrder()
    {
        thinkSprite.gameObject.SetActive(true);

        float multiplier = 1f;
        if (clientType == ClientType.OldWoman)
        {
            multiplier = 5f;
        }
        float timeToThink = Random.Range(minTimeThinking, maxTimeThinking + 1) * multiplier;

        int foodToAsk = Random.Range(0, foodCanAsk.Length);

        yield return new WaitForSeconds(timeToThink);

        foodAsked = foodCanAsk[foodToAsk];

        thinkSprite.gameObject.SetActive(false);

        if (foodAsked != TypeOfFoods.Mochis)
        {
            thinkSprite.sprite = foodSprites[(int)foodAsked];
        }
        else
        {
            thinkSprite.sprite = foodSprites[3];
        }
        thinkSprite.gameObject.SetActive(true);

        if (foodAsked == TypeOfFoods.WorkRice)
        {
            CookUI.instance.ShowCook();
        }
        else if (foodAsked == TypeOfFoods.Takoyaki)
        {
            CookUI.instance.Showtempuu();
        }

        feedbackController.PlayParticle(foodToAsk);

        if (happinessCanvas != null) 
            happinessCanvas.SetActive(true);

        canTakeOrder = 2;
        if (TutorialManager.instance != null)
            TutorialManager.instance.OnActionTriggered(TutorialManager.TutorialStep.SeatClient);

        GameManager.instance._UIcontroller.SetTaskImage(TakeOrder(), tableNumber);
    }

    public TypeOfFoods TakeOrder ()
    {
        canTakeOrder = 3;
        GameManager.instance._miniGameManager._orderManager.AddOrder(gameObject);
        IncreaseHappinessBar(maxTime);
        return foodAsked;
    }

    public void Payment(float defaultValueOfFood)
    {
        GameManager.instance._economiyBehaviour.IncreaseMoneyForClient(defaultValueOfFood, currentTime/maxTime); //Se le da la plata al jugador

        player.gameObject.GetComponent<MiniGameManager>()._orderManager.SatisfiedClient(); //Se aumenta el contador de clientes satisfechos
        StartEating();
    }
    void StartEating()
    {
        isEating = true;
        currentEatingTime = eatingTime;

        ChangeAnimation(3);
        if (happinessCanvas != null)
            happinessCanvas.SetActive(false);
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (thinkSprite != null)
            thinkSprite.gameObject.SetActive(false);
    }

    public void IncreaseHappinessBar (float amount)
    {
        if (currentTime + maxTime < maxTime)
        {
            currentTime += amount;
        }
        else
        {
            currentTime = maxTime;
        }
    }

    public void DecreaseHappinessBar (float amount)
    {
        if (currentTime - maxTime > 0)
        {
            currentTime -= amount;
        }
        else 
        {
            currentTime = 0;
        }
    }

    public void ChangeAnimation(int animationToChange) //0 = Idle, 1 = Caminar, 2 = Sentado, 3 = Comiendo
    {
        animationState = animationToChange;
        animator.SetInteger("State", animationState);
    }

    public void ChangeAnimationYakuza (int animationToChange)
    {
        animationState = animationToChange;
        animator.SetInteger("State", animationState);
        animator.SetBool("IsAngry", isAngry);
    }

    public void ChangeAnimationYakuza (float distancePlayer)
    {
        animator.SetFloat("Distance", distancePlayer);
    }
    private void OnDestroy()
    {
        if (isShakuza)
        {
            clientsBlocked = false;
        }
    }
}