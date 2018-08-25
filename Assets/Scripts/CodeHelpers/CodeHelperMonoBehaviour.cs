using UnityEngine;
using System.Collections;
using System;
using CodeHelpers.InputHelpers;

namespace CodeHelpers
{
	[AddComponentMenu("CodeHelper")]
	public class CodeHelperMonoBehaviour : MonoBehaviour
	{
		void Awake()
		{
			if (instance != null)
			{
				Destroy(this);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);

			ThreadHelpers.ThreadHelper.mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}

		public static event Action OnApplicationQuitMethods;
		public static event Action OnDrawGizmosMethods;

		public static event Action UnityUpdateMethods;
		public static event Action UnityPreUpdateMethods;
		public static event Action UnityEndUpdateMethods;

		//One-time events
		public static event Action OnUnityPreUpdateMethods;
		public static event Action OnUnityUpdateMethods;
		public static event Action OnUnityEndUpdateMethods;

		internal static CodeHelperMonoBehaviour instance;

		public static FramePhase FramePhase { get; private set; }

		void Start()
		{
			StartCoroutine(EndOfFrameUpdate());
		}

		#region Updates

		void PreUpdate()
		{
			InputHelper.inputInfoFromKey.ForEach(thisPair => thisPair.Value.UpdateInfo());

			FramePhase = FramePhase.pre;

			CodeHelper.invokeBeforeFrame.InvokeAll();

			//EVERTHING THATS NOT INTERNAL SHOULD BE AFTER THIS LINE

			UnityPreUpdateMethods?.Invoke();

			OnUnityPreUpdateMethods?.Invoke();
			OnUnityPreUpdateMethods = null;
		}

		void Update()
		{
			PreUpdate();

			FramePhase = FramePhase.middle;

			CodeHelper.invokeNextFrame.InvokeAll();
			UnityUpdateMethods?.Invoke();

			OnUnityUpdateMethods?.Invoke();
			OnUnityUpdateMethods = null;
		}

		void LateUpdate()
		{
			FramePhase = FramePhase.late;
		}

		void EndUpdate()
		{
			FramePhase = FramePhase.end;

			CodeHelper.invokeEndFrame.InvokeAll();
			UnityEndUpdateMethods?.Invoke();

			OnUnityEndUpdateMethods?.Invoke();
			OnUnityEndUpdateMethods = null;
		}

		#endregion

		void OnApplicationQuit()
		{
			OnApplicationQuitMethods?.Invoke();
		}

		IEnumerator EndOfFrameUpdate()
		{
			WaitForEndOfFrame endUpdate = new WaitForEndOfFrame();

			while (true)
			{
				yield return endUpdate;

				EndUpdate();
			}
		}

		void OnDrawGizmos()
		{
			OnDrawGizmosMethods?.Invoke();
		}

	}

	public enum FramePhase
	{
		pre,
		middle,
		late,
		end
	}
}