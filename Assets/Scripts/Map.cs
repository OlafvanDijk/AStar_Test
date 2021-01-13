using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private List<TerrainObject> availableTiles;

    [Header("Grid")]
    [SerializeField] private Vector3 gridSpacing;
    [SerializeField] private Vector2Int gridSize;

    private Tile[,] tiles;

    // Start is called before the first frame update
    void Awake()
    {
        tiles = new Tile[gridSize.x, gridSize.y];
        CreateMap();
        SetNeighbours();
    }

    private void CreateMap()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject tileObject = Instantiate(tilePrefab, tilesParent);

                float newY = y * gridSpacing.y;
                float newX = x;
                if (IsEven(y))
                {
                    newX += gridSpacing.z;
                }

                tileObject.name = $"Hexagon {y}-{x}";
                tileObject.transform.position = new Vector3(newX, 0, newY);

                Tile currentTile = tileObject.GetComponent<Tile>();
                tiles[x, y] = currentTile;

            }
        }
    }

    private void SetNeighbours()
    {

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int left = new Vector2Int(-1, 0);
                Vector2Int right = new Vector2Int(1, 0);
                Vector2Int upperLeft, upperRight, bottomLeft, bottomRight;

                if (IsEven(y))
                {
                    upperLeft = new Vector2Int(0, 1); //UpperLeft = y + 1
                    upperRight = new Vector2Int(1, 1); //UpperRight = Y+1 en x+1
                    bottomLeft = new Vector2Int(0, -1); //BottomLeft = y - 1
                    bottomRight = new Vector2Int(1, -1);//BottomRight = Y-1 en x+1
                }
                else
                {
                    upperLeft = new Vector2Int(-1, 1); //UpperLeft = y + 1 en x-1
                    upperRight = new Vector2Int(0, 1); //UpperRight = Y+1
                    bottomLeft = new Vector2Int(-1, -1); //BottomLeft = y - 1 en x-1
                    bottomRight = new Vector2Int(0, -1); //BottomRight = Y-1 
                }

                List<Tile> neighbours = new List<Tile>();
                AddTile(ref neighbours, left, x, y);
                AddTile(ref neighbours, right, x, y);
                AddTile(ref neighbours, upperLeft, x, y);
                AddTile(ref neighbours, upperRight, x, y);
                AddTile(ref neighbours, bottomLeft, x, y);
                AddTile(ref neighbours, bottomRight, x, y);

                Tile currentTile = tiles[x, y];
                SetTerrain(currentTile, new Vector2Int(x, y));
                currentTile.AddNeighbours(neighbours);
            }
        }
    }

    private bool IsEven(float number)
    {
        return number % 2 == 0;
    }

    private void AddTile(ref List<Tile> neighbours, Vector2Int index, int currentX, int currentY)
    {
        index.x += currentX;
        index.y += currentY;
        if (index.x < 0 || index.x >= gridSize.x || index.y < 0 || index.y >= gridSize.y)
        {
            return;
        }

        try
        {
            neighbours.Add(tiles[index.x, index.y]);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void SetTerrain(Tile tile, Vector2Int coordinates)
    {
        if (availableTiles.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availableTiles.Count);
            TerrainObject terrain = availableTiles[index];
            tile.SetTileInfo(terrain.cost, terrain.canBeCrossed, terrain.terrainMaterial, coordinates);
        }
    }
}