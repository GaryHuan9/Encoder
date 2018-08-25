using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Concurrent;

//REMEMBER that we need to make CodeHelpers.CodeHelperMonoBehaviour execute before other scripts by placing it in the 
//project settings, or some methods in CodeHelpers.InputHelpers and CodeHelpers.CodeHelperMonoBehaviour.PreUpdateMethods
//would not work.
using CodeHelpers.ThreadHelpers;

namespace CodeHelpers
{
	public static class CodeHelper
	{
		#region MinAndMax Helpers

		public static MinAndMax ToMinAndMax(this Vector2 thisVector) => new MinAndMax(thisVector);
		public static MinAndMax ToMinAndMax(this Vector3 thisVector) => new MinAndMax(thisVector);
		public static MinAndMax ToMinAndMax(this Vector4 thisVector) => new MinAndMax(thisVector);

		#endregion

		#region Number Helpers

		public static float ToSignedAngle(this float thisFloat) => Mathf.Repeat(thisFloat + 180f - epsilon, 360f) - 180f;
		public static int ToSignedAngle(this int thisInt) => Mathf.RoundToInt(((float)thisInt).ToSignedAngle());

		public static Vector2 ToVector2(this float thisFloat) => new Vector2(thisFloat, thisFloat);
		public static Vector2Int ToVector2(this int thisInt) => new Vector2Int(thisInt, thisInt);

		public static Vector2Int To2D(this int thisInt, int height) => new Vector2Int(thisInt / height, thisInt % height);

		public static Vector2 ToAngleInVector(this float thisFloat, float scale = 1f) => (Vector2.right * scale).Rotate(thisFloat);
		public static Vector2 ToAngleInVector(this int thisFloat) => Vector2.right.Rotate(thisFloat);

		public static int Normalized(this float thisFloat) => Mathf.Approximately(thisFloat, 0) ? 0 : thisFloat < 0 ? -1 : 1;
		public static int Normalized(this float thisFloat, float threshold) => Mathf.Abs(thisFloat) <= threshold ? 0 : thisFloat < 0 ? -1 : 1;
		public static int Normalized(this int thisInt) => thisInt == 0 ? 0 : thisInt < 0 ? -1 : 1;

		public static void Swap(ref float thisFloat, ref float otherFloat)
		{
			float storageFloat = thisFloat;
			thisFloat = otherFloat;
			otherFloat = storageFloat;
		}

		public static void Swap(ref int thisInt, ref int otherInt)
		{
			int storageInt = thisInt;
			thisInt = otherInt;
			otherInt = storageInt;
		}

		public const float epsilon = 0.00001f;

		#endregion

		#region Quaternion Helpers

		public static readonly Quaternion x270Rotation = Quaternion.Euler(-90, 0, 0);

		#endregion

		#region Random Helpers

		public static Color GetRandomColorBetweenColors(Color color1, Color color2) => Color.Lerp(color1, color2, (float)ThreadedRandom.Value);

		public static T GetRandomObjectFromCollection<T>(T[] thisArray) => thisArray[ThreadedRandom.Range(0, thisArray.Length)];

		public static T GetRandomObjectFromCollection<T>(List<T> thisList) => thisList[ThreadedRandom.Range(0, thisList.Count)];

		/// <summary>Fast random will only work if you are sure the dictonary is indexed like an array!</summary>
		public static T GetRandomObjectFromCollection<T>(Dictionary<int, T> thisDictionary, bool fastRandom = false)
		{
			int targetIndex = ThreadedRandom.Range(0, thisDictionary.Count);

			if (fastRandom) return thisDictionary[targetIndex];

			int currentIndex = 0;

			foreach (var thisPair in thisDictionary)
			{
				if (currentIndex == targetIndex) return thisPair.Value;
				currentIndex++;
			}

			throw new Exception("This should not happen!");
		}

		public static float Random1To1Value => (float)ThreadedRandom.Value * 2f - 1f;

		public static int GetRandomIndex(params float[] theseFloats)
		{
			float randomNumber = (float)ThreadedRandom.Value * theseFloats.Sum();

			for (int i = 0; i < theseFloats.Length; i++)
			{
				float thisFloat = theseFloats[i];
				if (randomNumber <= thisFloat) return i;
				randomNumber -= thisFloat;
			}

			return -1; //This should never happen
		}

