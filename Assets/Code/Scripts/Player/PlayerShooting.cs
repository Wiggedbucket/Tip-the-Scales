using AudioSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FireMode
{
    Automatic,
    SingleShot,
    Shotgun
}

public class PlayerShooting : MonoBehaviour
{
    AudioSource audioSource;
    Animator weaponAnimator;

    [Header("WeaponModels")]
    public GameObject rifleModel;
    public GameObject shotgunModel;

    [Header("WeaponStats")]
    public float damage = 25f;
    public float range = 100f;
    public float firerate = 0.2f;
    private float nextTimeToFire = 0f;
    public int maxAmmo = 24;
    public int currentAmmo;
    public float reloadTime = 0.8f;
    public float bulletSpread = 0.1f;
    public int pelletsPerShot = 2;

    [Header("Firing Mode Settings")]
    public FireMode currentFireMode = FireMode.Automatic;
    private bool hasFiredSingle = false;

    [Header("References")]
    public LayerMask enemyLayer;
    public Camera playerCam;
    private bool isShooting = false;
    private bool hasAmmo = true;

	//private bool isReloading = false; //still need to use so can't shoot while reloading & add reloading time.

	[Header("Audio")]
    public string gunshotSound = "";
    public string reloadSound = "";
    
    private void Awake()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        weaponAnimator = GetComponentInChildren<Animator>();
        EquipRifle();
    }
    void Update()
    {
        AmmoCheck();
        ShootCheck();
    }

    public void onWeaponSelect1 (InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EquipRifle();
        }
    }

    public void onWeaponSelect2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EquipShotgun();
        }
    }

    private void EquipRifle()
    {
        rifleModel.SetActive(true);
        shotgunModel.SetActive(false);

        currentFireMode = FireMode.Automatic;
        damage = 10f;
        firerate = 0.1f;
        bulletSpread = 0.05f;
        maxAmmo = 25;
        range = 50f;

        currentAmmo = maxAmmo;
        hasAmmo = true;
    }

    private void EquipShotgun()
    {
        rifleModel.SetActive (false);
        shotgunModel.SetActive(true);

        currentFireMode = FireMode.Shotgun;
        damage = 80f;
        firerate = 0.2f;
        bulletSpread = 0.1f;
        maxAmmo = 2;
        range = 30f;

        currentAmmo = maxAmmo;
        hasAmmo = true;
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
            if(reloadSound != "")
                SoundManager.instance.CreateSound().WithSoundData(reloadSound).WithrandomPitch().Play();
            currentAmmo = maxAmmo;
            hasAmmo = true;
            EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
            {
                Amount = 1,
                Reason = "Reloaded",
                TextColor = Color.yellow,
            });
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
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

    public void ShootCheck()
    {
        if (GameState.Instance.IsPaused || GameState.Instance.PlayerIsPermaDead)
            return;

        if (!isShooting || Time.time < nextTimeToFire || !hasAmmo) return;
        {
            if (currentFireMode == FireMode.Automatic )
            {
                ExecuteShot();
                currentAmmo -= 1;
                EventBus<WeaponFiredEvent>.Raise(new WeaponFiredEvent()
                {
                    Weapon = "Shotgun",
                });
            }
            else if (currentFireMode == FireMode.SingleShot)
            {
                if (!hasFiredSingle)
                {
                    ExecuteShot();
                    currentAmmo -= 1;
                    hasFiredSingle =true;
                    EventBus<WeaponFiredEvent>.Raise(new WeaponFiredEvent()
                    {
                        Weapon = "Shotgun",
                    });
                }
            }
            else if (currentFireMode == FireMode.Shotgun)
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
        if(gunshotSound != "")
            SoundManager.instance.CreateSound().WithSoundData(gunshotSound).WithrandomPitch().Play();
        
        nextTimeToFire = Time.time + firerate;

        int bulletsPerShot = (currentFireMode == FireMode.Shotgun) ? pelletsPerShot : 1;

        Vector2 shotSpread = Random.insideUnitCircle * bulletSpread;
        Vector3 direction = playerCam.transform.forward + (playerCam.transform.right * shotSpread.x) + (playerCam.transform.up * shotSpread.y);
        

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, direction, out hit, range, enemyLayer))
        {
            Debug.Log(hit.transform.name);

            Health health = hit.transform.GetComponentInParent<Health>();
            if (health != null)
            {
            float calculatedDamage = (currentFireMode == FireMode.Shotgun) ? (damage / pelletsPerShot) : damage;

                health.TakeDamage(calculatedDamage);

                EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
                {
                    Amount = damage * 0.5f,
                    Reason = "Shot Hit",
                    TextColor = Color.yellow,
                });
            }
        }
        else
        {
            Debug.Log("Miss");
            EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
            {
                Amount = -1,
                Reason = "you shot air",
                TextColor = Color.red,
            });
        }
    }
}
