using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    public Animator animator;
    public float fireRate = .3f;
    private float timer = 0;
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MusicManager.instance.PlayTrack("Room Music 1", 5f);
            
        }


        if (Mouse.current.leftButton.isPressed)
        { 
            if(timer >= fireRate)
            {
				animator.Play("Shoot", 0, 0f);
                timer = 0;
			}
            
        }
		timer += Time.deltaTime;
	}
}
