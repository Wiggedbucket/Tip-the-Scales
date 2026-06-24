using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

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

	public void SetVolume(float volume)
	{
		volume = Mathf.Clamp01(volume);
		musicSource.volume = volume;
	}

	public void MultiplyVolumeBy(float multiplier)
	{
		multiplier = Mathf.Clamp01(multiplier);
		musicSource.volume *= multiplier;
	}


	public void DampenTrack(float cutoff, float fadeDuration)
	{
		StartCoroutine(AnimateDampenTrack(musicSource, cutoff, fadeDuration));
	}

	public void UnDampedTrack(float fadeDuration)
	{
		StartCoroutine(AnimateUnDampenTrack(musicSource, fadeDuration));
	}

	public void DampenTrackOf(AudioSource source, float cutoff, float fadeDuration)
	{
		StartCoroutine(AnimateDampenTrack(source, cutoff, fadeDuration));
	}

	public void UnDampenTrackOf(AudioSource source, float fadeDuration)
	{
		StartCoroutine(AnimateUnDampenTrack(source, fadeDuration));
	}

	private IEnumerator AnimateDampenTrack(AudioSource source, float cutoff, float fadeDuration)
	{
		AudioLowPassFilter filter = source.GetComponent<AudioLowPassFilter>();
		float statedCutoff = filter.cutoffFrequency;
		float percent = 0;

		while(percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			filter.cutoffFrequency = Mathf.Lerp(statedCutoff, cutoff, percent);
			yield return null;
		}
	}
	private IEnumerator AnimateUnDampenTrack(AudioSource source, float fadeDuration)
	{
		AudioLowPassFilter filter = source.GetComponent<AudioLowPassFilter>();
		float statedCutoff = filter.cutoffFrequency;
		float percent = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			filter.cutoffFrequency = Mathf.Lerp(statedCutoff, 22000, percent);
			yield return null;
		}
	}

	public void PlayTrack(string trackName, double startTime = 0, float fadeInDuration = 0f)
	{
		AudioClip clip = library.GetTrackFromName(trackName);
		if (clip == null)
			return;

		if (fadeInDuration <= 0f)
		{
			musicSource.clip = clip;
			musicSource.PlayScheduled(startTime);
		}	
		else
			StartCoroutine(AnimateTrackFadeIn(clip, fadeInDuration));
		
	}
	public void PlayTrackOf(AudioSource source, double startTime = 0, float fadeInDuration = 0f)
	{

		if (fadeInDuration <= 0f)
		{
			source.PlayScheduled(startTime);
		}
		else
			StartCoroutine(AnimateTrackFadeInOf(source, fadeInDuration));

	}

	public void PauseTrack(float fadeOutDuration = 0f)
	{
		if (!musicSource.isPlaying)
			return;

		if (fadeOutDuration <= 0f)
		{
			musicSource.Pause();
		}
		else
		{
			StartCoroutine(AnimateTrackFadeOut(fadeOutDuration, () => musicSource.Pause()));
		}
	}

	public void PauseTrackOf(AudioSource source, float fadeOutDuration = 0f)
	{
		if (!source.isPlaying)
			return;

		if (fadeOutDuration <= 0f)
		{
			source.Pause();
		}
		else
		{
			StartCoroutine(AnimateTrackFadeOutOf(source, fadeOutDuration, () => source.Pause()));
		}
	}

	public void UnpauseTrack(float fadeInDuration = 0f)
	{
		if (musicSource.isPlaying)
			return;

		if (fadeInDuration <= 0f)
		{
			musicSource.UnPause();
		}
		else
		{
			musicSource.UnPause();
			StartCoroutine(AnimateTrackFadeInFromPause(fadeInDuration));
		}
	}

	public void UnpauseTrackOf(AudioSource source, float fadeInDuration = 0f)
	{
		if (source.isPlaying)
			return;

		if (fadeInDuration <= 0f)
		{
			source.UnPause();
		}
		else
		{
			source.UnPause();
			StartCoroutine(AnimateTrackFadeInFromPauseOf(source, fadeInDuration));
		}
	}

	public void StopTrack(float fadeInDuration = 0f)
	{
		if (!musicSource.isPlaying)
			return;

		if (fadeInDuration <= 0f)
		{
			musicSource.Stop();
		}
		else
		{
			StartCoroutine(AnimateTrackFadeOut(fadeInDuration, () => musicSource.Stop()));
		}
	}

	public void StopTrackOf(AudioSource source, float fadeInDuration = 0f)
	{
		if (!source.isPlaying)
			return;

		if (fadeInDuration <= 0f)
		{
			source.Stop();
		}
		else
		{
			StartCoroutine(AnimateTrackFadeOutOf(source, fadeInDuration, () => musicSource.Stop()));
		}
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
			musicSource.volume = Mathf.Lerp(musicSource.volume, 1f, percent);
			yield return null;
		}
	}

	private IEnumerator AnimateTrackFadeInFromPause(float fadeDuration)
	{
		float percent = 0;

		float statedVolume = musicSource.volume;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			musicSource.volume = Mathf.Lerp(statedVolume, 1f, percent);
			yield return null;
		}
	}

	public IEnumerator AnimateTrackFadeInFromPauseOf(AudioSource source, float fadeDuration)
	{
		float percent = 0;

		if (source.volume >= 1f)
			yield break;

		float statedVolume = source.volume;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			source.volume = Mathf.Lerp(statedVolume, 1f, percent);
			yield return null;
		}
	}

	private IEnumerator AnimateTrackFadeOut(float fadeDuration, Action afterFunc = null)
	{
		float percent = 0;

		float statedVolume = musicSource.volume;
		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			musicSource.volume = Mathf.Lerp(statedVolume, 0f, percent);
			yield return null;
		}
		if (afterFunc != null) 
			afterFunc();
	}

	public IEnumerator AnimateTrackFadeOutOf(AudioSource source, float fadeDuration, Action afterFunc = null)
	{
		if (source.volume <= 0f)
			yield break;

		float percent = 0;
		float statedVolume = source.volume;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			source.volume = Mathf.Lerp(statedVolume, 0f, percent);
			yield return null;
		}
		if (afterFunc != null)
			afterFunc();
	}

	private IEnumerator AnimateTrackFadeInOf(AudioSource source, float fadeDuration)
	{
		float percent = 0;
		source.volume = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			source.volume = Mathf.Lerp(0, 1f, percent);
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
			musicSource.volume = Mathf.Lerp(1, 0f, percent);

			yield return null;
		}

		musicSource.clip = nextTrack;
		musicSource.Play();
		Debug.Log("Playing a track");
		percent = 0;

		while (percent < 1)
		{
			percent += Time.deltaTime * 1 / fadeDuration;
			
			musicSource.volume = Mathf.Lerp(0, 1f, percent);
			Debug.Log(musicSource.volume);
			yield return null;
		}
	}


}
