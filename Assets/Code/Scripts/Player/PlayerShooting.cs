using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Models")]
    [SerializeField] private GameObject rifleModel;
    [SerializeField] private GameObject shotgunModel;

    [Header("Weapons")]
    [SerializeField] private WeaponStats rifleStats;
    [SerializeField] private WeaponStats shotgunStats;

    [Header("References")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Audio")]
    [SerializeField] private string gunshotSound = "";
    [SerializeField] private string reloadSound = "";

    [Header("Input Actions")]
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference weapon1Action;
    [SerializeField] private InputActionReference weapon2Action;

    private WeaponStats currentWeapon;
    public WeaponStats CurrentWeapon => currentWeapon;

    private int currentAmmo;
    public int CurrentAmmo => currentAmmo;

    private float nextTimeToFire;

    private bool isShooting;
    private bool hasFiredSingle;
    private bool hasAmmo;
    
    private void Awake()
    {
        EquipRifle();
    }

    private void OnEnable()
    {
        reloadAction.action.performed += OnReloadPerformed;
        weapon1Action.action.performed += OnWeapon1Performed;
        weapon2Action.action.performed += OnWeapon2Performed;
    }

    private void OnDisable()
    {
        reloadAction.action.performed -= OnReloadPerformed;
        weapon1Action.action.performed -= OnWeapon1Performed;
        weapon2Action.action.performed -= OnWeapon2Performed;
    }

    void Update()
    {
        ShootCheck();
    }

    private void OnWeapon1Performed(InputAction.CallbackContext context)
    {
        EquipRifle();
    }

    private void OnWeapon2Performed(InputAction.CallbackContext context)
    {
        EquipShotgun();
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

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        if (currentAmmo >= currentWeapon.MaxAmmo)
            return;

        if (!string.IsNullOrEmpty(reloadSound))
        {
            SoundManager.instance.CreateSound().WithSoundData(reloadSound).WithrandomPitch().Play();
        }

        currentAmmo = currentWeapon.MaxAmmo;
        hasAmmo = true;

        EventBus<StyleGainEvent>.Raise(new StyleGainEvent
        {
            Amount = 1,
            Reason = "Reloaded",
            TextColor = Color.yellow,
        });
    }

    private void EquipRifle()
    {
        rifleModel.SetActive(true);
        shotgunModel.SetActive(false);

        EquipWeapon(rifleStats);
    }

    private void EquipShotgun()
    {
        rifleModel.SetActive(false);
        shotgunModel.SetActive(true);

        EquipWeapon(shotgunStats);
    }

    private void EquipWeapon(WeaponStats weapon)
    {
        currentWeapon = weapon;
        currentAmmo = weapon.MaxAmmo;
        hasAmmo = true;
        hasFiredSingle = false;
    }

    private void ShootCheck()
    {
        if (GameState.Instance.IsPaused || GameState.Instance.MatchEnded)
            return;

        if (!isShooting)
            return;

        if (!hasAmmo)
            return;

        if (Time.time < nextTimeToFire)
            return;

        switch (currentWeapon.FireMode)
        {
            case FireMode.Automatic:
                FireWeapon();
                break;

            case FireMode.SingleShot:
                if (!hasFiredSingle)
                {
                    FireWeapon();
                    hasFiredSingle = true;
                }
                break;

            case FireMode.Shotgun:
                if (!hasFiredSingle)
                {
                    FireWeapon();
                    hasFiredSingle = true;
                }
                break;
        }
    }

    private void FireWeapon()
    {
        nextTimeToFire = Time.time + currentWeapon.FireRate;

        if (!string.IsNullOrEmpty(gunshotSound))
        {
            SoundManager.instance.CreateSound().WithSoundData(gunshotSound).WithrandomPitch().Play();
        }

        int pelletCount = Mathf.Max(1, currentWeapon.PelletsPerShot);

        for (int i = 0; i < pelletCount; i++)
        {
            FirePellet();
        }

        currentAmmo--;

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            hasAmmo = false;
        }

        EventBus<WeaponFiredEvent>.Raise(new WeaponFiredEvent
        {
            Weapon = currentWeapon.WeaponName,
        });
    }

    private void FirePellet()
    {
        Vector2 spread = Random.insideUnitCircle * currentWeapon.BulletSpread;

        Vector3 direction = playerCam.transform.forward + playerCam.transform.right * spread.x + playerCam.transform.up * spread.y;

        if (Physics.Raycast(playerCam.transform.position, direction, out RaycastHit hit, currentWeapon.Range, enemyLayer))
        {
            Health health = hit.transform.GetComponentInParent<Health>();

            if (health != null)
            {
                float damagePerPellet = currentWeapon.Damage / Mathf.Max(1, currentWeapon.PelletsPerShot);

                health.TakeDamage(damagePerPellet);

                EventBus<StyleGainEvent>.Raise(new StyleGainEvent
                {
                    Amount = damagePerPellet * 0.5f,
                    Reason = "Shot Hit",
                    TextColor = Color.yellow,
                });
            }
        }
        else
        {
            EventBus<StyleGainEvent>.Raise(new StyleGainEvent
            {
                Amount = -1,
                Reason = "You shot air",
                TextColor = Color.red,
            });
        }
    }
}
