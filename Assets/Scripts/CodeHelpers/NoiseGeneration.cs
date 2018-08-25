using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeHelpers;

namespace CodeHelpers.NoiseGeneration
{
	public static class NoiseGenerationHelper
	{
		#region Methods

		public static float[,] NoiseToArray(float spread, int layerCount, float persistance, float lacunarity, int seed, float[,] heights, Vector2 offsetPosition)
		{
			Vector2Int size = new Vector2Int(heights.GetLength(0), heights.GetLength(1));

			float amplitude = 1;
			float frequency = 1;

			for (int i = 0; i < layerCount; i++)
			{
				for (int x = 0; x < size.x; x++)
				{
					for (int y = 0; y < size.y; y++)
					{
						float coordX = (x + offsetPosition.x) / spread * frequency + seed + i;
						float coordY = (y + offsetPosition.y) / spread * frequency + seed + i;

						if (i == 0) heights[x, y] = 0;

						heights[x, y] += Mathf.PerlinNoise(coordX, coordY) * amplitude;
					}
				}

				amplitude *= persistance;
				frequency *= lacunarity;
			}

			//Set the noise range from 0 to 1
			heights.InverseLerp(0, NoiseInfo.GetMaxHeight(layerCount, persistance));

			return heights;
		}

		public static float[,] NoiseToArray(float spread, int layerCount, float persistance, float lacunarity, int seed, Vector2Int size, Vector2 offsetPosition)
		{
			return NoiseToArray(spread, layerCount, persistance, lacunarity, seed, new float[size.x, size.y], offsetPosition);
		}

		public static float[,] NoiseToArray(NoiseInfo noiseData, Vector2 position, float[,] heights, int seed)
		{
			return NoiseToArray(noiseData.spread, noiseData.LayerCount, noiseData.persistance, noiseData.lacunarity, seed, heights, position);
		}

		public static float[,] NoiseToArray(NoiseInfo noiseData, Vector2 position, Vector2Int size, int seed)
		{
			return NoiseToArray(noiseData.spread, noiseData.LayerCount, noiseData.persistance, noiseData.lacunarity, seed, size, position);
		}

		public static float[] NoiseToArray(float spread, int layerCount, float persistance, float lacunarity, int seed, Vector2[] positions, Vector2 positionOffset, float[] heights)
		{
			float amplitude = 1;
			float frequency = 1;

			for (int i = 0; i < layerCount; i++)
			{
				for (int j = 0; j < positions.Length; j++)
				{
					Vector2 coord = (positions[j] + positionOffset) / spread * frequency + Vector2.one * (seed + i);

					if (i == 0) heights[j] = 0;

					heights[j] += Mathf.PerlinNoise(coord.x, coord.y) * amplitude;
				}

				amplitude *= persistance;
				frequency *= lacunarity;
			}

			//Set the noise range from 0 to 1
			heights.InverseLerp(0, NoiseInfo.GetMaxHeight(layerCount, persistance));

			return heights;
		}

		public static float[] NoiseToArray(NoiseInfo noiseData, Vector2[] positions, Vector2 positionOffset, int seed)
		{
			return NoiseToArray(noiseData.spread, noiseData.LayerCount, noiseData.persistance, noiseData.lacunarity, seed, positions, positionOffset, new float[positions.Length]);
		}

		#endregion

		#region Extensions

		public static void InverseLerp(this float[] thisArray, float a, float b)
		{
			for (int i = 0; i < thisArray.Length; i++)
			{
				thisArray[i] = Mathf.InverseLerp(a, b, thisArray[i]);
			}
		}

		public static void InverseLerp(this float[,] thisArray, float a, float b)
		{
			for (int x = 0; x < thisArray.GetLength(0); x++)
			{
				for (int y = 0; y < thisArray.GetLength(1); y++)
				{
					thisArray[x, y] = Mathf.InverseLerp(a, b, thisArray[x, y]);
				}
			}
		}

		#endregion

	}
}

namespace CodeHelpers
{
	[System.Serializable]
	public struct NoiseInfo
	{
		public NoiseInfo(float spread, int layerCount, float persistance, float lacunarity)
		{
			this.spread = spread;
			this.layerCount = layerCount;
			this.persistance = persistance;
			this.lacunarity = lacunarity;
		}

		public float spread;

		[SerializeField] float layerCount;
		public int LayerCount { get { return Mathf.RoundToInt(layerCount); } set { layerCount = value; } }

		public float persistance;
		public float lacunarity;

		public static readonly NoiseInfo defaultInfo = new NoiseInfo { spread = 1, LayerCount = 1, persistance = 0.5f, lacunarity = 2 };

		internal float MaxHeight { get { return GetMaxHeight(this); } }

		internal static float GetMaxHeight(NoiseInfo thisData)
		{
			float maxheight = 0;
			float amplitude = 1;

			for (int i = 0; i < thisData.LayerCount; i++)
			{
				maxheight += amplitude;
				amplitude *= thisData.persistance;
			}

			return maxheight;
		}

		public static float GetMaxHeight(int layerCount, float persistance)
		{
			float maxheight = 0;
			float amplitude = 1;

			for (int i = 0; i < layerCount; i++)
			{
				maxheight += amplitude;
				amplitude *= persistance;
			}

			return maxheight;
		}

		public static implicit operator Vector4(NoiseInfo thisInfo)
		{
			return new Vector4(thisInfo.spread, thisInfo.LayerCount, thisInfo.persistance, thisInfo.lacunarity);
		}

		public static implicit operator NoiseInfo(Vector4 thisVector)
		{
			return new NoiseInfo(thisVector.x, 0, thisVector.z, thisVector.w) { layerCount = thisVector.y };
		}
	}
}