		#endregion

		#region Camera Helpers

		static Camera mainCamera;
		public static Camera MainCamera { get { return mainCamera = mainCamera ?? Camera.main; } set { mainCamera = value; } }

		#endregion

		#region Special Method Helpers

		/// <summary>This will generate a method that can be only called once a frame. They will be called at end frame.(Thread safe)</summary>
		public static Action GetOneCallMethod(Action thisAction) => new OneCallMethod(thisAction).CallAction;

		class OneCallMethod
		{
			public OneCallMethod(Action thisAction)
			{
				invokeAction = () =>
				{
					thisAction();
					hasCalled = false;
				};
			}

			readonly Action invokeAction;

			volatile bool hasCalled;

			public void CallAction()
			{
				if (hasCalled) return;

				hasCalled = true;
				InvokeEndFrame(invokeAction);
			}
		}

		/// <summary>This will generate a method that will invoke a method randomly within a period of time.(Not thread safe)</summary>
		public static Action GetRandomInvokeMethod(Action thisAction, float maxDelay = 1f) => new RandomInvokeMethod(maxDelay, thisAction).CallAction;

		class RandomInvokeMethod
		{
			public RandomInvokeMethod(float delay, Action thisAction)
			{
				this.delay = delay;
				this.thisAction = thisAction;
			}

			readonly float delay;
			readonly Action thisAction;

			bool isCalling;

			public void CallAction()
			{
				if (isCalling) return;

				isCalling = true;
				StartCoroutine(RandomInvoke());
			}

			IEnumerator RandomInvoke()
			{
				float thisDelay = (float)ThreadedRandom.Value * delay;

				yield return new WaitForSeconds(thisDelay);

				thisAction();
				isCalling = false;
			}
		}

		#endregion

		#region IEnumerable Helpers

		public static bool IsIndexValid<T>(this T[] thisArray, int index)
		{
			return thisArray.GetLength(0) > index && index >= 0;
		}

		public static bool IsIndexValid<T>(this T[,] thisArray, Vector2Int index)
		{
			return thisArray.GetLength(0) > index.x && index.x >= 0 &&
				   thisArray.GetLength(1) > index.y && index.y >= 0;
		}

		public static bool IsIndexValid<T>(this T[,,] thisArray, Vector3Int index)
		{
			return thisArray.GetLength(0) > index.x && index.x >= 0 &&
				   thisArray.GetLength(1) > index.y && index.y >= 0 &&
				   thisArray.GetLength(2) > index.z && index.z >= 0;
		}

		public static Vector2Int? IndexOf<T>(this T[,] thisArray, T thisT)
		{
			for (int x = 0; x < thisArray.GetLength(0); x++)
			{
				for (int y = 0; y < thisArray.GetLength(1); y++)
				{
					if (thisArray[x, y].Equals(thisT)) return new Vector2Int(x, y);
				}
			}

			return null;
		}

		public static Vector3Int? IndexOf<T>(this T[,,] thisArray, T thisT)
		{
			for (int x = 0; x < thisArray.GetLength(0); x++)
			{
				for (int y = 0; y < thisArray.GetLength(1); y++)
				{
					for (int z = 0; z < thisArray.GetLength(2); z++)
					{
						if (thisArray[x, y, z].Equals(thisT)) return new Vector3Int(x, y, z);
					}
				}
			}

			return null;
		}

		public static Vector2Int Size<T>(this T[,] array) => new Vector2Int(array.GetLength(0), array.GetLength(1));
		public static Vector3Int Size<T>(this T[,,] array) => new Vector3Int(array.GetLength(0), array.GetLength(1), array.GetLength(2));

		public static float[,] CombineFloatArrays(float[,] array1, float[,] array2, float chance1, float chance2)
		{
			float[,] finalArray = new float[array1.GetLength(0), array1.GetLength(1)];

			float totalChance = chance1 + chance2;
			chance1 = chance1 / totalChance;
			chance2 = chance2 / totalChance;

			for (int x = 0; x < array1.GetLength(0); x++)
			{
				for (int y = 0; y < array1.GetLength(1); y++)
				{
					finalArray[x, y] = array1[x, y] * chance1 + array2[x, y] * chance2;
				}
			}

			return finalArray;
		}

