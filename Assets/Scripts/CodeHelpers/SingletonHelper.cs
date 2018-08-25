using System;
using UnityEngine;
using UE = UnityEngine;

namespace CodeHelpers
{
	public static class SingletonHelper
	{
		public static T QuietDestroyCurrent<T>(ref T instance, T current) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current);
			return instance;
		}

		public static T QuietDestroyGameObject<T>(ref T instance, T current) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current.gameObject);
			return instance;
		}

		public static T DebugErrorNoDestroy<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;

			Debug.LogError(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
			return instance;
		}

		public static T DebugErrorDestroyCurrent<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current);
			Debug.LogError(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
			return instance;
		}

		public static T DebugErrorDestroyGameObject<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current.gameObject);
			Debug.LogError(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
			return instance;
		}

		public static T ThrowExceptionNoDestroy<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;
			throw new Exception(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
		}

		public static T ThrowExceptionDestroyCurrent<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current);
			throw new Exception(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
		}

		public static T ThrowExceptionDestroyGameObject<T>(ref T instance, T current, string customString = null) where T : Component
		{
			if (instance == null) return instance = current;

			UE.Object.Destroy(current.gameObject);
			throw new Exception(customString ?? ("There is already a " + nameof(T) + " instance! But you created another one!"));
		}
	}
}
