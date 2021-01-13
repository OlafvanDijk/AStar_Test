using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;

public class Tile : MonoBehaviour, IAStarNode
{
    public IEnumerable<IAStarNode> Neighbours => neighbours;

    //Todo x,y

    private List<Tile> neighbours = new List<Tile>();
    private float cost;

    public float GetCost()
    {
        return cost;
    }

    public void AddNeighbours(List<Tile> neighbours)
    {
        this.neighbours.AddRange(neighbours);
    }

    public float CostTo(IAStarNode neighbour)
    {
        Tile nb = neighbour as Tile;
        return nb.GetCost();
    }

    public float EstimatedCostTo(IAStarNode goal)
    {
        // Compare x, y value of goal with own
        // Lower Cost = higher possibility
        // Closer to target = higher possibility
        // Water is nono

        return 0;
    }

    /*public void Select()
    {
        selected = !selected;

        Vector3 newPos;

        if (selected)
        {
            newPos = Vector3.up;
        }
        else
        {
            newPos = -Vector3.up;
        }

        foreach (Tile neighbour in neighbours)
        {
            neighbour.transform.position += newPos;
        }
    }*/
}
