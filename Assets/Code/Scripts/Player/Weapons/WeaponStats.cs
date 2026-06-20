[System.Serializable]
public class WeaponStats
{
    public string WeaponName;
    public FireMode FireMode;
    public float Damage;
    public float Range;
    public float FireRate;
    public float BulletSpread;
    public int MaxAmmo;
    public int PelletsPerShot;
}

public enum FireMode
{
    Automatic,
    SingleShot,
    Shotgun,
}