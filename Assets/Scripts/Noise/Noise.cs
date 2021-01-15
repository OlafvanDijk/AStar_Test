using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    [Range(1, 1200)]
    [SerializeField] private int mapWidthAndHeight = 104;
    [Range(0.1f, 100)]
    [SerializeField] private float noiseScale = 27f;
    [Range(0, 10)]
    [SerializeField] private int octaves = 5;
    [Range(0, 1)]
    [SerializeField] private float persistance = 0.5f;
    [Range(1, 100)]
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private Vector2 offset;

    /// <summary>
    /// Generates a noise map based on the given seed and the class parameters.
    /// </summary>
    /// <param name="seed">Seed to ensure different or same NoiseMaps</param>
    /// <returns></returns>
    public float[,] GenerateNoiseMap(int seed)
    {
        System.Random random = new System.Random (seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(-100000, 100000) + offset.x;
            float offsetY = random.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noiseMap = new float[mapWidthAndHeight, mapWidthAndHeight];

        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidthAndHeight / 2f;

        for (int y = 0; y < mapWidthAndHeight; y++)
        {
            for (int x = 0; x < mapWidthAndHeight; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfWidth) / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapWidthAndHeight; y++)
        {
            for (int x = 0; x < mapWidthAndHeight; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    /// <summary>
    /// Get the Width and Height of the noise map
    /// </summary>
    /// <returns>Width and Height of the noise map</returns>
    public int GetWidthAndHeight()
    {
        return mapWidthAndHeight;
    }
}
