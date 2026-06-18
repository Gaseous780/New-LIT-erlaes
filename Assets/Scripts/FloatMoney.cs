using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatMoney : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] holdersNumbers;

    [SerializeField] private Texture[] numbersImages;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float timeToGetOff = 2f;

    private MaterialPropertyBlock mpb;

    private bool canMove;
    private int numbersToMove;

    [SerializeField] private float defaultCutOff = -0.44f;
    [SerializeField] private float speedCutOff = 0.5f;

    [SerializeField]private float yPosition;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (canMove == true)
        {
            for (int i = 0; i < numbersToMove; i++)
            {
                holdersNumbers[i].transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
                mpb = new MaterialPropertyBlock();
                holdersNumbers[i].GetPropertyBlock(mpb);
                mpb.SetFloat("_CutOffHeight", mpb.GetFloat("_CutOffHeight") + speedCutOff *Time.deltaTime);
                holdersNumbers[i].SetPropertyBlock(mpb);
            }
        }
    }

    public void ShowMoney (int amount)
    {
        StopAllCoroutines();
        Debug.Log(amount);
        for (int i = 0; i < holdersNumbers.Length; i++)
        {
            holdersNumbers[i].transform.localPosition = new Vector3 (holdersNumbers[i].transform.localPosition.x, yPosition, holdersNumbers[i].transform.localPosition.z);
            holdersNumbers[i].enabled = false;
            mpb = new MaterialPropertyBlock();
            holdersNumbers[i].GetPropertyBlock(mpb);
            mpb.SetFloat("_CutOffHeight", defaultCutOff);
            holdersNumbers[i].SetPropertyBlock(mpb);
        }

        char[] amountToUse = amount.ToString().ToCharArray();

        for (int i = 0; i < amountToUse.Length; i++) 
        {
            mpb = new MaterialPropertyBlock();
            holdersNumbers[i].enabled = true;
            holdersNumbers[i].GetPropertyBlock(mpb);
            mpb.SetTexture("_Texture", numbersImages[(int)char.GetNumericValue(amountToUse[i])]);
            //holdersNumbers[i].sprite = numbersImages[(int)char.GetNumericValue(amountToUse[i])];
            holdersNumbers[i].SetPropertyBlock(mpb);
        }

        canMove = true;
        numbersToMove = amountToUse.Length;

        StartCoroutine(TimeToOff());
    }

    private IEnumerator TimeToOff()
    {
        yield return new WaitForSeconds(timeToGetOff);

        for (int i = 0; i < holdersNumbers.Length; i++)
        {
            holdersNumbers[i].transform.localPosition = new Vector3(holdersNumbers[i].transform.localPosition.x, yPosition, holdersNumbers[i].transform.localPosition.z);
            holdersNumbers[i].enabled = false;
        }

        canMove = false;
    }
}
