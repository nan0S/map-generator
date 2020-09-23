using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour 
{
	#region Variables

	public enum DrawMode
	{	
		NoiseMap, 
		ColorMap,
		Mesh
	};

	public MapDisplay mapDisplay;
	public GameObject plane;
	public GameObject mesh;

	const int mapChunkSize = 241;
	[Range(0, 6)]
	public int levelOfDetail;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public bool autoUpdate;

	public DrawMode drawMode;

	public TerrainType[] terrainTypes;

	public float meshHeightMultiplier;
	public AnimationCurve heightCurve;

	#endregion

	public void GenerateMap()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		if (drawMode == DrawMode.NoiseMap)
		{
			mesh.SetActive(false);
			plane.SetActive(true);

			Texture2D texture = TextureGenerator.GenerateTextureFromNoiseMap(noiseMap);
			mapDisplay.DrawTexture(texture);
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			mesh.SetActive(false);
			plane.SetActive(true);

			Texture2D texture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(noiseMap), mapChunkSize, mapChunkSize);
			mapDisplay.DrawTexture(texture);
		}
		else if (drawMode == DrawMode.Mesh)
		{
			mesh.SetActive(true);
			plane.SetActive(false);

			Texture2D texture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(noiseMap), mapChunkSize, mapChunkSize);
			MeshData meshData = MeshGenerator.GenerateMeshFromNoiseMap(noiseMap, meshHeightMultiplier, heightCurve, levelOfDetail);

			mapDisplay.DrawMesh(meshData, texture);
		}
	}

	private Color[] GenerateColorMap (float[,] noiseMap)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		Color[] colorMap = new Color[width * height];

		for (int x = 0; x < width; ++x)
			for (int y = 0; y < height; ++y)
			{
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < terrainTypes.Length; ++i)
					if (currentHeight <= terrainTypes[i].height)
					{
						colorMap[y * width + x] = terrainTypes[i].color;
						break;
					}
			}

		return colorMap;
	}

	private void OnValidate()
	{
		if (lacunarity < 1)
			lacunarity = 1;
		if (octaves < 1)
			octaves = 1;
	}

	[System.Serializable]
	public struct TerrainType
	{
		public string name;
		public float height;
		public Color color;
	};
}
