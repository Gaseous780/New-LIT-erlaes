using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField] private MiniGamesBase recipeController;

    [SerializeField] private float amountToAport;

    [Header ("Sounds Section")]
    [SerializeField] private AudioClip soundBreak;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Breakable") == true)
        {
            soundManager.ReproduceSound(soundBreak);
            IAddIngredients recipeToAdd = recipeController as IAddIngredients;
            recipeToAdd.AddIngredientsToBowl();
            other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dragger") == true)
        {
            IAddIngredients recipeToAdd = recipeController as IAddIngredients;
            recipeToAdd.AddIngredientsToBowl(amountToAport * Time.deltaTime, other.gameObject.name);
        }
    }
}
