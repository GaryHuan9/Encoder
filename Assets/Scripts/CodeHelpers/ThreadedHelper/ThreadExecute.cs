using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System;

namespace CodeHelpers.ThreadHelpers
{
	public class ThreadExecute
	{
		/// <summary>Create a new instance</summary>
		/// <param name="checkDelay">If this value is below 0, then we check it right after the executions. Otherwise we delay it by checkDelay seconds.</param>
		public ThreadExecute(float checkDelay = -1f)
		{
			ExecutionThread = ThreadHelper.NewThread(ExecuteQueueingExecutions);
			delay = checkDelay;

			CodeHelperMonoBehaviour.OnApplicationQuitMethods += ExecutionThread.Abort;

			executeExecutionAction = (thisExecution) =>
			{
				if (thisExecution.useId && killingId == thisExecution.id) return;
				while (killingId != defaultValue) { } //Wait until they equal

				ExecutingId = thisExecution.id;
				thisExecution.thisAction();
				ExecutingId = 0;
			};
		}

		public Thread ExecutionThread { get; private set; }

		volatile int executingId;
		public int ExecutingId { get { return executingId; } private set { executingId = value; } }

		readonly ConcurrentQueue<Execution> executionQueue = new ConcurrentQueue<Execution>();

		float delay;

		long killingId = long.MaxValue; //If this long is larger than int.MaxValue then we count it as null
		const long defaultValue = long.MaxValue; //This is the value when we are not killing any execution

		Action<Execution> executeExecutionAction;

		void ExecuteQueueingExecutions()
		{
			while (true)
			{
				executionQueue.ForEach(executeExecutionAction);
				if (delay > 0) Thread.Sleep((int)Math.Round(delay * 1000f));
			}
		}

		public void AddExecution(Action thisExecution)
		{
			if (!ThreadHelper.IsInMainThread) throw new Exception("You only call this in the main thread.");

			executionQueue.Enqueue(new Execution(thisExecution));
			if (ExecutionThread.ThreadState == ThreadState.Unstarted) ExecutionThread.Start();
		}

		public void AddExecution(Action thisExecution, int id)
		{
			if (!ThreadHelper.IsInMainThread) throw new Exception("You only call this in the main thread.");

			executionQueue.Enqueue(new Execution(thisExecution, id));
			if (ExecutionThread.ThreadState == ThreadState.Unstarted) ExecutionThread.Start();
		}

		/// <summary>This method kills the current execution if it has the same id, and deletes all executions with this id in the queue</summary>
		public bool KillAllExecutions(int id)
		{
			if (!ThreadHelper.IsInMainThread) throw new Exception("You only call this in the main thread because it contains enumeration of the queue.");

			bool successful = false;
			Interlocked.Exchange(ref killingId, id);

			if (ExecutingId == id)
			{
				ExecutionThread.Abort();
				ExecutionThread = ThreadHelper.NewThread(ExecuteQueueingExecutions);

				successful = true;
			}

			executionQueue.ForEach(thisExecution => { if (thisExecution.id != id) executionQueue.Enqueue(thisExecution); else successful = true; });
			Interlocked.Exchange(ref killingId, defaultValue);

			return successful;
		}

		struct Execution
		{
			public Execution(Action thisAction)
			{
				this.thisAction = thisAction;
				id = 0;
				useId = false;
			}

			public Execution(Action thisAction, int id)
			{
				this.thisAction = thisAction;
				this.id = id;
				useId = true;
			}

			public Action thisAction;

			public bool useId;
			public int id;
		}
	}
}