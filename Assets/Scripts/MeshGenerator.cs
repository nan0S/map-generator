using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator 
{
	public static MeshData GenerateMeshFromNoiseMap(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
	{
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		Vector2 topLeft = new Vector2((width - 1) / -2f, (height - 1) / 2f);

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
		int vertexIndex = 0;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

		for (int x = 0; x < width; x += meshSimplificationIncrement)
			for (int y = 0; y < height; y += meshSimplificationIncrement)
			{
				float heightMap = heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier;
			
				meshData.vertices[vertexIndex] = new Vector3(topLeft.x + x, heightMap, topLeft.y - y);
				meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1)
				{
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + verticesPerLine + 1);
				}

				++vertexIndex;
			}

		return meshData;
	}
}

public class MeshData
{
	public Vector3[] vertices;
	public Vector2[] uvs;
	public int[] triangles;

	int trianglesIndex = 0;

	public MeshData(int width, int height)
	{
		vertices = new Vector3[width * height];
		uvs = new Vector2[width * height];
		triangles = new int[(width - 1) * (height - 1) * 6];
	}

	public void AddTriangle(int a, int b, int c)
	{
		triangles[trianglesIndex + 0] = c;
		triangles[trianglesIndex + 1] = b;
		triangles[trianglesIndex + 2] = a;
		trianglesIndex += 3;
	}

	public Mesh GetMesh()
	{
		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		return mesh;
	}
}
