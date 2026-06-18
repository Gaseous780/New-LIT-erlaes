using UnityEngine;

public class PointFry : MonoBehaviour
{
    [SerializeField] private ReWorkRiceBehaviour re;

    public void AddPoint(bool succes)
    {
        re.CompleteSkillCheck(succes);
    }
}
