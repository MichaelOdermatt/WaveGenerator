using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    private static int seed = 243;

    public static float[,] GenerateNoiseMap(
        int mapWidth, int mapHeight, float scale, int octaves, 
        float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Vector2[] ocataveOffsets = generateOctaveOffset(seed, octaves, offset);

        // this prevents a divide by zero error
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + ocataveOffsets[i].x;
                    float sampleY = y / scale * frequency + ocataveOffsets[i].y;

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

        noiseMap = normalizeNoiseMap(noiseMap, maxNoiseHeight, minNoiseHeight);

        return noiseMap;
    }

    private static Vector2[] generateOctaveOffset(int seed, int octaves, Vector2 offset)
    {
        System.Random rand = new System.Random(seed);
        Vector2[] ocataveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offset.x;
            float offsetY = rand.Next(-100000, 100000) + offset.y;
            ocataveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return ocataveOffsets;
    }

    private static float[,] normalizeNoiseMap(float[,] noiseMap, float maxNoiseHeight, float minNoiseHeight)
    {
        for (int y = 0; y < noiseMap.GetLength(0); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(1); x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}

// Sources:
// Gernstner Waves https://www.youtube.com/watch?v=MRNFcywkUSA&ab_channel=SebastianLague
