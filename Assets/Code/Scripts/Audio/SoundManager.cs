using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AudioSystem
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager instance;

		//[SerializeField] private AudioSource soundFXObject;
		private IObjectPool<SoundEmitter> soundEmitterPool;
		private readonly List<SoundEmitter> activeSoundEmitters = new();
		public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

		public SoundLibrary soundLibrary;

		[SerializeField] private SoundEmitter soundEmitterPrefab;
		[SerializeField] private bool collectionCheck = true;
		[SerializeField] private int defaultCapacity = 10;
		[SerializeField] private int maxPoolSize = 100;
		[SerializeField] private int maxSoundInstances = 30;


		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		private void Start()
		{
			InitializePool();
		}

		public SoundBuilder CreateSound() => new SoundBuilder(this);

		public bool CanPlaySound(SoundData data)
		{
			if (!data.frequentSound) return true;

			if(FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
			{
				try
				{
					soundEmitter.Stop();
					return true;
				}
				catch
				{
					Debug.Log("Sound Emitter is alreadyt released");
				}
				return false;
			}
			return true;
		}

		public SoundEmitter Get()
		{
			return soundEmitterPool.Get();
		}

		public void ReturnToPool(SoundEmitter soundEmitter)
		{
			soundEmitterPool.Release(soundEmitter);
		}

		private void OnDestroyPoolObject(SoundEmitter soundEmitter)
		{
			Destroy(soundEmitter.gameObject);
		}

		private void OnReturnedToPool(SoundEmitter soundEmitter)
		{
			soundEmitter.gameObject.SetActive(false);
			activeSoundEmitters.Remove(soundEmitter);
		}

		private void OnTakeFromPool(SoundEmitter soundEmitter)
		{
			soundEmitter.gameObject.SetActive(true);
			activeSoundEmitters.Add(soundEmitter);
		}

		private SoundEmitter CreateSoundEmitter()
		{
			SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);
			soundEmitter.gameObject.SetActive(false);
			return soundEmitter;
		}

		private void InitializePool()
		{
			soundEmitterPool = new ObjectPool<SoundEmitter>(
				CreateSoundEmitter,
				OnTakeFromPool,
				OnReturnedToPool,
				OnDestroyPoolObject,
				collectionCheck,
				defaultCapacity,
				maxPoolSize);
		}

		//public void PLaySoundFXClip(AudioClip audioClip, Transform position, float volume)
		//{
		//	AudioSource source = Instantiate(soundFXObject, position.position, Quaternion.identity);
		//	source.clip = audioClip;
		//	source.volume = volume;
		//	source.Play();
		//	float clipLength = source.clip.length;
		//	Destroy(source.gameObject, clipLength);
		//}

		//public void PLayRandomSoundFXClip(AudioClip[] audioClips, Transform position, float volume)
		//{
		//	AudioSource source = Instantiate(soundFXObject, position.position, Quaternion.identity);

		//	int rnd = Random.Range(0, audioClips.Length);

		//	source.clip = audioClips[rnd];
		//	source.volume = volume;

		//	source.Play();

		//	float clipLength = source.clip.length;
		//	Destroy(source.gameObject, clipLength);
		//}
	}
}

