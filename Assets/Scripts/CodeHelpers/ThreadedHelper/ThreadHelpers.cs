using System.Collections.Generic;
using System;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

namespace CodeHelpers.ThreadHelpers
{
	public static class ThreadHelper
	{
		static ThreadHelper()
		{
			CodeHelperMonoBehaviour.UnityUpdateMethods += () => queuedActions.ForEach(thisAction => thisAction());
		}

		static ConcurrentQueue<Action> queuedActions = new ConcurrentQueue<Action>();

		static internal int mainThreadId;

		public static void InvokeInMainThread(Action thisAction)
		{
			queuedActions.Enqueue(thisAction);
		}

		/// <summary>This returns a new thread that will make sure to print out exceptions.</summary>
		public static Thread NewThread(Action thisAction, bool throwThreadAbortException = false, Action abortAction = null)
		{
			return new Thread(() =>
			{
				try
				{
					thisAction();
				}
				catch (ThreadAbortException thisException)
				{
					if (throwThreadAbortException) Debug.LogException(thisException);
					abortAction?.Invoke();
				}
				catch (Exception thisException)
				{
					Debug.LogException(thisException);
				}
			});
		}

		public static bool IsInMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId;

		/// <summary>This returns a method that can only be executing by one thread. If it is already being executed by thread A, then if thread B tries to execute it the current executing invoked by thread A will keep going but the one invoked by B will be returned.</summary>
		public static Action GetOneCallMethod(Action thisAction)
		{
			return new OneCallMethod(thisAction).Execute;
		}

		class OneCallMethod
		{
			public OneCallMethod(Action thisAction)
			{
				this.thisAction = thisAction;
			}

			readonly Action thisAction;
			volatile bool executing;

			object thisLock = new object();

			public void Execute()
			{
				lock (thisLock)
				{
					if (executing) return;
					executing = true;
				}

				thisAction();
				executing = false;
			}
		}
	}
}