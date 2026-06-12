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
    public int maxAmmo = 24;
    public int currentAmmo;
    public float reloadTime = 0.8f;

    [Header("Firing Mode Settings")]
    public fireMode currentFireMode = fireMode.Automatic;
    private bool hasFiredSingle = false;

    [Header("References")]
    public Camera playerCam;
    private bool isShooting = false;
    private bool hasAmmo = true;
    private bool isReloading = false; //still need to use so can't shoot while reloading & add reloading time.

    private void Awake()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        weaponAnimator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        AmmoCheck();
        ShootCheck();
    }

    public void AmmoCheck()
    {
        if (currentAmmo <= 0)
        {
            hasAmmo = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (hasAmmo == false || currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
            hasAmmo = true;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (hasAmmo == true)
        { 
            if (context.started)
            {
                isShooting = true;
            }
            else if (context.canceled)
            {
                isShooting = false;
                hasFiredSingle = false;
            }
        }
    }

    public void ShootCheck()
    {
        if (!isShooting || Time.time < nextTimeToFire) return;
        {
            if (currentFireMode == fireMode.Automatic )
            {
                ExecuteShot();
                currentAmmo -= 1;
            }
            else if (currentFireMode == fireMode.SingleShot)
            {
                if (!hasFiredSingle)
                {
                    ExecuteShot();
                    currentAmmo -= 1;
                    hasFiredSingle =true;
                }
            }
        }
    }

    public void ExecuteShot()
    {
        if (hasAmmo == true)
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
        else
        {
            Debug.Log("No ammo");
        }
    }
}
