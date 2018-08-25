using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System;
using CodeHelpers;

public static class CodingController
{
	static CodingController()
	{
		CurrentMethod = ShiftBits;
	}

	public static IMethod CurrentMethod;

	public static string Encode(string input) => CurrentMethod.EncodeMethod(input);
	public static string Decode(string input) => CurrentMethod.DecodeMethod(input);

	public static readonly IMethod ShiftBits = new ShiftBitsMethod();
	public static readonly IMethod Base64 = new Base64Method();

	public static readonly IMethod ShiftBits2 = new ShiftBitsMethod(2);
	public static readonly IMethod ShiftBits3 = new ShiftBitsMethod(3);

	static bool GetBit(byte thisByte, int index) => (thisByte & 1 << index) != 0;

	class ShiftBitsMethod : IMethod
	{
		public ShiftBitsMethod(int layer = 1)
		{
			this.layer = layer;
		}

		int layer;

		public string EncodeMethod(string input)
		{
			for (int i = 0; i < layer; i++) input = EncodeMethodInternal(input);
			return input;
		}

		public string DecodeMethod(string input)
		{
			for (int i = 0; i < layer; i++) input = DecodeMethodInternal(input);
			return input;
		}

		public string EncodeMethodInternal(string input)
		{
			input = Base64.EncodeMethod(input);

			bool[,] bools = new bool[input.Length, 8];
			int length = input.Length;
			string result = "";

			input.ForEach((thisChar, index) =>
			{
				byte thisByte = (byte)thisChar;
				for (int i = 0; i < 8; i++) bools[(index + i) % length, i] = GetBit(thisByte, i);
			});

			for (int i = 0; i < length; i++)
			{
				byte thisByte = 0;
				for (int j = 0; j < 8; j++) if (bools[i, j]) thisByte += (byte)(1 << j);
				result += thisByte.ToString("000");
			}

			return result;
		}

		public string DecodeMethodInternal(string input)
		{
			if (input.Length % 3 != 0 || input.Length == 0) return "";

			int length = input.Length / 3;
			bool[,] bools = new bool[length, 8];
			string result = "";

			Split(input, 3).ForEach((thisStringByte, index) =>
			{
				byte thisByte = byte.Parse(thisStringByte);
				for (int i = 0; i < 8; i++) bools[index, i] = GetBit(thisByte, i);
			});

			for (int i = 0; i < length; i++)
			{
				byte thisByte = 0;
				for (int j = 0; j < 8; j++) { if (bools[(i + j) % length, j]) thisByte += (byte)(1 << j); }
				result += (char)thisByte;
			}

			return Base64.DecodeMethod(result);
		}

		string[] Split(string input, int size)
		{
			int length = input.Length / size;
			var result = new string[length];

			for (int i = 0; i < length; i++) result[i] = input.Substring(i * size, size);
			return result;
		}
	}

	class Base64Method : IMethod
	{
		public string EncodeMethod(string input) => Convert.ToBase64String(Encoding.ASCII.GetBytes(input));
		public string DecodeMethod(string input) => Encoding.ASCII.GetString(Convert.FromBase64String(input));
	}
}

public interface IMethod
{
	string EncodeMethod(string input);
	string DecodeMethod(string input);
}
