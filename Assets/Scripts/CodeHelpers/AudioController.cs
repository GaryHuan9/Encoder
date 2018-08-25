using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CodeHelpers
{
	public class AudioController : MonoBehaviour
	{
		void Awake()
		{
			if (GetComponents<Component>().Length > 2)
			{
				Debug.LogError("This Game Object should only have the AudioController attached!!!");
				return;
			}

			if (instance != null)
			{
				//Debug.LogError("You cannot have two AudioController!\nCodeHelpers will fix this for you this time!");

				Destroy(this);
				return;
			}

			instance = this;

			for (int i = 0; i < sounds.Length; i++)
			{
				Sound thisSound = sounds[i];
				AudioSource source = gameObject.AddComponent<AudioSource>();

				thisSound.source = source;

				//Clip will be set when play
				source.volume = thisSound.volume;
				source.loop = thisSound.loop;

				source.playOnAwake = false;

				//Dictionary
				soundFromName.Add(thisSound.name, thisSound);
			}
		}

		static AudioController instance;

		[SerializeField] Sound[] sounds;

		Dictionary<string, Sound> soundFromName = new Dictionary<string, Sound>();

		public static Func<Vector3> GetPlayerPosition;

		void Update()
		{
			if (GetPlayerPosition != null) transform.position = GetPlayerPosition();
		}

		public static void Play(string name)
		{
			if (instance == null) throw new Exception("You haven't setup a AudioController in your scene yet!");

			if (!instance.soundFromName.ContainsKey(name)) throw new Exception("No sound named " + name + "!\nYou might want to check that!");

			Sound thisSound = instance.soundFromName[name];
			AudioSource thisSource = thisSound.source;

			if (thisSource.isPlaying) thisSource.Stop();

			thisSource.pitch = thisSound.pitch.RandomValue;
			thisSource.clip = CodeHelper.GetRandomObjectFromCollection(thisSound.clips);

			thisSource.Play();
		}

		[Serializable]
		class Sound
		{
			public string name = "";
			public AudioClip[] clips = null; //For random clips (Remove warning null)

			[Range(0f, 1f)] public float volume = 0.5f;
			public MinAndMax pitch = new MinAndMax(1f); //Pitch will random

			public bool loop = false; //Remove warning false

			internal AudioSource source;
		}
	}

}