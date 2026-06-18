using UnityEngine;

public class TableOrder : MonoBehaviour
{
    private ClientBehaviour clientOnTable;

    public ClientBehaviour _clientOnTable { set { clientOnTable = value; } }

    public void LeftFood (FoodBehaviour food, float valueOfFood)
    {
        if (clientOnTable != null)
        {
            if (food._food == clientOnTable._foodAsked)
            {
                clientOnTable.Payment(valueOfFood);
                GameManager.instance._player.GetComponent<Interaction>()._heldObject = null;
                food.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log(food._food);
                Debug.Log(clientOnTable._foodAsked);
            }
        }
        else
        {
            Conditions.instance.AddFail();
        }
    }
}
