using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image[] slotsFood;
    [SerializeField] private Sprite[] imagesOfFood; //Imagén cero es de wok
    [SerializeField] private Image[] tableImage;
    private int slotsInUses = 0;

    private Dictionary <int , int > asociationTables; //La key int es el slot del pedido, mientras que el value es el slot de la mesa

    [Header("Sounds Section")]
    [SerializeField] private AudioClip wokSound;
    [SerializeField] private AudioClip tempSound;
    [SerializeField] private AudioClip mochisSound;
    private SoundManager soundManager;

    private void Awake()
    {
        asociationTables = new Dictionary<int, int > ();
    }

    private void Start()
    {
        foreach (Image slot in slotsFood)
        {
            slot.color = new Color32(0,0,0,0);
        }
        foreach (Image tableImage in tableImage)
        {
            tableImage.enabled = false;
        }

        soundManager = GameManager.instance._soundManager;
    }

    public void SetTaskImage(TypeOfFoods food, int table) 
    {
        int slotToUse = 0;

        for (int i = 0; i < 3; i++)
        {
            if (slotsFood[i].sprite == null)
            {
                slotToUse = i;
                break;
            }
        }

        switch (food)
        {
            case TypeOfFoods.WorkRice:
                slotsFood[slotToUse].sprite = imagesOfFood[0];
                slotsFood[slotToUse].color = Color.white;

                soundManager.ReproduceSound(wokSound);
                break;
            case TypeOfFoods.Takoyaki:
                slotsFood[slotToUse].sprite = imagesOfFood[1];
                slotsFood[slotToUse].color = Color.white;

                soundManager.ReproduceSound(tempSound);
                break;
            case TypeOfFoods.Mochis:
                slotsFood[slotToUse].sprite = imagesOfFood[2];
                slotsFood[slotToUse].color = Color.white;

                soundManager.ReproduceSound(mochisSound);
                break;
            case TypeOfFoods.JapaneseCheesecake:
                slotsFood[slotToUse].sprite = imagesOfFood[3];
                slotsFood[slotToUse].color = Color.white;
                break;

        }

        tableImage[table - 1].enabled = true;
        tableImage[table - 1].rectTransform.localPosition = new Vector3(slotsFood[slotToUse].transform.localPosition.x - 40, tableImage[table - 1].rectTransform.localPosition.y, tableImage[table - 1].rectTransform.localPosition.z);
        asociationTables.Add(slotToUse, table - 1);

        slotsInUses ++;
    }

    public void RemoveFoodFromTasks(GameObject client)
    {
        int numberOfSlot = GameManager.instance._miniGameManager._orderManager._clientOrders[client] - 1;

        slotsFood[numberOfSlot].sprite = null;
        slotsFood[numberOfSlot].color = new Color32(0, 0, 0, 0);

        tableImage[client.GetComponent<ClientBehaviour>()._tableNumber - 1].enabled = false;
        asociationTables.Remove(numberOfSlot);

        slotsInUses--;

        MoveTaskOrder(numberOfSlot);
    }

    private void MoveTaskOrder(int slotRemoved)
    {
        for (int i = slotRemoved; i < 3; i++) 
        {
            if (i < 2)
            {
                if (slotsFood[i + 1].sprite != null)
                {
                    tableImage[asociationTables[i + 1]].rectTransform.localPosition = new Vector3(slotsFood[i].transform.localPosition.x - 40, tableImage[asociationTables[i + 1]].rectTransform.localPosition.y, tableImage[asociationTables[i + 1]].rectTransform.localPosition.z);
                    slotsFood[i].sprite = slotsFood[i + 1].sprite;
                    slotsFood[i].color = Color.white;
                    asociationTables.Add(i, asociationTables[i + 1]);
                    asociationTables.Remove(i + 1);

                }
                else
                {
                    slotsFood[i].sprite = null;
                    slotsFood[i].color = new Color32(0, 0, 0, 0);
                }
            }
            else
            {
                slotsFood[i].sprite = null;
                slotsFood[i].color = new Color32(0, 0, 0, 0);
            }
        }
    }
}
