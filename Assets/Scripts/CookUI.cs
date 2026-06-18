using System.Collections;
using UnityEngine;

public class CookUI : MonoBehaviour
{
    public static CookUI instance;
    public SpriteRenderer cook;
    public SpriteRenderer tempu;
    public SpriteRenderer freir;
    public SpriteRenderer refri;

    private void Awake()
    {
        instance = this;
        cook.enabled = false;
        tempu.enabled = false;
        freir.enabled = false;
        refri.enabled = false;
    }

    public void ShowCook()
    {
        StartCoroutine(Show());
    }
    public void Showtempuu()
    {
        StartCoroutine(Showtempu());
    }
    IEnumerator Show()
    {
        cook.enabled = true;
        yield return new WaitForSeconds(5f);
        cook.enabled = false;
    }
    IEnumerator Showtempu()
    {
        refri.enabled = true;
        yield return new WaitForSeconds(5f);
        refri.enabled = false;
        tempu.enabled = true;
        yield return new WaitForSeconds(5f);
        tempu.enabled = false;
        freir.enabled = true;
        yield return new WaitForSeconds(5f);
        freir.enabled = false;
    }
}