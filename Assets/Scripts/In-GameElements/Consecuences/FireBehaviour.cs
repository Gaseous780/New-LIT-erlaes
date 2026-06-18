using UnityEngine;

public class FireBehaviour : ConsecuencesBase
{
    [SerializeField] private MiniGameManager miniGameManager;
    [SerializeField] private ParticleSystem smokeParticles;

    private float counterToExtinguish;
    [SerializeField] private float speedToExtinguish = 5;

    private void OnEnable()
    {
        counterToExtinguish = 0;
    }

    public override void EnableConsecuences()
    {
        gameObject.SetActive(true);
        if (gameObject.activeSelf == true)
        {
            smokeParticles.gameObject.SetActive(true);
            smokeParticles.Play();
        }
        miniGameManager._thereIsFire = true;

    }

    public override void DisableConsecuences()
    {
        gameObject.SetActive(false);
        if (gameObject.activeSelf == false)
        {
            smokeParticles.gameObject.SetActive(false);
            smokeParticles.Play();
        }
        miniGameManager._thereIsFire = false;
    }

    public void ExtinguishFire()
    {
        if (counterToExtinguish <= 100)
        {
            counterToExtinguish += speedToExtinguish * Time.deltaTime;
        }
        else
        {
            DisableConsecuences();
        }
    }
}
