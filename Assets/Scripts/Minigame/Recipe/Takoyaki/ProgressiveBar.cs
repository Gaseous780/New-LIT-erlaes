using UnityEngine;
using UnityEngine.UI;

public class ProgressiveBar : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private float maxPosition;
    [SerializeField] private float minPosition;

    [SerializeField] private float timeToComplete = 4f;

    private float progress;

    [SerializeField] private GameObject textTo;

    private bool canActivate;

    public bool _canActivate { get { return canActivate; } set { canActivate = value; } }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        ResetProgress();
        canActivate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canActivate == true)
        {
            if (progress <= timeToComplete)
            {
                progress += Time.deltaTime;
            }
            else
            {
                textTo.SetActive(true);
            }

            UpdatePositionBar();
        }
    }

    private void UpdatePositionBar()
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, Mathf.Lerp(minPosition, maxPosition, progress / timeToComplete), rectTransform.position.z);
    }

    public void ResetProgress()
    {
        progress = 0;
        textTo.SetActive(false);
    }
}
