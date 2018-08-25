using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeHelpers;

namespace CodeHelpers.InputHelpers
{
	public static class InputHelper
	{
		static InputHelper()
		{
			inputInfoFromKey = new Dictionary<int, InputInfo>();

			for (int i = -1; i >= -3; i--)
			{
				inputInfoFromKey.Add(i, new InputInfo(i));
			}

			for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
			{
				inputInfoFromKey.Add(i, new InputInfo(i));
			}

		}

		internal static Dictionary<int, InputInfo> inputInfoFromKey;

		public static void AddKeyToRecord(int key, bool isKeyboardKey) //By default we only record the keys for English charactors A to Z, but you can also add them your own
		{
			int realKey = isKeyboardKey ? key : -key - 1;
			inputInfoFromKey.Add(realKey, new InputInfo(realKey));
		}

		public static int? AnyNumber
		{
			get
			{
				int outInt;
				return int.TryParse(Input.inputString, out outInt) ? outInt : (int?)null;
			}
		}

		public static Vector2Int GetWASDMovement(KeyCode WKey = KeyCode.W,
												 KeyCode AKey = KeyCode.A,
												 KeyCode SKey = KeyCode.S,
												 KeyCode DKey = KeyCode.D)
		{
			return new Vector2Int((Input.GetKey(DKey)) ? 1 : 0 + (Input.GetKey(AKey) ? -1 : 0),
								  (Input.GetKey(WKey)) ? 1 : 0 + (Input.GetKey(SKey) ? -1 : 0));
		}

		public static bool GetKeyHoldLessThan(KeyCode thisKey, float time = 0.2f)
		{
			return Input.GetKeyUp(thisKey) && Time.unscaledTime - inputInfoFromKey[(int)thisKey].thisPressedTime < time;
		}

		public static bool GetKeyDoublePressed(KeyCode thisKey, float doublePressSpeed = 0.2f)
		{
			return Input.GetKeyDown(thisKey) && Time.unscaledTime - inputInfoFromKey[(int)thisKey].lastPressedTime < doublePressSpeed;
		}

		public static bool GetMouseButtonHoldLessThan(int thisButton, float time = 0.2f)
		{
			return Input.GetMouseButtonUp(thisButton) && Time.unscaledTime - inputInfoFromKey[-thisButton - 1].thisPressedTime < time;
		}

		public static bool GetMouseButtonDoublePressed(int thisButton, float doublePressSpeed = 0.2f)
		{
			return Input.GetMouseButtonDown(thisButton) && Time.unscaledTime - inputInfoFromKey[-thisButton - 1].lastPressedTime < doublePressSpeed;
		}
	}
}

namespace CodeHelpers
{
	class InputInfo
	{
		internal InputInfo(int key)
		{
			this.key = key;
			isKeyboardKey = key >= 0;
		}

		readonly int key; //If this is larger or equals to 0 then it is a key, or else it is a mouse button, -1=left -2=right -3=middle
		readonly bool isKeyboardKey;

		internal float lastPressedTime = float.MinValue;
		internal float thisPressedTime = float.MinValue;

		internal void UpdateInfo()
		{
			if (isKeyboardKey ? Input.GetKeyDown((KeyCode)key) : Input.GetMouseButtonDown(-key - 1))
			{
				lastPressedTime = thisPressedTime;
				thisPressedTime = Time.unscaledTime;
			}
		}
	}
}