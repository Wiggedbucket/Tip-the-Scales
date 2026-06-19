using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    public float fireRate = .3f;
    private float timer = 0;
    public GameObject place;
    public SoundData data;

	void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //MusicManager.instance.PlayTrack("Room Music 1", 5f);
            Debug.Log("Space pressed");
            SoundManager.instance.CreateSound().WithSoundData("ShotgunShoot").WithPosition(place.transform.position).Play();
            
        }


  //      if (Mouse.current.leftButton.isPressed)
  //      { 
  //          if(timer >= fireRate)
  //          {
		//		animator.Play("Shoot", 0, 0f);
  //              timer = 0;
		//	}
            
  //      }
		//timer += Time.deltaTime;
	}
}
