using System.Collections;
using UnityEngine;

public class FireUI : MonoBehaviour
{
    public static FireUI instance;
    public SpriteRenderer fire;

    private void Awake()
    {
        instance = this;
        fire.enabled = false;
    }

    public void ShowFire()
    {
        StartCoroutine(Show());
    }
    IEnumerator Show()
    {
        fire.enabled = true;
        yield return new WaitForSeconds(3f);
        fire.enabled = false;
    }
}
