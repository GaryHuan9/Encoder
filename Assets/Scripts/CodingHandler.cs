using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodingHandler : MonoBehaviour
{
	[SerializeField] Text destinationText;

	string encodingText;
	string decodingText;

	int frameDelay;

	void Update()
	{
		if (frameDelay-- <= 0) frameDelay = 30;
		else return;

		if (encodingText != null) destinationText.text = CodingController.Encode(encodingText);
		if (decodingText != null) try { destinationText.text = CodingController.Decode(decodingText); } catch { }

		encodingText = decodingText = null;
	}

	public void OnEncodeInputChanged(string input) => encodingText = input;
	public void OnDecodeInputChanged(string input) => decodingText = input;
}
