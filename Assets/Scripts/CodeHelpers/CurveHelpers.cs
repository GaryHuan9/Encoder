using System;
using UnityEngine;

namespace CodeHelpers
{
	public static class CurveHelper
	{
		public static readonly AnimationCurve sigmoidCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		public static readonly AnimationCurve linearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	}
}