		public static T[] Get1D<T>(this T[,] array, int index)
		{
			T[] result = new T[array.GetLength(1)];
			for (int i = 0; i < array.GetLength(1); i++) result[i] = array[index, i];
			return result;
		}

		public static IEnumerable<T> GetAllNonNull<T>(this T[] ienumerable) where T : class => from thisT in ienumerable
																							   where thisT != null
																							   select thisT;

		public static int Count<T>(this IEnumerable<T> thisIEnumerable, Func<T, int> countSelector)
		{
			int count = 0;
			thisIEnumerable.ForEach(thisT => count += countSelector(thisT));
			return count;
		}

		#region ForEach

		public static void ForEach<T>(this IEnumerable<T> thisIEnumerable, Action<T> thisAction)
		{
			foreach (T thisT in thisIEnumerable) thisAction(thisT);
		}

		public static void ForEach<T>(this IEnumerable<T> thisIEnumerable, Action<T, int> thisAction)
		{
			int index = 0;
			foreach (T thisT in thisIEnumerable)
			{
				thisAction(thisT, index);
				index++;
			}
		}

		public static void ForEach<T>(this T[] thisArray, Action<T> thisAction)
		{
			if (thisArray == null) return;

			int length = thisArray.Length;

			for (int i = 0; i < length; i++)
			{
				thisAction(thisArray[i]);
			}
		}

		public static void ForEach<T>(this T[] thisArray, Action<T, int> thisAction)
		{
			if (thisArray == null) return;

			int length = thisArray.Length;

			for (int i = 0; i < length; i++)
			{
				thisAction(thisArray[i], i);
			}
		}

