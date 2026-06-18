using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatyClientBehaviour : MonoBehaviour
{
    [SerializeField] private ClientBehaviour client;
    [SerializeField] private GameObject sweatPuddlePrefab;
    [SerializeField] private List<Transform> wanderingPoints;
    [SerializeField] private Transform bathroomPoint;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private float puddleInterval = 10f;
    [SerializeField] private float timeBeforeWalking = 8f;
    [SerializeField] private float bathroomChance = 0.4f;

    private bool isWalking;
    private bool canMove;

    private void Start()
    {
        if (client == null)
            client = GetComponent<ClientBehaviour>();

        StartCoroutine(MainRoutine());
        StartCoroutine(DropPuddles());
    }
    private void Awake()
    {
        client = GetComponent<ClientBehaviour>();

        wanderingPoints = new List<Transform>(
            GameObject.Find("SweatPointsHolder")
            .GetComponentsInChildren<Transform>()
        );

        GameObject bathroom = GameObject.Find("BathroomPoint");

        if (bathroom != null)
            bathroomPoint = bathroom.transform;
    }

    IEnumerator MainRoutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => client._isOnTable);

            yield return new WaitForSeconds(timeBeforeWalking);

            if (Random.value <= bathroomChance)
            {
                yield return StartCoroutine(GoToPoint(bathroomPoint));
            }
            else
            {
                Transform randomPoint =
                    wanderingPoints[Random.Range(0, wanderingPoints.Count)];

                yield return StartCoroutine(GoToPoint(randomPoint));
            }

            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(ReturnToTable());
        }
    }

    IEnumerator GoToPoint(Transform target)
    {
        canMove = true;
        isWalking = true;

        while (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            Vector3 dir = (target.position - transform.position).normalized;

            transform.position += dir * walkSpeed * Time.deltaTime;
            transform.forward = dir;

            yield return null;
        }

        isWalking = false;
    }

    IEnumerator ReturnToTable()
    {
        if (client._table == null)
            yield break;

        Transform tablePos = client._table.transform;

        isWalking = true;

        while (Vector3.Distance(transform.position, tablePos.position) > stopDistance)
        {
            Vector3 dir = (tablePos.position - transform.position).normalized;

            transform.position += dir * walkSpeed * Time.deltaTime;
            transform.forward = dir;

            yield return null;
        }

        isWalking = false;
    }

    IEnumerator DropPuddles()
    {
        while (true)
        {
            yield return new WaitForSeconds(puddleInterval);

            if (isWalking)
            {
                Instantiate(
                    sweatPuddlePrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }
    }
}