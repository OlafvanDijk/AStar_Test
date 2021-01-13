using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoutePooler
{
    private Queue routePool = new Queue();
    private List<GameObject> routeTiles = new List<GameObject>();

    /// <summary>
    /// Enqueue all active routeTiles
    /// </summary>
    public void EnqueueAll()
    {
        if (routeTiles.Count <= 0)
            return;

        foreach (GameObject routeTile in routeTiles)
        {
            Enqueue(routeTile);
        }

        routeTiles.Clear();
    }
    
    /// <summary>
    /// Enqueue given routeTile
    /// </summary>
    /// <param name="routeTile">Route Tile to Enqueue</param>
    public void Enqueue(GameObject routeTile)
    {
        routeTile.SetActive(false);
        routePool.Enqueue(routeTile);
    }

    /// <summary>
    /// Get the first available routeTile
    /// </summary>
    /// <returns></returns>
    public GameObject Dequeue()
    {
        try
        {
            GameObject routeTile = routePool.Dequeue() as GameObject;
            routeTile.SetActive(true);
            routeTiles.Add(routeTile);
            return routeTile;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
        
    }
}