		public static void ForEach<T>(this T[,] thisArray, Action<T> thisAction)
		{
			if (thisArray == null) return;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					thisAction(thisArray[i, j]);
				}
			}
		}

		public static void ForEach<T>(this T[,] thisArray, Action<T, int> thisAction)
		{
			if (thisArray == null) return;

			int index = 0;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					thisAction(thisArray[i, j], index);
					index++;
				}
			}
		}

		public static void ForEach<T>(this T[,] thisArray, Action<T, Vector2Int> thisAction)
		{
			if (thisArray == null) return;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					thisAction(thisArray[i, j], new Vector2Int(i, j));
				}
			}
		}

		public static void ForEach<T>(this T[,] thisArray, Action<T, int, int> thisAction)
		{
			if (thisArray == null) return;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					thisAction(thisArray[i, j], i, j);
				}
			}
		}

		public static void ForEach<T>(this T[,,] thisArray, Action<T> thisAction)
		{
			if (thisArray == null) return;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);
			int length2 = thisArray.GetLength(2);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					for (int k = 0; k < length2; k++)
					{
						thisAction(thisArray[i, j, k]);
					}
				}
			}
		}

		public static void ForEach<T>(this T[,,] thisArray, Action<T, int> thisAction)
		{
			if (thisArray == null) return;

			int index = 0;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);
			int length2 = thisArray.GetLength(2);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					for (int k = 0; k < length2; k++)
					{
						thisAction(thisArray[i, j, k], index);
						index++;
					}
				}
			}
		}

		public static void ForEach<T>(this T[,,] thisArray, Action<T, int, int, int> thisAction)
		{
			if (thisArray == null) return;

			int length0 = thisArray.GetLength(0);
			int length1 = thisArray.GetLength(1);
			int length2 = thisArray.GetLength(2);

			for (int i = 0; i < length0; i++)
			{
				for (int j = 0; j < length1; j++)
				{
					for (int k = 0; k < length2; k++)
					{
						thisAction(thisArray[i, j, k], i, j, k);
					}
				}
			}
		}

		public static void ForEach<T>(this IList<T> thisList, Action<T> thisAction)
		{
			if (thisList == null) return;

			int count = thisList.Count;

			for (int i = 0; i < count; i++)
			{
				thisAction(thisList[i]);
			}
		}

		public static void ForEach<T>(this IList<T> thisList, Action<T, int> thisAction)
		{
			if (thisList == null) return;

			int count = thisList.Count;

			for (int i = 0; i < count; i++)
			{
				thisAction(thisList[i], i);
			}
		}

		public static void ForEach<T>(this Queue<T> thisQueue, Action<T> thisAction)
		{
			while (thisQueue.Count > 0) thisAction(thisQueue.Dequeue());
		}

		public static void ForEach<T>(this Queue<T> thisQueue, Action<T, int> thisAction)
		{
			int index = 0;
			while (thisQueue.Count > 0)
			{
				thisAction(thisQueue.Dequeue(), index);
				index++;
			}
		}

		public static void ForEach<T>(this ConcurrentQueue<T> thisQueue, Action<T> thisAction)
		{
			while (thisQueue.Count > 0)
			{
				T thisT;
				if (thisQueue.TryDequeue(out thisT)) thisAction(thisT);
			}
		}

		public static void ForEach<T>(this ConcurrentQueue<T> thisQueue, Action<T, int> thisAction)
		{
			int index = 0;
			while (thisQueue.Count > 0)
			{
				T thisT;
				if (thisQueue.TryDequeue(out thisT)) thisAction(thisT, index);
				index++;
			}
		}

		public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> thisDictionary, Action<TKey> thisAction)
		{
			thisDictionary.Keys.ForEach(thisKey => thisAction(thisKey));
		}

		public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> thisDictionary, Action<TKey, int> thisAction)
		{
			thisDictionary.Keys.ForEach((thisKey, index) => thisAction(thisKey, index));
		}

		public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> thisDictionary, Action<TValue> thisAction)
		{
			thisDictionary.Values.ForEach(thisValue => thisAction(thisValue));
		}

		public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> thisDictionary, Action<TValue, int> thisAction)
		{
			thisDictionary.Values.ForEach((thisValue, index) => thisAction(thisValue, index));
		}

		//For each specials

		public static void ForEachS<T>(this IDictionary<int, T> thisDictionary, Action<T> thisAction) //These only work with dictionary indexed like an array
		{
			int count = thisDictionary.Count;

			for (int i = 0; i < count; i++)
			{
				thisAction(thisDictionary[i]);
			}
		}

		public static void ForEachS<T>(this IDictionary<int, T> thisDictionary, Action<T, int> thisAction) //These only work with dictionary indexed like an array
		{
			int count = thisDictionary.Count;

			for (int i = 0; i < count; i++)
			{
				thisAction(thisDictionary[i], i);
			}
		}

		#endregion

		public static bool AllSameValue<T, TValue>(this IEnumerable<T> thisIEnumerable, Func<T, TValue> valueGetter)
		{
			if (!thisIEnumerable.Any()) throw new NullReferenceException("thisIEnumerable cannot be null!");

			TValue selectedValue = valueGetter(thisIEnumerable.First());

			return thisIEnumerable.All((thisT) => valueGetter(thisT).Equals(selectedValue));
		}

		public static T MinT<T>(this IEnumerable<T> thisIEnumerable, Func<T, float> selector)
		{
			T minT = thisIEnumerable.FirstOrDefault();
			float minValue = float.MaxValue;

			thisIEnumerable.ForEach(thisT =>
			{
				float thisMin = selector(thisT);
				if (thisMin < minValue)
				{
					minValue = thisMin;
					minT = thisT;
				}
			});
			return minT;
		}

		public static T MaxT<T>(this IEnumerable<T> thisIEnumerable, Func<T, float> selector)
		{
			T maxT = thisIEnumerable.FirstOrDefault();
			float maxValue = float.MinValue;

			thisIEnumerable.ForEach(thisT =>
			{
				float thisMax = selector(thisT);
				if (thisMax > maxValue)
				{
					maxValue = thisMax;
					maxT = thisT;
				}
			});
			return maxT;
		}

		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> thisIDictionary, TKey key)
		{
			return thisIDictionary != null && thisIDictionary.ContainsKey(key) ? thisIDictionary[key] : default(TValue);
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> thisIDictionary, TKey key, TValue addValue)
		{
			if (!thisIDictionary.ContainsKey(key)) return thisIDictionary[key] = addValue;
			return thisIDictionary[key];
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> thisIDictionary, TKey key, Func<TValue> addFunc)
		{
			if (!thisIDictionary.ContainsKey(key)) return thisIDictionary[key] = addFunc();
			return thisIDictionary[key];
		}

		public static void Swap<T>(this T[] thisVector, int index1, int index2)
		{
			T storage = thisVector[index1];
			thisVector[index1] = thisVector[index2];
			thisVector[index2] = storage;
		}

		public static void Swap<T>(this IList<T> thisList, int index1, int index2)
		{
			T storage = thisList[index1];
			thisList[index1] = thisList[index2];
			thisList[index2] = storage;
		}

		#endregion

		#region Color Helpers

		public static Vector3Int ToVector8bit(this Color thisColor) => new Vector3(thisColor.r * 255, thisColor.g * 255, thisColor.b * 255).RoundToInt();
		public static Vector3Int ToVector16bit(this Color thisColor) => new Vector3(thisColor.r * 65535, thisColor.g * 65535, thisColor.b * 65535).RoundToInt();

		#endregion

		#region Call Events

		internal static InvokeMethod invokeBeforeFrame = new InvokeMethod();
		internal static InvokeMethod invokeNextFrame = new InvokeMethod();
		internal static InvokeMethod invokeEndFrame = new InvokeMethod();

		//These methods does not ensure which frame the action execute, but it will execute them in the correct frame phase

		public static void InvokeBeforeFrame(Action thisAction) => invokeBeforeFrame.Add(thisAction);
		public static void InvokeNextFrame(Action thisAction) => invokeNextFrame.Add(thisAction);
		public static void InvokeEndFrame(Action thisAction) => invokeEndFrame.Add(thisAction);

		public static void InvokeAfter(Action thisAction, float delay)
		{
			if (thisAction == null) throw new NullReferenceException("thisAction cannot be null!!!");
			StartCoroutine(CallMethodAfterCoroutine(thisAction, delay));
		}

		static IEnumerator CallMethodAfterCoroutine(Action thisAction, float delay)
		{
			yield return new WaitForSeconds(delay);
			thisAction();
		}

		internal class InvokeMethod
		{
			readonly Queue<Action> actions = new Queue<Action>();
			readonly ConcurrentQueue<Action> threadedActions = new ConcurrentQueue<Action>();

			public void Add(Action thisAction)
			{
				if (thisAction == null) throw new NullReferenceException("thisAction cannot be null!!!");
				if (ThreadHelper.IsInMainThread) actions.Enqueue(thisAction);
				else threadedActions.Enqueue(thisAction);
			}

			public void InvokeAll()
			{
				actions.ForEach(thisAction => thisAction());
				threadedActions.ForEach(thisAction => thisAction());
			}
		}

		#endregion

		public static Coroutine StartCoroutine(IEnumerator thisCoroutine)
		{
			if (CodeHelperMonoBehaviour.instance == null) throw new Exception("You did not add CodeHelper in the scene yet!!!");
			return CodeHelperMonoBehaviour.instance.StartCoroutine(thisCoroutine);
		}

		public static T SameThenDefault<T>(this T thisT, T otherT)
		{
			return thisT.Equals(otherT) ? default(T) : thisT;
		}

		/// <summary>If <paramref name="thisFrom"/> is null, then we returns <paramref name="defaultResult"/>. Otherwise we select the result from <paramref name="selector"/>.</summary>
		public static TResult RINN<TFrom, TResult>(this TFrom thisFrom, TResult result, TResult defaultResult = default(TResult)) where TFrom : class where TResult : struct
		{
			return thisFrom == null ? defaultResult : result;
		}

		/// <summary>If <paramref name="thisFrom"/> is null, then we returns <paramref name="defaultResult"/>. Otherwise we select the result from <paramref name="selector"/>.</summary>
		public static TResult RINN<TFrom, TResult>(this TFrom thisFrom, Func<TFrom, TResult> selector, TResult defaultResult = default(TResult)) where TFrom : class where TResult : struct
		{
			return thisFrom == null ? defaultResult : selector(thisFrom);
		}

		public static readonly object placeholder = new object();
	}
}