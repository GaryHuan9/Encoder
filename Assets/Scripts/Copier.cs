using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Copier : MonoBehaviour
{
	public Text copyText;

	public void OnPressed()
	{
		EditorGUIUtility.systemCopyBuffer = copyText.text;
	}
}
