using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map : MonoBehaviour
{
    [Serializable]
    private struct AvailableTile
    {
        public TerrainObject terrain;
        [Range(0,1)] public float perlinHeight;
    }

    [Header("Grid")]
    [SerializeField] private Vector3 gridSpacing = new Vector3(1f, 0.74f, 0.5f);
    [SerializeField] private int gridSize = 8;

    [Header("Perlin Noise")]
    [SerializeField] int seed = 3;
    [SerializeField] Noise noiseGenerator;

    [Header("Tiles")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private List<AvailableTile> availableTiles;

    private Tile[,] tiles;
    private float[,] noiseMap;

    /// <summary>
    /// Create Map and set the neighbours.
    /// </summary>
    private void Awake()
    {
        tiles = new Tile[gridSize, gridSize];
        CreateMap();
        SetNeighbours();
    }

    /// <summary>
    /// Creates a hexagon grid and generates the noise to be used later.
    /// </summary>
    private void CreateMap()
    {
        noiseMap = noiseGenerator.GenerateNoiseMap(seed);

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject tileObject = Instantiate(tilePrefab, tilesParent);

                float newY = y * gridSpacing.y;
                float newX = x * gridSpacing.x;
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

    /// <summary>
    /// Set the neighbours for all the tiles.
    /// Cleares the noisemap to free up memory.
    /// </summary>
    private void SetNeighbours()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
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

        noiseMap = null;
    }

    /// <summary>
    /// Check if the given number is even or uneven.
    /// </summary>
    /// <param name="number">Number to check</param>
    /// <returns>True if even, false if uneven</returns>
    private bool IsEven(float number)
    {
        return number % 2 == 0;
    }

    /// <summary>
    /// Add the tile as a neighbour that results from adding the given index to the current index.
    /// </summary>
    /// <param name="neighbours">List of Neighbours</param>
    /// <param name="index">Indexes to add to the current index</param>
    /// <param name="currentX">Current X index</param>
    /// <param name="currentY">Current Y index</param>
    private void AddTile(ref List<Tile> neighbours, Vector2Int index, int currentX, int currentY)
    {
        index.x += currentX;
        index.y += currentY;
        if (index.x < 0 || index.x >= gridSize || index.y < 0 || index.y >= gridSize)
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

    /// <summary>
    /// Set Terrain for the given tile.
    /// Terrain is chosen by a calculation that uses perlin noise.
    /// </summary>
    /// <param name="tile">Tile to give terrain</param>
    /// <param name="coordinates">Coordinates (indexes) of the given tile</param>
    private void SetTerrain(Tile tile, Vector2Int coordinates)
    {
        if (availableTiles.Count > 0)
        {
            coordinates *= noiseGenerator.GetWidthAndHeight() / gridSize;

            float perlinNoise = noiseMap[coordinates.x, coordinates.y];

            int index = 0;
            float previousHeight = 0;

            for (int i = 0; i < availableTiles.Count; i++)
            {
                AvailableTile available = availableTiles[i];
                if (perlinNoise == previousHeight || (perlinNoise > previousHeight && perlinNoise <= available.perlinHeight))
                {
                    index = i;
                    break;
                }
                else
                {
                    previousHeight = available.perlinHeight;
                }
            }       

            Debug.Log("Index value:" + index);

            TerrainObject terrain = availableTiles[index].terrain;
            tile.SetTileInfo(terrain.cost, terrain.canBeCrossed, terrain.terrainMaterial, coordinates);
        }
    }
}