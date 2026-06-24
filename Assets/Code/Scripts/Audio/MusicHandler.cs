using System.Collections;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler instance;

	[Header("References")]
    public AudioSource angelicChoir;
    public AudioSource demonicChoir;

	[Header("Choir values")]
	public float waitTime = 5f;
	public float fadeTime = 2f;
	[Range(0f, 1f)]
	public float angelWinningTreshhold = .2f;
	[Range(-1f, 0f)]
	public float demonWinningTreshhold = -.2f;

	[Header("Damping Values")]
	public float dampCutoff = 1000f;
	public float dampFadeDuration = 2f;

	//HELPERS
	public bool IsOSTPlaying { get; set; } = false;
	private bool isWaiting = false;
	private int winningSide = 0;
	private bool isDamped = false;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		angelicChoir.volume = 0f;
		demonicChoir.volume = 0f;
	}

	private void Update()
	{
		if(GameState.Instance.Scale > angelWinningTreshhold && !isWaiting && winningSide != 1)
		{
			isWaiting = true;
			StartCoroutine(WaitingAngelRoutine());
		}
		if (GameState.Instance.Scale < demonWinningTreshhold && !isWaiting && winningSide != -1)
		{
			isWaiting = true;
			StartCoroutine(WaitingDemonRoutine());
		}
		if (GameState.Instance.Scale > demonWinningTreshhold && GameState.Instance.Scale < angelWinningTreshhold && winningSide != 0 && !isWaiting)
		{
			isWaiting = true;
			StartCoroutine(WaitingNeutralRoutine());
		}
	}

	public void SetOSTVolume(float volume)
	{
		volume = Mathf.Clamp01(volume);

		MusicManager.instance.SetVolume(volume);
		angelicChoir.volume = volume;
		demonicChoir.volume = volume;
	}

	public void MultiplyOSTVolumeBy(float multiplier)
	{
		multiplier = Mathf.Clamp01(multiplier);

		MusicManager.instance.MultiplyVolumeBy(multiplier);
		angelicChoir.volume *= multiplier;
		demonicChoir.volume *= multiplier;
	}

	public void DampenOST()
	{
		if (!IsOSTPlaying)
			return;

		if (isDamped)
			return;

		isDamped = true;

		MusicManager.instance.DampenTrack(dampCutoff, dampFadeDuration);
		MusicManager.instance.DampenTrackOf(angelicChoir, dampCutoff, dampFadeDuration);
		MusicManager.instance.DampenTrackOf(demonicChoir, dampCutoff, dampFadeDuration);
	}

	public void UnDampenOST()
	{
		if (!IsOSTPlaying)
			return;

		if (!isDamped)
			return;

		isDamped = false;
		MusicManager.instance.UnDampedTrack(dampFadeDuration);
		MusicManager.instance.UnDampenTrackOf(angelicChoir, dampFadeDuration);
		MusicManager.instance.UnDampenTrackOf(demonicChoir, dampFadeDuration);
	}

	public void PlayOST()
	{
		if (IsOSTPlaying)
			return;

		IsOSTPlaying = true;

		double startTime = AudioSettings.dspTime + .1f;

		MusicManager.instance.PlayTrack("MainOST", startTime);
		MusicManager.instance.PlayTrackOf(angelicChoir, startTime);
		MusicManager.instance.PlayTrackOf(demonicChoir, startTime);
	}

	public void PlayAngelicChoir()
	{
		StartCoroutine(MusicManager.instance.AnimateTrackFadeInFromPauseOf(angelicChoir, fadeTime));
	}

	public void StopAngelicChoir()
	{
		StartCoroutine(MusicManager.instance.AnimateTrackFadeOutOf(angelicChoir, fadeTime));
	}
	public void PlayDemonicChoir()
	{
		StartCoroutine(MusicManager.instance.AnimateTrackFadeInFromPauseOf(demonicChoir, fadeTime));
	}

	public void StopDemonicChoir()
	{
		StartCoroutine(MusicManager.instance.AnimateTrackFadeOutOf(demonicChoir, fadeTime));
	}

	private IEnumerator WaitingAngelRoutine()
	{
		float timer = 0;
		Debug.Log("[AdaptiveMusic]Waiting angel");
		do
		{
			timer += Time.deltaTime;
			if (GameState.Instance.Scale < angelWinningTreshhold)
			{
				isWaiting = false;
				Debug.Log("[AdaptiveMusic]Failed to play angel at " + timer);
				yield break;
			}
			yield return null;
		} while (timer < waitTime);

		isWaiting = false;
		winningSide = 1;
		Debug.Log("[AdaptiveMusic]Angelic choir is fading in");
		StopDemonicChoir();
		PlayAngelicChoir();
	}

	private IEnumerator WaitingDemonRoutine()
	{
		float timer = 0;
		Debug.Log("[AdaptiveMusic]Waiting demon");
		do
		{
			timer += Time.deltaTime;
			if (GameState.Instance.Scale > demonWinningTreshhold)
			{
				isWaiting = false;
				Debug.Log("[AdaptiveMusic]Failed to play demon at " + timer);
				yield break;
			}
			yield return null;
		} while (timer < waitTime);

		isWaiting = false;
		winningSide = -1;
		Debug.Log("[AdaptiveMusic]Demonic choir is fading in");
		StopAngelicChoir();
		PlayDemonicChoir();
	}

	private IEnumerator WaitingNeutralRoutine()
	{
		float timer = 0;
		Debug.Log("[AdaptiveMusic]Waiting neutral");
		do
		{
			timer += Time.deltaTime;
			if (GameState.Instance.Scale < demonWinningTreshhold || GameState.Instance.Scale > angelWinningTreshhold)
			{
				isWaiting = false;
				Debug.Log("[AdaptiveMusic]Failed to play neutral at " + timer);
				yield break;
			}
			yield return null;
		} while (timer < waitTime);

		isWaiting = false;
		winningSide = 0;
		Debug.Log("[AdaptiveMusic]Neutral state of the song");
		StopAngelicChoir();
		StopDemonicChoir();
	}
}
