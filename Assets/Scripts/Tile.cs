using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;
using System;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour, IAStarNode
{
    public IEnumerable<IAStarNode> Neighbours => neighbours;

    private float cost;
    private bool canBeCrossed;
    private Vector2Int coordinates;
    private List<Tile> neighbours = new List<Tile>();

    private Map map;
    private Renderer renderer;

    #region Unity Methods
    /// <summary>
    /// Get Renderer
    /// </summary>
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Add the given neighbours to the neighbours list
    /// </summary>
    /// <param name="neighbours">Neighbouring Tile Nodes</param>
    public void AddNeighbours(List<Tile> neighbours)
    {
        this.neighbours.AddRange(neighbours);
    }

    /// <summary>
    /// Shows the cost of moving to the given neighbour
    /// </summary>
    /// <param name="neighbour"></param>
    /// <returns>Cost of the neighbouring tile.</returns>
    public float CostTo(IAStarNode neighbour)
    {
        Tile tile = neighbour as Tile;
        return tile.GetCost();
    }

    /// <summary>
    /// Estimates the cost from this tile to the given goal tile
    /// </summary>
    /// <param name="goal"></param>
    /// <returns>Estimated cost from this tile to the goal</returns>
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

    /// <summary>
    /// Set the terrain info for the tile
    /// </summary>
    /// <param name="cost">The cost of crossing this tile</param>
    /// <param name="canBeCrossed">Can this tile be crossed or not</param>
    /// <param name="terrainMaterial">Material for how the tile should look</param>
    /// <param name="coordinates">Coordinates in the map</param>
    public void SetTileInfo(float cost, bool canBeCrossed, Material terrainMaterial, Vector2Int coordinates)
    {
        this.cost = cost;
        this.canBeCrossed = canBeCrossed;
        this.coordinates = coordinates;
        renderer.material = terrainMaterial;
    }

    /// <summary>
    /// Returns cost
    /// </summary>
    /// <returns>Cost of the Tile</returns>
    public float GetCost()
    {
        return cost;
    }

    /// <summary>
    /// Returns the canBeCrossed value
    /// </summary>
    /// <returns>Can this tile be crossed</returns>
    public bool CanBeCrossed()
    {
        return canBeCrossed;
    }

    /// <summary>
    /// Returns the coordinates of this tile
    /// </summary>
    /// <returns>coordinates of this tile within the map</returns>
    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    /// <summary>
    /// Set a refference to the map
    /// </summary>
    /// <param name="map">Map that created the tile</param>
    public void SetMap(Map map)
    {
        this.map = map;
    }
    #endregion

    #region Private Methods
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
    #endregion
}
