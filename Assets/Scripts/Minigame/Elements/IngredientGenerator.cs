using System.Collections.Generic;
using UnityEngine;

public class IngredientGenerator : MonoBehaviour
{
    [SerializeField] private GameObject ingredient;
    [SerializeField] private string nameOfTheIngredient;

    private List <GameObject> ingredientsList;

    [SerializeField] private float cost;

    [Header("Sound Section")]
    [SerializeField] private AudioClip soundGrab;
    private SoundManager soundManager;

    public List <GameObject> _ingredientsList => ingredientsList;

    private void OnEnable()
    {
        ingredientsList = new List <GameObject>();
    }

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    public GameObject GrabIngredient()
    {
        soundManager.ReproduceSound(soundGrab);

        GameObject newIngredient = Instantiate (ingredient, transform.position, Quaternion.identity);
        newIngredient.name = nameOfTheIngredient;
        ingredientsList.Add (newIngredient);

        GameManager.instance._economiyBehaviour.DecreaseMoney (cost);

        return newIngredient;
    }
}
