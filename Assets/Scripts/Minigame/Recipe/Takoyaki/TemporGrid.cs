using System.Collections.Generic;
using UnityEngine;

public class TemporGrid : MonoBehaviour
{
    [SerializeField] private TemporNode initialNode;

    [SerializeField] private TemporNode endNode;

    [SerializeField] private List <TemporNode> transitionalsNodes;

    [SerializeField] private Material temporMaterial;
    [SerializeField] private Material transitionableMaterial;
    [SerializeField] private Material endMaterial;

    [SerializeField] private List <TemporNode> idealorderNodes;
    [SerializeField] private List<SpriteRenderer> arrows;

    private int counterArrow = 0;

    public Material _transitionableMaterial => transitionableMaterial;
    public Material _temporMaterial => temporMaterial;
    public Material _endMaterial => endMaterial;
    public TemporNode _endNode => endNode;
    public List <TemporNode> _transitionalsNodes => transitionalsNodes;

    public TemporNode _initialNode => initialNode;
    public int _counterArrow => counterArrow;

    public void SetTransitionalsNodes(bool status)
    {
        counterArrow = 0;
        DisableAllArrows();

        foreach (TemporNode node in transitionalsNodes)
        {
            node.WasTransitioned(false);

            if (node == endNode && status == true)
            {
                node.SetNodeStatus(status, true);
            }
            else
            {
                node.SetNodeStatus(status, false);
            }
        }
    }

    public void SetPlayerOnInitial(PlayerOnNode player)
    {
        player.InstantMove(initialNode);
        initialNode.WasTransitioned(true);
        EnableArrow();
    }

    public bool CheckComplete()
    {
        int nodesToCheck = transitionalsNodes.Count;

        foreach (TemporNode node in transitionalsNodes)
        {
            if (node._hasBeenTransitioned == true)
            {
                nodesToCheck--;
            }
        }

        if (nodesToCheck == 0) 
        {
            return true;
        }
        
        return false;
    }

    public int CheckPlayerNodePosition(TemporNode playerNode)
    {
        return transitionalsNodes.IndexOf(playerNode);
    }

    public void EnableArrow()
    {
        if (counterArrow > 0)
        {
            arrows[counterArrow - 1].enabled = false;
        }

        arrows[counterArrow].enabled = true;

        counterArrow++;
    }

    public void DisableAllArrows()
    {
        foreach (SpriteRenderer sprite in arrows)
        {
            sprite.enabled = false;
        }
    }
}