using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;

public enum WeaponType
{
    Hitscan,
    Projectile
}

[CreateAssetMenu(fileName = "Gun", menuName = "Gun/Guns")]
public class GunData : ScriptableObject
{

    [Header("Weapon")]
    [SerializeField] public WeaponType type;
    [SerializeField] public bool automatic = true;
    [SerializeField] public int maxAmmo = 2;
    [SerializeField] public string Name;
    [SerializeField] public int Damage = 25;
    [SerializeField] public float FireRate = 0.5f;
    [SerializeField] public float ReloadTime = 0.5f;
    [SerializeField] public float EquipTime = 0.5f;
    [SerializeField] public float Range = 100f;
    [SerializeField] public float headShotMultiplier = 1.5f;

    [Header("Muzzle")]
    public GameObject fireEffect;
    public AudioClip fireSound;

    [Header("Raycast")]
    public GameObject hitEffect;

    //[Header("Prefab")]
    //public something weaponPrefab;
}
