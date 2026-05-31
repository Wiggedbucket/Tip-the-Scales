using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Sound
{
	public bool enabled = true;
	public string name;
	public AudioClip clip;
}

public class WeaponSoundEvents : MonoBehaviour
{
    public Sound[] sounds;

	public void PlaySound(string soundName)
	{
		Sound sound = Array.Find<Sound>(sounds, el => el.name == soundName);
		if (sound.enabled)
			SoundFXManager.instance.PLaySoundFXClip(sound.clip, transform, 1);
	}
}
