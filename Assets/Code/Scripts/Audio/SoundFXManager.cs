using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

	[SerializeField] private AudioSource soundFXObject;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	public void PLaySoundFXClip(AudioClip audioClip, Transform position, float volume)
	{
		AudioSource source = Instantiate(soundFXObject, position.position, Quaternion.identity);
		source.clip = audioClip;
		source.volume = volume;
		source.Play();
		float clipLength = source.clip.length;
		Destroy(source.gameObject, clipLength);
	}
}
