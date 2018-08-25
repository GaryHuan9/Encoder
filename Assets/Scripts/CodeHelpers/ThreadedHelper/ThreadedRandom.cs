using System;
using System.Threading;

namespace CodeHelpers.ThreadHelpers
{
	public static class ThreadedRandom
	{
		static ThreadedRandom()
		{
			seed = Environment.TickCount;
		}

		static int originalSeed;
		static int seed;

		public static int Seed
		{
			get { return originalSeed; }
			set
			{
				originalSeed = value;
				Interlocked.Exchange(ref seed, value);

				thisRandom = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
			}
		}

		public static int ThreadSeed
		{
			set { thisRandom.Value = new Random(value); }
		}

		static ThreadLocal<Random> thisRandom = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

		public static Random Random { get { return thisRandom.Value; } }

		public static double Value { get { return thisRandom.Value.NextDouble(); } }

		public static int Range(int min, int max)
		{
			return thisRandom.Value.Next(min, max);
		}

		public static float Range(float min, float max)
		{
			if (min > max) throw new Exception("min cannot be larger than max!!!");
			return (float)thisRandom.Value.NextDouble() * (max - min) + min;
		}

		public static double Range(double min, double max)
		{
			if (min > max) throw new Exception("min cannot be larger than max!!!");
			return thisRandom.Value.NextDouble() * (max - min) + min;
		}

		public static float RangeEpsilon(float min, float max) => Range(min + float.Epsilon, max - float.Epsilon);
		public static double RangeEpsilon(double min, double max) => Range(min + double.Epsilon, max - double.Epsilon);
	}
}
