using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private MusicLibrary library;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public void PlayTrack(string trackName, float fadeInDuration = 0f)
	{
		AudioClip clip = library.GetTrackFromName(trackName);
		if (clip == null)
			return;

		if (fadeInDuration <= 0f)
		{
			musicSource.clip = clip;
			musicSource.Play();
		}	
		else
			StartCoroutine(AnimateTrackFadeIn(clip, fadeInDuration));
		
	}
	public void PlayMusicCrossfade(string trackName, float fadeDuration = .5f)
	{
		StartCoroutine(AnimateMusicCrossfade(library.GetTrackFromName(trackName), fadeDuration));
	}

	private IEnumerator AnimateTrackFadeIn(AudioClip track, float fadeDuration)
	{
		float percent = 0;
		musicSource.clip = track;
		musicSource.volume = 0;
		musicSource.Play();

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			musicSource.volume = Mathf.Lerp(0f, 1f, percent);
			yield return null;
		}
	}

	private IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration)
	{
		float percent = 0;
		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			Debug.Log(percent);
			musicSource.volume = Mathf.Lerp(1f, 0f, percent);

			yield return null;
		}

		musicSource.clip = nextTrack;
		musicSource.Play();
		Debug.Log("Playing a track");
		percent = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			
			musicSource.volume = Mathf.Lerp(0f, 1f, percent);
			Debug.Log(musicSource.volume);
			yield return null;
		}
	}


}
