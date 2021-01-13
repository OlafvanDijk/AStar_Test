using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour, IAStarNode
{
    public IEnumerable<IAStarNode> Neighbours => neighbours;

    [SerializeField] private float cost;
    [SerializeField] private bool canBeCrossed;
    [SerializeField] private Vector2Int coordinates;

    private List<Tile> neighbours = new List<Tile>();
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

    public void SetTileInfo(float cost, bool canBeCrossed, Material terrainMaterial, Vector2Int coordinates)
    {
        this.cost = cost;
        this.canBeCrossed = canBeCrossed;
        this.coordinates = coordinates;
        renderer.material = terrainMaterial;
    }

    private float GetCost()
    {
        return cost;
    }

    private bool CanBeCrossed()
    {
        return canBeCrossed;
    }

    private Vector2Int GetCoordinates()
    {
        return coordinates;
    }
}
