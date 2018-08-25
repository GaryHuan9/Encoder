using System;
using UnityEngine;
using System.Collections.Generic;

namespace CodeHelpers
{
	public static class VectorHelper
	{
		#region Vector3 Helpers

		public static Vector3 ToXZ3(this Vector2 thisVector, float y = 0) => new Vector3(thisVector.x, y, thisVector.y);
		public static Vector3 ToXZ3(this Vector2Int thisVector, float y) => new Vector3(thisVector.x, y, thisVector.y);
		public static Vector3Int ToXZ3(this Vector2Int thisVector, int y = 0) => new Vector3Int(thisVector.x, y, thisVector.y);

		public static Quaternion ToQuaternion(this Vector3 thisVector) => Quaternion.Euler(thisVector);

		public static Vector3Int Floor(this Vector3 thisVector) => new Vector3Int(Mathf.FloorToInt(thisVector.x), Mathf.FloorToInt(thisVector.y), Mathf.FloorToInt(thisVector.z));

		public static Vector3Int Ceil(this Vector3 thisVector) => new Vector3Int(Mathf.CeilToInt(thisVector.x), Mathf.CeilToInt(thisVector.y), Mathf.CeilToInt(thisVector.z));

		public static Vector3 Round(this Vector3 thisVector) => new Vector3(Mathf.Round(thisVector.x), Mathf.Round(thisVector.y), Mathf.Round(thisVector.z));
		public static Vector3 Round(this Vector3 thisVector, float increment) => new Vector3(Mathf.Round(thisVector.x / increment) * increment, Mathf.Round(thisVector.y / increment) * increment, Mathf.Round(thisVector.z / increment) * increment);

		public static Vector3Int Round(this Vector3 thisVector, int increment) => new Vector3Int(Mathf.RoundToInt(thisVector.x / increment) * increment, Mathf.RoundToInt(thisVector.y / increment) * increment, Mathf.RoundToInt(thisVector.z / increment) * increment);
		public static Vector3Int RoundToInt(this Vector3 thisVector) => new Vector3Int(Mathf.RoundToInt(thisVector.x), Mathf.RoundToInt(thisVector.y), Mathf.RoundToInt(thisVector.z));

		public static Vector3 ToFloat(this Vector3Int thisVector) => new Vector3(thisVector.x, thisVector.y, thisVector.z);
		public static Vector3Int ToInt(this Vector3 thisVector) => new Vector3Int((int)thisVector.x, (int)thisVector.y, (int)thisVector.z);

		public static Vector3 Mod(this Vector3 thisVector, float modValue) => new Vector3(thisVector.x % modValue, thisVector.y % modValue, thisVector.z % modValue);
		public static Vector3 Mod(this Vector3Int thisVector, float modValue) => new Vector3(thisVector.x % modValue, thisVector.y % modValue, thisVector.z % modValue);

		public static Vector3 Mod(this Vector3 thisVector, Vector3 modValue) => new Vector3(thisVector.x % modValue.x, thisVector.y % modValue.y, thisVector.z % modValue.z);
		public static Vector3Int Mod(this Vector3Int thisVector, Vector3Int modValue) => new Vector3Int(thisVector.x % modValue.x, thisVector.y % modValue.y, thisVector.z % modValue.z);

		public static Vector3 Abs(this Vector3 thisVector) => new Vector3(Mathf.Abs(thisVector.x), Mathf.Abs(thisVector.y), Mathf.Abs(thisVector.z));
		public static Vector3Int Abs(this Vector3Int thisVector) => new Vector3Int(Mathf.Abs(thisVector.x), Mathf.Abs(thisVector.y), Mathf.Abs(thisVector.z));

		public static Vector3 AddEpsilon(this Vector3 thisVector, int scale = 1) => thisVector + Vector3.one * CodeHelper.epsilon * scale;

		public static Vector3 Divide(this Vector3 thisVector, Vector3 otherVector) => new Vector3(thisVector.x / otherVector.x, thisVector.y / otherVector.y, thisVector.z / otherVector.z);
		public static Vector3Int Divide(this Vector3Int thisVector, Vector3Int otherVector) => new Vector3Int(thisVector.x / otherVector.x, thisVector.y / otherVector.y, thisVector.z / otherVector.z);

		public static float MaxValue(this Vector3 thisVector) => Mathf.Max(thisVector.x, Mathf.Max(thisVector.y, thisVector.z));
		public static float MinValue(this Vector3 thisVector) => Mathf.Min(thisVector.x, Mathf.Min(thisVector.y, thisVector.z));

