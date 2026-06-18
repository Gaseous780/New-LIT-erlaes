using UnityEngine;
using UnityEngine.UI;

public class LifeBehaviour : MonoBehaviour
{
    [SerializeField] private Image[] hearts;

    private int lifes;

    [SerializeField] private int maxLifes = 3;

    private void OnEnable()
    {
        lifes = maxLifes;

        foreach (Image image in hearts)
        {
            image.enabled = true;
        }
    }

    public int AddLife()
    {
        UpdateHearts(true);

        lifes++;

        return lifes;
    }

    public int RestLife()
    {
        lifes--;

        UpdateHearts(false);

        return lifes;
    }

    private void UpdateHearts(bool status)
    {
        hearts[lifes].enabled = status;
    }

}
