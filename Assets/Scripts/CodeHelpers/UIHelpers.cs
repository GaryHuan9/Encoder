using System.Linq;
using UnityEngine;

namespace CodeHelpers
{
	public static class UIHelper
	{
		public static RectTransform RectTransform(this Component thisComponent) => (RectTransform)thisComponent.transform;

		static Canvas mainCanvas;
		const string mainCanvasTag = "MainCanvas";
		public static Canvas MainCanvas => mainCanvas = mainCanvas ?? GetMainCanvas();

		static Canvas GetMainCanvas()
		{
			var allMainCanvas = (from thisCanvas in Object.FindObjectsOfType<Canvas>()
								 where thisCanvas.isRootCanvas
								 select thisCanvas).ToArray();

			if (allMainCanvas.Length == 0) return null; //Maybe change something or throw a warning?
			if (allMainCanvas.Length == 1) return allMainCanvas[0];

			var allTaggedCanvas = (from thisCanvas in allMainCanvas
								   where thisCanvas.tag == mainCanvasTag
								   select thisCanvas).ToArray();

			if (allTaggedCanvas.Length == 1) return allTaggedCanvas[0];
			if (allTaggedCanvas.Length == 0)
			{
				Debug.LogWarning("There are multiple root canvas. UIHelper cannot determine which one is the main canvas. Please tag the main canvas with \"" + mainCanvasTag + "\".");
				return allMainCanvas[0];
			}

			Debug.LogWarning("There are multiple canvas tagged \"" + mainCanvasTag + "\". UIHelper cannot determine which one is the main canvas. Please only tag one canvas with \"" + mainCanvasTag + "\".");
			return allTaggedCanvas[0];
		}

		public static Vector2 MainCanvasWorldSize => MainCanvas.WorldSize();
		public static Vector2 WorldSize(this Canvas canvas) => Vector2.Scale(canvas.RectTransform().sizeDelta, canvas.RectTransform().localScale);
	}
}
