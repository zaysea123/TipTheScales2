using AudioSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class WeaponSoundEvents : MonoBehaviour
{
	[Serializable]
	public class WeaponSound
	{
		public string name;
		public SoundData data;
		public bool enabled;
	}

	public WeaponSound[] sounds;

	public void PlaySound(string soundName)
	{
		WeaponSound sound = Array.Find<WeaponSound>(sounds, el => el.name == soundName);
		if (sound.enabled)
		{
			SoundManager.instance.CreateSound()
			.WithSoundData(sound.data)
			.Play();
		}
		
	}
}
