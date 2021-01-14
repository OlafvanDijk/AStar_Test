using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;
using System;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour, IAStarNode, IComparable
{
    public IEnumerable<IAStarNode> Neighbours => neighbours;

    [SerializeField] private float cost;
    [SerializeField] private bool canBeCrossed;
    [SerializeField] private Vector2Int coordinates;

    private List<Tile> neighbours = new List<Tile>();
    private Map map;
    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void AddNeighbours(List<Tile> neighbours)
    {
        this.neighbours.AddRange(neighbours);
    }

    public float CostTo(IAStarNode neighbour)
    {
        Tile tile = neighbour as Tile;
        return tile.GetCost();
    }

    public float EstimatedCostTo(IAStarNode goal)
    {
        float avgCost = map.GetAverageCost();

        Tile goalTile = (Tile)goal;
        Vector2Int currentXY = this.GetCoordinates();
        Vector2Int goalXY = goalTile.GetCoordinates();

        float cost = 0;
        while (!currentXY.Equals(goalXY))
        {
            Move(ref currentXY, goalXY);
            cost += avgCost;
        }

        return avgCost;
    }

    public void SetTileInfo(float cost, bool canBeCrossed, Material terrainMaterial, Vector2Int coordinates)
    {
        this.cost = cost;
        this.canBeCrossed = canBeCrossed;
        this.coordinates = coordinates;
        renderer.material = terrainMaterial;
    }

    public float GetCost()
    {
        return cost;
    }

    public bool CanBeCrossed()
    {
        return canBeCrossed;
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    public void SetMap(Map map)
    {
        this.map = map;
    }

    /// <summary>
    /// Move the given Vector2 towards the goal.
    /// Keeping in mind the diagonal movement of a hexagonal grid.
    /// </summary>
    /// <param name="currentXY">Current postion</param>
    /// <param name="goalXY">Position to be moving towards</param>
    private void Move(ref Vector2Int currentXY, Vector2Int goalXY)
    {
        Vector2Int difference = goalXY - currentXY;
        if (difference.x == 0)
        {
            if (difference.y > 0)
            {
                currentXY.y++;
                return;
            }
            else
            {
                currentXY.y--;
                return;
            }
        }

        if (difference.x > 0)
        {
            currentXY.x += 1;
            UpdateY(ref currentXY, difference, 0, 1);
        }
        else
        {
            currentXY.x -= 1;
            UpdateY(ref currentXY, difference, 1, 0);
        }
    }

    /// <summary>
    /// Update the given currentY with the Even or Uneven Y value
    /// </summary>
    /// <param name="currentXY">CurrentXY position</param>
    /// <param name="difference">Difference from the current position to the goal</param>
    /// <param name="evenY">Value to apply when on a even row.</param>
    /// <param name="unevenY">Value ot apply when on a uneven row.</param>
    private void UpdateY(ref Vector2Int currentXY, Vector2Int difference, int evenY, int unevenY)
    {
        if (difference.y != 0)
        {
            int y = 0;
            if (currentXY.y % 2 == 0)
            {
                y = evenY;
            }
            else
            {
                y = unevenY;
            }

            if (difference.y > 0)
            {
                currentXY.y += y;
            }
            else
            {
                currentXY.y -= y;
            }
        }
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;

        Tile tile = obj as Tile;
        if (tile != null)
        {
            if (tile.cost >= this.cost)
                return 1;
            else
                return -1;
        }
        else
        {
            throw new ArgumentException("Object is not a Tile");
        }
    }
}
