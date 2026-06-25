using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioMixerManager : MonoBehaviour
{
	public static AudioMixerManager instance;

    [SerializeField] private AudioMixer mixer;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
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
