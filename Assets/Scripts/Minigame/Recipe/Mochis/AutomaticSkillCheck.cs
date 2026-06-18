using UnityEngine;

public class AutomaticSkillCheck : MonoBehaviour
{
    [Header("PositionsReferences")]

    [SerializeField] private float idealMinPoint;
    [SerializeField] private float idealMaxPoint;

    [Header ("Skill check Parameters")]
    [SerializeField] private Transform skillCheckSemiCircle;
    [SerializeField] private float speedMovement = 3f;
    [SerializeField] private float radius;
    [SerializeField] private float angle;

    [SerializeField] private float limitToBack; //El limite que debe de llegar el punto para darse vuelta
    private bool isGoingReverse;

    [SerializeField] private GameObject visualBar; //La barra sobre la que esta dando vueltas. Solo es visual.
    [SerializeField] private GameObject idealVisualBar;

    private void OnEnable()
    {
        visualBar.SetActive(true);
        idealVisualBar.SetActive(true);

        int initialAngle = Random.Range(0, 2);

        switch (initialAngle)
        {
            case 0:
                angle = 0; 
                isGoingReverse = false;
                break;

            case 1:
                angle = limitToBack - 0.2f;
                isGoingReverse = true;
                break;
        }
    }

    private void OnDisable()
    {
        visualBar.SetActive (false);
        idealVisualBar.SetActive(false);
    }

    private void Update()
    {
        MoveSkillCheck();
    }

    private void MoveSkillCheck()
    {
        float x = skillCheckSemiCircle.position.x - skillCheckSemiCircle.localScale.x + Mathf.Cos(angle) * radius;
        float y = skillCheckSemiCircle.position.y;
        float z = skillCheckSemiCircle.position.z - skillCheckSemiCircle.localScale.z + Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, y, z);

        if (isGoingReverse == false)
        {
            if (angle < limitToBack)
            {
                angle += speedMovement * Time.deltaTime;
            }
            else
            {
                isGoingReverse = true;
            }
        }
        else
        {
            if (angle > 0)
            {
                angle -= speedMovement * Time.deltaTime;
            }
            else
            {
                isGoingReverse = false;
            }
        }
    }

    public bool ResolveSkillCheck(bool returnWithoutTry = false)
    {
        gameObject.SetActive(false);
        if (angle > idealMinPoint && angle < idealMaxPoint && returnWithoutTry == false)
        {
            return true;
        }

        return false;
    }
}