		public static int MaxValue(this Vector3Int thisVector) => Mathf.Max(thisVector.x, Mathf.Max(thisVector.y, thisVector.z));
		public static int MinValue(this Vector3Int thisVector) => Mathf.Min(thisVector.x, Mathf.Min(thisVector.y, thisVector.z));

		public static float Size(this Vector3 thisVector) => Mathf.Abs(thisVector.x * thisVector.y * thisVector.z);
		public static int Size(this Vector3Int thisVector) => Mathf.Abs(thisVector.x * thisVector.y * thisVector.z);

		public static void ForEach(this Vector3 thisVector, Action<float> thisAction)
		{
			for (int i = 0; i < 3; i++)
			{
				thisAction(thisVector[i]);
			}
		}

		public static void ForEach(this Vector3 thisVector, Action<float, int> thisAction)
		{
			for (int i = 0; i < 3; i++)
			{
				thisAction(thisVector[i], i);
			}
		}

		public static void ForEach(this Vector3Int thisVector, Action<int> thisAction)
		{
			for (int i = 0; i < 3; i++)
			{
				thisAction(thisVector[i]);
			}
		}

		public static void ForEach(this Vector3Int thisVector, Action<int, int> thisAction)
		{
			for (int i = 0; i < 3; i++)
			{
				thisAction(thisVector[i], i);
			}
		}

		public static void ForEach(this Vector3Int thisVector, Action<Vector3Int> thisAction)
		{
			Vector3Int absVector = thisVector.Abs();
			Vector3Int direction = new Vector3Int(
				thisVector.x.Normalized(),
				thisVector.y.Normalized(),
				thisVector.z.Normalized()
			);

			for (int x = 0; x <= absVector.x; x++)
			{
				for (int y = 0; y <= absVector.y; y++)
				{
					for (int z = 0; z <= absVector.z; z++)
					{
						thisAction(Vector3Int.Scale(new Vector3Int(x, y, z), direction));
					}
				}
			}
		}

		public static void ForEach(this Vector3Int thisVector, Action<Vector3Int, int> thisAction)
		{
			int index = 0;

			Vector3Int absVector = thisVector.Abs();
			Vector3Int direction = new Vector3Int(
				thisVector.x.Normalized(),
				thisVector.y.Normalized(),
				thisVector.z.Normalized()
			);

			for (int x = 0; x <= absVector.x; x++)
			{
				for (int y = 0; y <= absVector.y; y++)
				{
					for (int z = 0; z <= absVector.z; z++)
					{
						thisAction(Vector3Int.Scale(new Vector3Int(x, y, z), direction), index);
						index++;
					}
				}
			}
		}

		public static bool LessThan(this Vector3 thisVector, Vector3 otherVector) => thisVector.x < otherVector.x && thisVector.y < otherVector.y && thisVector.z < otherVector.z;
		public static bool GreaterThan(this Vector3 thisVector, Vector3 otherVector) => thisVector.x > otherVector.x && thisVector.y > otherVector.y && thisVector.z > otherVector.z;
		public static bool LessThanOrEquals(this Vector3 thisVector, Vector3 otherVector) => thisVector.x <= otherVector.x && thisVector.y <= otherVector.y && thisVector.z <= otherVector.z;
		public static bool GreaterThanOrEquals(this Vector3 thisVector, Vector3 otherVector) => thisVector.x >= otherVector.x && thisVector.y >= otherVector.y && thisVector.z >= otherVector.z;

		public static bool LessThan(this Vector3Int thisVector, Vector3Int otherVector) => thisVector.x < otherVector.x && thisVector.y < otherVector.y && thisVector.z < otherVector.z;
		public static bool GreaterThan(this Vector3Int thisVector, Vector3Int otherVector) => thisVector.x > otherVector.x && thisVector.y > otherVector.y && thisVector.z > otherVector.z;
		public static bool LessThanOrEquals(this Vector3Int thisVector, Vector3Int otherVector) => thisVector.x <= otherVector.x && thisVector.y <= otherVector.y && thisVector.z <= otherVector.z;
		public static bool GreaterThanOrEquals(this Vector3Int thisVector, Vector3Int otherVector) => thisVector.x >= otherVector.x && thisVector.y >= otherVector.y && thisVector.z >= otherVector.z;

		public static void Swap(this Vector3 thisVector, int index1, int index2)
		{
			float storage = thisVector[index1];
			thisVector[index1] = thisVector[index2];
			thisVector[index2] = storage;
		}

