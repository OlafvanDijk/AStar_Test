using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrain", menuName = "ScriptableObjects/Create New Terrain", order = 1)]
public class TerrainObject : ScriptableObject
{
    public Material terrainMaterial;
    public float cost;
    public bool canBeCrossed;
}