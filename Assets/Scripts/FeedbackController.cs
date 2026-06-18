using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleTables; //0 = particula del wok, 1 = particula del tempour, 2 = particula de freyer
    
    public void PlayParticle (int food)
    {
        particleTables[food].Play();
    }

}
