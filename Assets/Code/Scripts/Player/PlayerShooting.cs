using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public float Range = 100f;
    public float Damage = 100f;

    public Camera playerCam;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            RaycastHit hit;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit))
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
}