		public static void Swap(this Vector3Int thisVector, int index1, int index2)
		{
			int storage = thisVector[index1];
			thisVector[index1] = thisVector[index2];
			thisVector[index2] = storage;
		}

		public static readonly Vector3 epsilonVector3 = Vector3.one * CodeHelper.epsilon;

		public static readonly Vector3 maxVector3 = Vector3.one * float.MaxValue;
		public static readonly Vector3 minVector3 = Vector3.one * float.MinValue;

		#endregion

		#region Vector2 Helpers

		public static Vector2 ToXZ2(this Vector3 thisVector) => new Vector2(thisVector.x, thisVector.z);
		public static Vector2Int ToXZ2(this Vector3Int thisVector) => new Vector2Int(thisVector.x, thisVector.z);

		public static Vector2Int Floor(this Vector2 thisVector) => new Vector2Int(Mathf.FloorToInt(thisVector.x), Mathf.FloorToInt(thisVector.y));
		public static Vector2Int Ceil(this Vector2 thisVector) => new Vector2Int(Mathf.CeilToInt(thisVector.x), Mathf.CeilToInt(thisVector.y));

		public static Vector2 Round(this Vector2 thisVector) => new Vector2(Mathf.Round(thisVector.x), Mathf.Round(thisVector.y));
		public static Vector2 Round(this Vector2 thisVector, float increment) => new Vector2(Mathf.Round(thisVector.x / increment) * increment, Mathf.Round(thisVector.y / increment) * increment);
		public static Vector2Int Round(this Vector2 thisVector, int increment) => new Vector2Int(Mathf.RoundToInt(thisVector.x / increment) * increment, Mathf.RoundToInt(thisVector.y / increment) * increment);
		public static Vector2Int RoundToInt(this Vector2 thisVector) => new Vector2Int(Mathf.RoundToInt(thisVector.x), Mathf.RoundToInt(thisVector.y));

		public static Vector2 ToFloat(this Vector2Int thisVector) => new Vector2(thisVector.x, thisVector.y);
		public static Vector2Int ToInt(this Vector2 thisVector) => new Vector2Int((int)thisVector.x, (int)thisVector.y);

		public static int To1D(this Vector2Int thisVector, int height) => thisVector.y + thisVector.x * height;

		public static Vector2 Rotate(this Vector2 thisVector, float angle)
		{
			float angleInRad = angle * Mathf.Deg2Rad;
			float sin = Mathf.Sin(angleInRad);
			float cos = Mathf.Cos(angleInRad);

			return new Vector2(cos * thisVector.x - sin * thisVector.y, sin * thisVector.x + cos * thisVector.y);
		}

		public static Vector2 Rotate(this Vector2 thisVector, float angle, Vector2 pivot) => (thisVector - pivot).Rotate(angle) + pivot;
		public static Vector2 Rotate(this Vector2Int thisVector, float angle) => Rotate(thisVector.ToFloat(), angle);
		public static Vector2 Rotate(this Vector2Int thisVector, float angle, Vector2 pivot) => (thisVector - pivot).Rotate(angle) + pivot;

		public static Vector2 Mod(this Vector2 thisVector, float modValue) => new Vector2(thisVector.x % modValue, thisVector.y % modValue);
		public static Vector2 Mod(this Vector2Int thisVector, float modValue) => new Vector2(thisVector.x % modValue, thisVector.y % modValue);
		public static Vector2 Mod(this Vector2 thisVector, Vector2 modValue) => new Vector2(thisVector.x % modValue.x, thisVector.y % modValue.y);
		public static Vector2Int Mod(this Vector2Int thisVector, Vector2Int modValue) => new Vector2Int(thisVector.x % modValue.x, thisVector.y % modValue.y);

		public static Vector2 Clamp(this Vector2 thisVector, Vector2 minVector, Vector2 maxVector) => new Vector2(Mathf.Clamp(thisVector.x, minVector.x, maxVector.x), Mathf.Clamp(thisVector.y, minVector.y, maxVector.y));
		public static Vector2Int Clamp(this Vector2Int thisVector, Vector2Int minVector, Vector2Int maxVector) => new Vector2Int(Mathf.Clamp(thisVector.x, minVector.x, maxVector.x), Mathf.Clamp(thisVector.y, minVector.y, maxVector.y));

		public static Vector2 InverseLerp(this Vector2 thisVector, Vector2 vector1, Vector2 vector2) => new Vector2(Mathf.InverseLerp(thisVector.x, vector1.x, vector2.x), Mathf.InverseLerp(thisVector.y, vector1.y, vector2.y));

