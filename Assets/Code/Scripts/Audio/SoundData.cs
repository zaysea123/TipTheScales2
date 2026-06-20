using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
	[Serializable]
	public class SoundData
	{
		public AudioClip clip;
		public AudioMixerGroup mixerGroup;
		public bool loop;
		public bool playOnAwake;
		public bool frequentSound;

		[Range(0f, 1f)]
		public float spatialBlend = 0;

		[Header("3D Sound Settings")]
		[Range(0f, 1f)]
		public float dopplerLevel = 0;
		[Range(0, 360)]
		public int spread = 0;
		public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
		public float minDistance = 2f;
		public float maxDistance = 10f;

		public void DrawGizmos(Vector3 position)
		{
			Gizmos.color = Color.lightBlue;
			Gizmos.DrawWireSphere(position, minDistance);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(position, maxDistance);
		}
	}

}

