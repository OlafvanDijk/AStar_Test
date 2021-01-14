using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;

public class Select : MonoBehaviour
{
    [Header("Route")]
    [SerializeField] private GameObject routePrefab;
    [SerializeField] private Material goalMaterial;
    [SerializeField] private Material routeMaterial;
    [SerializeField] private int routePoolSize = 16;
    [SerializeField] private Transform routeParent;

    private Tile start;
    private Tile goal;
    private RoutePooler routePooler;

    /// <summary>
    /// Create Route Tiles and Enqueue them.
    /// </summary>
    private void Awake()
    {
        routePooler = new RoutePooler();
        for (int i = 0; i < routePoolSize; i++)
        {
            GameObject route = Instantiate(routePrefab, routeParent);
            routePooler.Enqueue(route);
        }
    }

    /// <summary>
    /// Select a Tile when clicking.
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                Tile selected = hitInfo.collider.GetComponent<Tile>();
                if (selected && selected.CanBeCrossed())
                {
                    SelectTile(selected);
                }
            }
        }
    }

    /// <summary>
    /// Select the given tile as start or goal.
    /// This method also calls the spawning of the Route Tiles.
    /// </summary>
    /// <param name="selected">Selected tile</param>
    private void SelectTile(Tile selected)
    {
        if (!start)
        {
            routePooler.EnqueueAll();

            start = selected;
            SpawnRouteTile(start.transform.position, true);
            goal = null;
        }
        else if (!goal)
        {
            if (selected != start)
            {
                goal = selected;
                SpawnRouteTile(goal.transform.position, true);
                ShowRoute();
                start = null;
            }
        }
    }

    private void ShowRoute()
    {
        float cost = 0;
        IList<IAStarNode> route = AStar.GetPath(start, goal);
        for (int i = 0; i < route.Count; i++)
        {
            Tile tile = route[i] as Tile;
            if (i == 0)
            {
                continue;
            }
            else if (i == route.Count - 1)
            {
                cost += tile.GetCost();
                continue;
            }
            SpawnRouteTile(tile.transform.position, false);
            cost += tile.GetCost();
        }

        //Debug.Log("Estimated cost: " + navigation.EstimatedCost(start, goal));
        Debug.Log("Route cost: " + cost);
    }

    /// <summary>
    /// Spawns a RouteTile on the given position.
    /// </summary>
    /// <param name="position">Position of path tile</param>
    /// <param name="isStartOrGoal">Detirmens the color of the material</param>
    private void SpawnRouteTile(Vector3 position, bool isStartOrGoal)
    {
        GameObject routeTile = routePooler.Dequeue();
        if (routeTile)
        {
            SetRouteTilePosition(position, routeTile);
            Renderer renderer = routeTile.GetComponent<Renderer>();
            if (isStartOrGoal)
            {
                renderer.material = goalMaterial;
            }
            else
            {
                renderer.material = routeMaterial;
            }
        }
    }

    /// <summary>
    /// Set Position of RouteTile.
    /// </summary>
    /// <param name="tile">Tile below the route</param>
    /// <param name="routeTile">Route Tile to adjust</param>
    private void SetRouteTilePosition(Vector3 tile, GameObject routeTile)
    {
        Vector3 pos = routeTile.transform.position;
        routeTile.transform.position = new Vector3(tile.x, pos.y, tile.z);
    }
}
