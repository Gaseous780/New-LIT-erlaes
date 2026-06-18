using System.Collections.Generic;
using UnityEngine;

public class OrderManager
{
    private Dictionary<GameObject, int> clientOrders; //El gameObject es el cliente el cual hizo el pedido, mientras que el int es el número de pedido
    private int clientCounter;
    private int clientsSatisfied;

    private int limitOfOrders = 3;

    private int[] numberOfOrdersAskeds = new int[4];

    private Conditions gameConditions;

    public Dictionary<GameObject, int> _clientOrders => clientOrders;

    public OrderManager()
    {
        InitializeOrderManager();
    }

    public void InitializeOrderManager()
    {
        clientCounter = 0;
        clientsSatisfied = 0;
        clientOrders = new Dictionary<GameObject, int>();

        gameConditions = GameManager.instance._gameConditions;
    }

    public bool AddOrder(GameObject client)
    {
        int numberOfOrder = -1;
        int indexToSave = numberOfOrdersAskeds.Length - 1;

        for (int i = 0; i < limitOfOrders + 1; i++)
        {
            for (int j = 0; j < numberOfOrdersAskeds.Length; j++)
            {
                if (numberOfOrder == -1)
                {
                    indexToSave = GetIndexWithNoUse();
                }

                numberOfOrder = i;

                if (i == numberOfOrdersAskeds[j])
                {
                    numberOfOrder = -1;
                    break;
                }
            }

            if (numberOfOrder != -1)
            {
                break;
            }
        }

        if (numberOfOrder == -1)
        {
            return false;
        }

        clientOrders.Add(client, numberOfOrder);

        numberOfOrdersAskeds[indexToSave] = numberOfOrder;

        clientCounter++;

        return true;
    }

    public void RemoveOrder(GameObject client)
    {
        ReOrderAsks(clientOrders[client]);

        clientOrders.Remove(client);
        clientCounter--;
    }

    private int GetIndexWithNoUse()
    {
        for (int i = 0; i < numberOfOrdersAskeds.Length; i++)
        {
            if (numberOfOrdersAskeds[i] == 0)
            {
                return i;
            }
        }

        return 99;
    }

    public void ReOrderAsks(int orderDeleted)
    {
        for (int i = orderDeleted - 1; i < numberOfOrdersAskeds.Length - 1; i++)
        {
            if (numberOfOrdersAskeds[i + 1] != 0)
            {
                foreach (GameObject j in clientOrders.Keys)
                {
                    if (clientOrders[j] == numberOfOrdersAskeds[i + 1] && clientOrders[j] != 0)
                    {
                        clientOrders[j] = numberOfOrdersAskeds[i];
                        break;
                    }
                }
                numberOfOrdersAskeds[i] = numberOfOrdersAskeds[i + 1] - 1;
            }
            else
            {
                numberOfOrdersAskeds[i] = 0;
            }
        }
    }

    public void SatisfiedClient()
    {
        clientsSatisfied++;

        if (clientsSatisfied > gameConditions._clientsNeeded)
        {
            gameConditions.WinCondition();
        }
    }
}
