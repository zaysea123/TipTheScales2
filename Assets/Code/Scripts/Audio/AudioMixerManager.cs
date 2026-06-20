using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private UIDocument UI;
	private Slider soundFXSlider;

	private void Start()
	{
		//soundFXSlider = UI.rootVisualElement.Q<Slider>("soundFXSlider");
		//soundFXSlider.RegisterValueChangedCallback(evt =>
		//{
		//	SetSoundFXVolume(evt.newValue);
		//});
	}

	public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSoundFXVolume(float volume)
    {
		mixer.SetFloat("soundFXVolume", Mathf.Log10(volume) * 20f);
	}

	public void SetMusicVolume(float volume)
	{
		mixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20f);
	}
}
