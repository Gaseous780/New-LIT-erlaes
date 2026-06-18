using System.Collections;
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
            EggScript egg = other.GetComponentInChildren<EggScript>();

            if (egg != null && egg._eggCounter < 2)
            {
                MaterialPropertyBlock mbp = new MaterialPropertyBlock();
                Renderer renderer = egg.gameObject.GetComponent<MeshRenderer>();
                renderer.GetPropertyBlock(mbp);
                mbp.SetFloat("_EggState", egg._eggCounter +  1);
                egg._eggCounter += 1;
                Debug.Log(mbp.GetFloat("_EggState"));
                renderer.SetPropertyBlock(mbp);

                return;
            }

            StartCoroutine(BreakEgg(other));

        }
    }

    private IEnumerator BreakEgg(Collider other)
    {
        yield return new WaitForSeconds (1);

        soundManager.ReproduceSound(soundBreak);
        IAddIngredients recipeToAdd = recipeController as IAddIngredients;
        recipeToAdd.AddIngredientsToBowl();
        other.gameObject.SetActive(false);
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
