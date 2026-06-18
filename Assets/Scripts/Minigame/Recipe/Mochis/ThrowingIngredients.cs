using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingIngredients : MonoBehaviour
{
    private GameObject ingredientToSpawn;

    [SerializeField] private GameObject bomb;

    private List<GameObject> poolIngredients; //En el pool se guarda el gameObject a arrojar

    [SerializeField] private float minToSpawn; //Estas posiciones son para saber en que parte de la pantalla spawnean;
    [SerializeField] private float maxToSpawn;

    [SerializeField] private float forceThrowX; //La fuerza con la que se arrojarán los ingredientes en el eje X
    [SerializeField] private float forceThrowZ; //La fuerza con la que se arrojarán los ingredientes en el eje Z

    [SerializeField] private float maxTimeToSpawnIngredient = 3f; //El tiempo en el que se spawneara un ingrediente

    private float counterOfIngredients; //Los ingredientes que se spawnearan al momento
    [SerializeField] private int maxIngredientsToSpawn = 3;

    [Header("Sounds Section")]
    [SerializeField] private AudioClip throwSound;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = GameManager.instance._soundManager;
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        foreach (GameObject ingredient in poolIngredients)
        {
            ingredient.SetActive(false);
            foreach (GameObject rest in ingredient.GetComponent<IngredientToCut>()._restCutList)
            {
                rest.SetActive(false);
            }
        }
    }

    public void SetDefaultsValues(GameObject ingredient)
    {
        ingredientToSpawn = ingredient;
        poolIngredients = new List<GameObject>();

        counterOfIngredients = 1f;

        StartCoroutine(GenerateIngredients());
    }

    private IEnumerator GenerateIngredients()
    {
        for (int i = 0; i < (int)counterOfIngredients; i++)
        {
            SpawnIngredient();
        }

        if (counterOfIngredients <= maxIngredientsToSpawn)
        {
            counterOfIngredients += 0.3f;
        }

        float timeToReSpawn = Random.Range(0, maxTimeToSpawnIngredient);

        yield return new WaitForSeconds(timeToReSpawn);

        StartCoroutine(GenerateIngredients());
    }

    private void SpawnIngredient()
    {
        if (soundManager == null) { soundManager = GameManager.instance._soundManager; }
        soundManager.ReproduceSound(throwSound);

        float positionOnScreen = Random.Range(minToSpawn, maxToSpawn);

        Vector3 position = new Vector3(positionOnScreen, transform.position.y, transform.position.z);

        //for (int i = 0; i < poolIngredients.Count; i++)
        //{
        //    if (poolIngredients[i].GetComponent<IngredientToCut>()._isInScene == false || poolIngredients[i].activeSelf == false)
        //    {
        //        poolIngredients[i].SetActive(true);
        //        poolIngredients[i].transform.position = position;

        //        Throw(poolIngredients[i]);

        //        return;
        //    }
        //}

        int isBomb = Random.Range(0, 5);

        GameObject newIngredient;

        if (isBomb == 0)
        {
            newIngredient = Instantiate(bomb, position, Quaternion.identity);
        }
        else 
        {
            newIngredient = Instantiate(ingredientToSpawn, position, Quaternion.identity); 
        }

        poolIngredients.Add(newIngredient);
        Throw (newIngredient);
    }

    private void Throw(GameObject throwGameObject)
    {
        int throwDirection = Random.Range(0, 5);

        switch (throwDirection)
        {
            case 0:
                throwGameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward) * forceThrowZ, ForceMode.Force);
                break;

            case 1:
                throwGameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward * forceThrowZ) + (Vector3.left * forceThrowX), ForceMode.Force);
                break;

            case 2:
                throwGameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward * forceThrowZ) + (Vector3.right * forceThrowX), ForceMode.Force);
                break;

            case 3:
                throwGameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward * forceThrowZ) + (Vector3.left * (forceThrowX * 0.5f)), ForceMode.Force);
                break;

            case 4:
                throwGameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward * forceThrowZ) + (Vector3.right * (forceThrowX * 0.5f)), ForceMode.Force);
                break;
        }
    }
}
