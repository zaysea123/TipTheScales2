using System;
using System.Collections;
using UnityEngine;

namespace AudioSystem
{
	public class SoundEmitter : MonoBehaviour
	{
		public SoundData Data { get; private set; }

		private AudioSource audioSource;
		private Coroutine playingCoroutine;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void Play()
		{
			if (playingCoroutine != null)
				StopCoroutine(playingCoroutine);

			audioSource.Play();
			playingCoroutine = StartCoroutine(WaitForSoundToEnd());
		}

		IEnumerator WaitForSoundToEnd()
		{
			yield return new WaitWhile(()=> audioSource.isPlaying);
			SoundManager.instance.ReturnToPool(this);
		}

		public void Stop()
		{
			if (playingCoroutine != null)
			{
				StopCoroutine(playingCoroutine);
				playingCoroutine = null;
			}

			audioSource.Stop();
			SoundManager.instance.ReturnToPool(this);
		}

		public void Initialize(SoundData data)
		{
			Data = data;
			audioSource.clip = data.clip;
			audioSource.outputAudioMixerGroup = data.mixerGroup;
			audioSource.loop = data.loop;
			audioSource.playOnAwake = data.playOnAwake;
			audioSource.spatialBlend = data.spatialBlend;
			audioSource.dopplerLevel = data.dopplerLevel;
			audioSource.spread = data.spread;
			audioSource.rolloffMode = data.rolloffMode;
			audioSource.minDistance = data.minDistance;
			audioSource.maxDistance = data.maxDistance;
		}

		public void WithRandomPitch(float min = -.05f, float max = .05f)
		{
			audioSource.pitch += UnityEngine.Random.Range(min, max);
		}

		private void OnDrawGizmos()
		{
			Data.DrawGizmos(transform.position);
		}
	}
}