		public static Vector2 Abs(this Vector2 thisVector) => new Vector2(Mathf.Abs(thisVector.x), Mathf.Abs(thisVector.y));
		public static Vector2Int Abs(this Vector2Int thisVector) => new Vector2Int(Mathf.Abs(thisVector.x), Mathf.Abs(thisVector.y));

		public static Vector2 AddEpsilon(this Vector2 thisVector, int scale = 1) => thisVector + Vector2.one * Mathf.Epsilon * scale;

		public static Vector2 Divide(this Vector2 thisVector, Vector2 otherVector) => new Vector2(thisVector.x / otherVector.x, thisVector.y / otherVector.y);
		public static Vector2Int Divide(this Vector2Int thisVector, Vector2Int otherVector) => new Vector2Int(thisVector.x / otherVector.x, thisVector.y / otherVector.y);

		public static Vector2 SwapXY(this Vector2 thisVector) => new Vector2(thisVector.y, thisVector.x);
		public static Vector2Int SwapXY(this Vector2Int thisVector) => new Vector2Int(thisVector.x, thisVector.y);

		public static float MaxValue(this Vector2 thisVector) => Mathf.Max(thisVector.x, thisVector.y);
		public static float MinValue(this Vector2 thisVector) => Mathf.Min(thisVector.x, thisVector.y);

		public static int MaxValue(this Vector2Int thisVector) => Mathf.Max(thisVector.x, thisVector.y);
		public static int MinValue(this Vector2Int thisVector) => Mathf.Min(thisVector.x, thisVector.y);

		public static float Size(this Vector2 thisVector) => Mathf.Abs(thisVector.x * thisVector.y);
		public static int Size(this Vector2Int thisVector) => Mathf.Abs(thisVector.x * thisVector.y);

		public static void ForEach(this Vector2 thisVector, Action<float> thisAction)
		{
			for (int i = 0; i < 2; i++)
			{
				thisAction(thisVector[i]);
			}
		}

		public static void ForEach(this Vector2 thisVector, Action<float, int> thisAction)
		{
			for (int i = 0; i < 2; i++)
			{
				thisAction(thisVector[i], i);
			}
		}

		public static void ForEach(this Vector2Int thisVector, Action<int> thisAction)
		{
			for (int i = 0; i < 2; i++)
			{
				thisAction(thisVector[i]);
			}
		}

		public static void ForEach(this Vector2Int thisVector, Action<int, int> thisAction)
		{
			for (int i = 0; i < 2; i++)
			{
				thisAction(thisVector[i], i);
			}
		}

		public static void ForEach(this Vector2Int thisVector, Action<Vector2Int> thisAction)
		{
			Vector2Int absVector = thisVector.Abs();
			Vector2Int direction = new Vector2Int(
				thisVector.x.Normalized(),
				thisVector.y.Normalized()
			);

			for (int x = 0; x <= absVector.x; x++)
			{
				for (int y = 0; y <= absVector.y; y++)
				{
					thisAction(Vector2Int.Scale(new Vector2Int(x, y), direction));
				}
			}
		}

		public static void ForEach(this Vector2Int thisVector, Action<Vector2Int, int> thisAction)
		{
			int index = 0;

			Vector2Int absVector = thisVector.Abs();
			Vector2Int direction = new Vector2Int(
				thisVector.x.Normalized(),
				thisVector.y.Normalized()
			);

			for (int x = 0; x <= absVector.x; x++)
			{
				for (int y = 0; y <= absVector.y; y++)
				{
					thisAction(Vector2Int.Scale(new Vector2Int(x, y), direction), index);
					index++;
				}
			}
		}

		public static bool LessThan(this Vector2 thisVector, Vector2 otherVector) => thisVector.x < otherVector.x && thisVector.y < otherVector.y;
		public static bool GreaterThan(this Vector2 thisVector, Vector2 otherVector) => thisVector.x > otherVector.x && thisVector.y > otherVector.y;
		public static bool LessThanOrEquals(this Vector2 thisVector, Vector2 otherVector) => thisVector.x <= otherVector.x && thisVector.y <= otherVector.y;
		public static bool GreaterThanOrEquals(this Vector2 thisVector, Vector2 otherVector) => thisVector.x >= otherVector.x && thisVector.y >= otherVector.y;

		public static bool LessThan(this Vector2Int thisVector, Vector2Int otherVector) => thisVector.x < otherVector.x && thisVector.y < otherVector.y;
		public static bool GreaterThan(this Vector2Int thisVector, Vector2Int otherVector) => thisVector.x > otherVector.x && thisVector.y > otherVector.y;
		public static bool LessThanOrEquals(this Vector2Int thisVector, Vector2Int otherVector) => thisVector.x <= otherVector.x && thisVector.y <= otherVector.y;
		public static bool GreaterThanOrEquals(this Vector2Int thisVector, Vector2Int otherVector) => thisVector.x >= otherVector.x && thisVector.y >= otherVector.y;

