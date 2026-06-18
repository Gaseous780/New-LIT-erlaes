using System.Collections;
using UnityEngine;

public enum TypeOfFoods { None, WorkRice, Takoyaki, Seafood, TakoyakiIncomplete, Peach, Chocolate, Strawberry, 
    Mochis, JapaneseCheesecake, JapaneseCheesecakeIncomplete }
public class FoodBehaviour : MonoBehaviour
{
    [SerializeField] private TypeOfFoods food;
    [SerializeField] private float valueOfFood;

    [SerializeField] private float timeInGetBad = 30f;

    [SerializeField] private MeshRenderer rendererMPB;
    [SerializeField] private int indexMesh;
    private MaterialPropertyBlock mpb;

    private int state;

    public TypeOfFoods _food => food;
    public float _valueOfFood => valueOfFood;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        switch (food)
        {
            case TypeOfFoods.WorkRice:
                Debug.Log("MeDisparo");
                StartCoroutine(Putrifact());
                break;

            case TypeOfFoods.Takoyaki:
                Debug.Log("MeDisparo");
                StartCoroutine(Putrifact());
                break;
        }
    }

    private IEnumerator Putrifact()
    {
        yield return new WaitForSeconds(timeInGetBad);

        rendererMPB.GetPropertyBlock(mpb, indexMesh);

        state++;

        mpb.SetFloat("_State", state);

        rendererMPB.SetPropertyBlock(mpb, indexMesh);

        if (state > 3) { yield return null; }
        else { StartCoroutine (Putrifact()); }

    }

    public void SetFood (TypeOfFoods type, float value, string name)
    {
        food = type;
        valueOfFood = value;

        gameObject.name = name;
    }
}
