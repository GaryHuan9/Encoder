using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using CodeHelpers.ThreadHelpers;

namespace CodeHelpers
{
	[Serializable]
	public struct MinAndMax
	{
		public MinAndMax(float min, float max)
		{
			this.min = Mathf.Min(min, max);
			this.max = Mathf.Max(min, max);
		}

		public MinAndMax(float value)
		{
			min = max = value;
		}

		public MinAndMax(params float[] values)
		{
			min = Mathf.Min(values);
			max = Mathf.Max(values);
		}

		public MinAndMax(Vector4 thisVector)
		{
			min = thisVector.MinValue();
			max = thisVector.MaxValue();
		}

		public MinAndMax(Vector3 thisVector)
		{
			min = thisVector.MinValue();
			max = thisVector.MaxValue();
		}

		public MinAndMax(Vector2 thisVector)
		{
			min = thisVector.MinValue();
			max = thisVector.MaxValue();
		}

		public float Min
		{
			get { return min; }
			set
			{
				if (value > max) throw new Exception("Min cannot be larger than max! Please set them correctly!");
				min = value;
			}
		}

		public float Max
		{
			get { return max; }
			set
			{
				if (value < min) throw new Exception("Max cannot be smaller than min! Please set them correctly!");
				max = value;
			}
		}

		float min;
		float max;

		public float RandomValue => ThreadedRandom.Range(min, max);

		public float ClampBetween(float value) => Mathf.Clamp(value, min, max);
		public float LerpBetween(float t) => Mathf.Lerp(min, max, t);
		public Vector2 ToVector2() => new Vector2(min, max);
		public bool IsInRange(float thisFloat) => min <= thisFloat && thisFloat <= max;

		public void Encapsulate(float thisFloat)
		{
			min = Mathf.Min(min, thisFloat);
			max = Mathf.Max(max, thisFloat);
		}

		public static MinAndMax Zero => new MinAndMax(0f, 0f);
		public static MinAndMax One => new MinAndMax(1f, 1f);
		public static MinAndMax ZeroToOne => new MinAndMax(0f, 1f);
		public static MinAndMax OneToOne => new MinAndMax(-1f, 1f);
		public static MinAndMax MinToMax => new MinAndMax(float.MinValue, float.MaxValue);

		public static IEnumerable<MinAndMax> GetRangesFromValue(float value, MinAndMax[] minAndMaxes) => from thisMinAndMax in minAndMaxes
																										 where thisMinAndMax.IsInRange(value)
																										 select thisMinAndMax;

		public static MinAndMax operator +(MinAndMax thisMinAndMax, MinAndMax otherMinAndMax) => new MinAndMax(thisMinAndMax.min + otherMinAndMax.min, thisMinAndMax.max + otherMinAndMax.max);
		public static MinAndMax operator -(MinAndMax thisMinAndMax, MinAndMax otherMinAndMax) => new MinAndMax(thisMinAndMax.min - otherMinAndMax.min, thisMinAndMax.max - otherMinAndMax.max);
		public static MinAndMax operator *(MinAndMax thisMinAndMax, float multiplier) => new MinAndMax(thisMinAndMax.min * multiplier, thisMinAndMax.max * multiplier);
		public static MinAndMax operator /(MinAndMax thisMinAndMax, float divider) => new MinAndMax(thisMinAndMax.min / divider, thisMinAndMax.max / divider);

		public static MinAndMax operator *(MinAndMax thisMinAndMax, MinAndMax otherMinAndMax) => new MinAndMax(thisMinAndMax.min * otherMinAndMax.min, thisMinAndMax.max * otherMinAndMax.max);
		public static MinAndMax operator /(MinAndMax thisMinAndMax, MinAndMax otherMinAndMax) => new MinAndMax(thisMinAndMax.min / otherMinAndMax.min, thisMinAndMax.max / otherMinAndMax.max);

		public static implicit operator MinAndMax(Vector2 vector) => new MinAndMax(vector);
		public static implicit operator MinAndMax(Vector3 vector) => new MinAndMax(vector);
		public static implicit operator MinAndMax(Vector4 vector) => new MinAndMax(vector);

		public static implicit operator Vector2(MinAndMax minAndMax) => minAndMax.ToVector2();

		public override string ToString() => "min : " + min + " max : " + max;
		public override int GetHashCode() => base.GetHashCode();

		public override bool Equals(object obj)
		{
			if (!(obj is MinAndMax)) return false;
			MinAndMax thisMinAndMax = (MinAndMax)obj;
			return Mathf.Approximately(thisMinAndMax.min, min) && Mathf.Approximately(thisMinAndMax.max, max);
		}
	}
}