		public static readonly Vector2 epsilonVector2 = Vector2.one * CodeHelper.epsilon;

		public static readonly Vector2 maxVector2 = Vector2.one * float.MaxValue;
		public static readonly Vector2 minVector2 = Vector2.one * float.MinValue;

		#region Get Positions

		public static IEnumerable<Vector2Int> GetPositionsFromCircle(float radius)
		{
			radius -= 0.0001f;

			List<Vector2Int> result = new List<Vector2Int>();
			int radiusInt = Mathf.CeilToInt(radius);
			float radiusSquared = Mathf.Pow(radius, 2);

			for (int x = -radiusInt; x <= radiusInt; x++)
			{
				for (int y = -radiusInt; y <= radiusInt; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					if (position.sqrMagnitude <= radiusSquared) result.Add(position);
				}
			}

			return result;
		}

		//All the positions are included in the result
		public static IEnumerable<Vector2Int> GetPositionsFromRectangle(Vector2Int position1, Vector2Int position2)
		{
			Vector2Int minPosition = Vector2Int.Min(position1, position2);
			Vector2Int maxPosition = Vector2Int.Max(position1, position2);
			List<Vector2Int> thesePositions = new List<Vector2Int>();

			for (int x = minPosition.x; x <= maxPosition.x; x++)
			{
				for (int y = minPosition.y; y <= maxPosition.y; y++)
				{
					thesePositions.Add(new Vector2Int(x, y));
				}
			}

			return thesePositions;
		}

		public static IEnumerable<Vector2Int> GetPositionsFromRectangle(Vector2Int position, int width, int height)
		{
			return GetPositionsFromRectangle(position, position + new Vector2Int(width, height) - Vector2Int.one);
		}

		public static IEnumerable<Vector2Int> GetPositionsFromLine(Vector2Int position1, Vector2Int position2)
		{
			int sampleCount = Mathf.RoundToInt(new MinAndMax((position1 - position2).Abs()).Max);
			var result = new List<Vector2Int>();

			for (int i = 0; i <= sampleCount; i++) result.Add(Vector2.Lerp(position1, position2, i / sampleCount).RoundToInt());

			return result;
		}

		#endregion

		public static readonly Vector2Int[] Neighbor4Positions = { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };
		public static readonly Vector2Int[] Neighbor8Positions = { Vector2Int.right, new Vector2Int(1, -1), Vector2Int.down, new Vector2Int(-1, -1), Vector2Int.left, new Vector2Int(-1, 1), Vector2Int.up, new Vector2Int(1, 1) };
		public static readonly Vector2Int[] Corner4Positions = { new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1) };
		public static readonly Vector2Int[] Corner4Positions01 = { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.one };

		#endregion

		#region Vector4 Helpers

		public static Vector4 Abs(this Vector4 thisVector) => Vector4.Max(thisVector, -thisVector);

		public static float MaxValue(this Vector4 thisVector) => Mathf.Max(Mathf.Max(thisVector.x, thisVector.y), Mathf.Max(thisVector.z, thisVector.w));
		public static float MinValue(this Vector4 thisVector) => Mathf.Min(Mathf.Min(thisVector.x, thisVector.y), Mathf.Min(thisVector.z, thisVector.w));

		public static float Size(this Vector4 thisVector) => Mathf.Abs(thisVector.x * thisVector.y * thisVector.z * thisVector.w);

		public static void ForEach(this Vector4 thisVector, Action<float> thisAction)
		{
			for (int i = 0; i < 4; i++)
			{
				thisAction(thisVector[i]);
			}
		}

		public static void ForEach(this Vector4 thisVector, Action<float, int> thisAction)
		{
			for (int i = 0; i < 4; i++)
			{
				thisAction(thisVector[i], i);
			}
		}

		public static void Swap(this Vector4 thisVector, int index1, int index2)
		{
			float storage = thisVector[index1];
			thisVector[index1] = thisVector[index2];
			thisVector[index2] = storage;
		}

		public static readonly Vector4 epsilonVector4 = Vector4.one * CodeHelper.epsilon;

		public static readonly Vector4 maxVector4 = Vector4.one * float.MaxValue;
		public static readonly Vector4 minVector4 = Vector4.one * float.MinValue;

		#endregion
	}
}
