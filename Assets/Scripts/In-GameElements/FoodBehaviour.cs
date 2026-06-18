using System.Collections;
using UnityEngine;

public enum TypeOfFoods { None, WorkRice, Takoyaki, Seafood, TakoyakiIncomplete, Peach, Chocolate, Strawberry, 
    Mochis, JapaneseCheesecake, JapaneseCheesecakeIncomplete }
public class FoodBehaviour : MonoBehaviour
{
    [SerializeField] private TypeOfFoods food;
    [SerializeField] private float valueOfFood;

    [SerializeField] private float timeInGetBad = 30f;

    public TypeOfFoods _food => food;
    public float _valueOfFood => valueOfFood;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        //StartCoroutine(Putrifact());
    }

    private IEnumerator Putrifact()
    {
        yield return new WaitForSeconds(timeInGetBad);

        gameObject.SetActive(false);
    }

    public void SetFood (TypeOfFoods type, float value, string name)
    {
        food = type;
        valueOfFood = value;

        gameObject.name = name;
    }
}
