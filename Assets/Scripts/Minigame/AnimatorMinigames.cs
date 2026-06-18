using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatorMinigames : MonoBehaviour
{
    [SerializeField] private Transform[] positionsToMove; //Los transforms son las posiciones en las que se moveran los objetos, no nencesariamente cada transform es de una secuencia
    [SerializeField] private GameObject[] gameObjectsToMove;
    [SerializeField] private string[] sequencesAnimation; //Si solo hay un char, significa que se mueve SOLO un objeto en esa secuencia, si hay dos o más significa que ese otro char es un gameObject más. Si en el char hay algo que no sea numero significa que se saltea ese objeto.
    [SerializeField] private float[] speedOfEachSequences;

    private List<(GameObject , Vector3, Vector3,int)> animationsOnReproduce;
    private float progressionAnimation;


    private int sequencesCounter; //Contador de secuencias completadas
    private int transformsIn; //Contador de los transforms usados

    private Action executeAfterFinishAnimation;
    [SerializeField] private bool resetPositionsAtEnd;
    private List<Vector3> originalsPositions;

    public Action _executeAfterFinishAnimation { set { executeAfterFinishAnimation = value; } }

    private void OnEnable()
    {
        sequencesCounter = 0;
        progressionAnimation = 0;
        transformsIn = 0;
        animationsOnReproduce.Clear();
    }

    private void Awake()
    {
        animationsOnReproduce = new List<(GameObject, Vector3, Vector3,int)>();

        originalsPositions = new List<Vector3>();
    }

    void Update()
    {
        if (animationsOnReproduce.Count > 0)
        {
            for (int i = 0; i < animationsOnReproduce.Count; i++)
            {
                MoveObject(i);
            }
        }
    }

    public void ReproduceAnimation()
    {
        if (sequencesCounter == 0 && resetPositionsAtEnd == true)
        {
            for (int i = 0; i < gameObjectsToMove.Length; i++)
            {
                originalsPositions.Add(gameObjectsToMove[i].transform.position);
            }
        }

        for (int i = 0; i < sequencesAnimation[sequencesCounter].Length; i++) //Cualquier letra es convertido a -1
        { 
            if ((int)Char.GetNumericValue(sequencesAnimation[sequencesCounter][i]) != -1)
            {
                InitMoveObjects(gameObjectsToMove[(int)Char.GetNumericValue(sequencesAnimation[sequencesCounter][i])], transformsIn);
                transformsIn++;
            }
        }
    }

    private void InitMoveObjects(GameObject objectToMove, int numberOfAnimation)
    {
        Vector3 originalPosition = objectToMove.transform.position;
        Vector3 toMove = positionsToMove[numberOfAnimation].position;

        animationsOnReproduce.Add((objectToMove, originalPosition, toMove, sequencesCounter));
    }

    private void MoveObject(int index)
    {
        animationsOnReproduce[index].Item1.transform.position = new Vector3(Mathf.Lerp(animationsOnReproduce[index].Item2.x, animationsOnReproduce[index].Item3.x, progressionAnimation), Mathf.Lerp(animationsOnReproduce[index].Item2.y, animationsOnReproduce[index].Item3.y, progressionAnimation), Mathf.Lerp(animationsOnReproduce[index].Item2.z, animationsOnReproduce[index].Item3.z, progressionAnimation));
        progressionAnimation += speedOfEachSequences[animationsOnReproduce[index].Item4] * Time.deltaTime;

        if (progressionAnimation >= 1)
        {
            animationsOnReproduce.RemoveAt(index);

            if (sequencesCounter < sequencesAnimation.Length - 1 && animationsOnReproduce.Count == 0)
            {
                Debug.Log("Me mueveteo");
                progressionAnimation = 0;
                sequencesCounter++;
                ReproduceAnimation();
            }
            else if (sequencesCounter == sequencesAnimation.Length - 1)
            {
                if (resetPositionsAtEnd == true)
                {
                    for (int i = 0; i < gameObjectsToMove.Length; i++) 
                    {
                        gameObjectsToMove[i].transform.position = originalsPositions[i];
                        Debug.Log("Te reseteo");
                    }
                }
                executeAfterFinishAnimation();
            }
        }
    }
}
