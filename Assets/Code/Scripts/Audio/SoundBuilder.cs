using System;
using UnityEngine;

namespace AudioSystem
{
	public class SoundBuilder
	{
		private readonly SoundManager soundManager;
		private SoundData soundData;
		private Vector3 position = Vector3.zero;
		private bool randomPitch = false;

		public SoundBuilder(SoundManager soundManager)
		{
			this.soundManager = soundManager;
		}

		public SoundBuilder WithSoundData(SoundData soundData)
		{
			this.soundData = soundData;
			return this;
		}

		public SoundBuilder WithSoundData(string soundName)
		{
			this.soundData = SoundManager.instance.soundLibrary.GetDataFromName(soundName);
			if (soundData == null)
				throw new NullReferenceException("No sound with name: " +  soundName);

			return this;
		}

		public SoundBuilder WithPosition(Vector3 position)
		{
			this.position = position;
			return this;
		}

		public SoundBuilder WithrandomPitch()
		{
			this.randomPitch = true;
			return this;
		}

		public void Play()
		{
			if (!soundManager.CanPlaySound(soundData)) return;

			SoundEmitter soundEmitter = soundManager.Get();
			soundEmitter.Initialize(soundData);
			soundEmitter.transform.position = position;
			soundEmitter.transform.parent = soundManager.transform;

			if (randomPitch)
			{
				soundEmitter.WithRandomPitch();
			}

			if (soundData.frequentSound)
			{
				soundManager.FrequentSoundEmitters.Enqueue(soundEmitter);
			}
			soundEmitter.Play();
		}
	}
}

