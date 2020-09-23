using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
	private static Vector2[] octaveOffsets;
	private static System.Random prng;

	public static float[,] GenerateNoiseMap (int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		CalculateRandomSeed(seed);
		CalculateOctaves(octaves, offset);

		return GetNewNoiseMap(mapWidth, mapHeight, scale, octaves, persistance, lacunarity);
	}

	private static void CalculateRandomSeed(int seed)
	{
		prng = new System.Random(seed);
	}

	private static void CalculateOctaves (int octaves, Vector2 offset)
	{
		octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; ++i)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}
	}

	private static float[,] GetNewNoiseMap (int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		float minNoiseValue = float.MaxValue;
		float maxNoiseValue = float.MinValue;

		int halfWidth = mapWidth / 2;
		int halfHeight = mapHeight / 2;

		for (int x = 0; x < mapWidth; ++x)
			for (int y = 0; y < mapHeight; ++y)
			{
				float perlinValue = PerlinNoise(x - halfWidth, y - halfHeight, scale, octaves, persistance, lacunarity);
				noiseMap[x, y] = perlinValue;

				if (perlinValue > maxNoiseValue)
					maxNoiseValue = perlinValue;
				if (perlinValue < minNoiseValue)
					minNoiseValue = perlinValue;
			}

		for (int x = 0; x < mapWidth; ++x)
			for (int y = 0; y < mapHeight; ++y)
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noiseMap[x, y]);

		return noiseMap;
	}

	private static float PerlinNoise(int x, int y, float scale, int octaves, float persistance, float lacunarity)
	{
		float amplitude = 1;
		float frequency = 1;
		float noiseValue = 0;
		float revScale = 1 / scale;

		for (int i = 0; i < octaves; ++i)
		{
			float xCoord = x * revScale * frequency + octaveOffsets[i].x;
			float yCoord = y * revScale * frequency + octaveOffsets[i].y;

			float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
			noiseValue += perlinValue * amplitude;

			amplitude *= persistance;
			frequency *= lacunarity;
		}

		return noiseValue;
	}
}
