using UnityEngine;
using UnityEngine.InputSystem;


public enum fireMode {Automatic, SingleShot}

public class PlayerShooting : MonoBehaviour
{
    AudioSource audioSource;
    Animator weaponAnimator;

    [Header("WeaponStats")]
    public float damage = 25f;
    public float range = 100f;
    public float firerate = 0.2f;
    private float nextTimeToFire = 0f;

    [Header("Firing Mode Settings")]
    public fireMode currentFireMode = fireMode.Automatic;
    private bool hasFiredSingle = false;

    [Header("References")]
    public Camera playerCam;
    private bool isShooting = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        weaponAnimator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        ShootCheck();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isShooting=true;
        }
        else if(context.canceled)
        {
            isShooting=false;
            hasFiredSingle = false;
        }
    }

    public void ShootCheck()
    {
        if (!isShooting || Time.time < nextTimeToFire) return;
        {
            if (currentFireMode == fireMode.Automatic )
            {
                ExecuteShot();
            }
            else if (currentFireMode == fireMode.SingleShot)
            {
                if (!hasFiredSingle)
                {
                    ExecuteShot();
                    hasFiredSingle=true;
                }
            }
        }
    }

    public void ExecuteShot()
    {
        nextTimeToFire = Time.time + firerate;

        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.takeDamage(damage);
            }
        }
    }
}
