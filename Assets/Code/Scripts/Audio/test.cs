using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MusicManager.instance.PlayMusicCrossfade("Room Music 1", 5f);

        }

    }
}
