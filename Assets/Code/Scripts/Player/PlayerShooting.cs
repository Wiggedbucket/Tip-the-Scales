using UnityEngine;
using UnityEngine.InputSystem;

public enum FireMode
{
    Automatic,
    SingleShot
}

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
    public float bulletSpread = 0.1f;

    [Header("Firing Mode Settings")]
    public FireMode currentFireMode = FireMode.Automatic;
    private bool hasFiredSingle = false;

    [Header("References")]
    public Camera playerCam;
    private bool isShooting = false;
    private bool hasAmmo = true;
    //private bool isReloading = false; //still need to use so can't shoot while reloading & add reloading time.

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
            currentAmmo = 0;
            hasAmmo = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {

        if (context.performed && currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
            hasAmmo = true;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {

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
        if (!isShooting || Time.time < nextTimeToFire || !hasAmmo) return;
        {
            if (currentFireMode == FireMode.Automatic )
            {
                ExecuteShot();
                currentAmmo -= 1;
            }
            else if (currentFireMode == FireMode.SingleShot)
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
            nextTimeToFire = Time.time + firerate;

        Vector2 shotSpread = Random.insideUnitCircle * bulletSpread;
        Vector3 direction = playerCam.transform.forward + (playerCam.transform.right * shotSpread.x) + (playerCam.transform.up * shotSpread.y);
        

            RaycastHit hit;

            if (Physics.Raycast(playerCam.transform.position, direction, out hit, range))
            {
                Debug.Log(hit.transform.name);

                Health health = hit.transform.GetComponentInParent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        
        else
        {
            Debug.Log("Miss");
        }
    }
}
