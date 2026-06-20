using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Tip the Scales/Enemy Stats")]
[Serializable]
public class EnemyStats : ScriptableObject
{
    [SerializeField] public string enemyName = "Knight";
    [SerializeField] public float moveSpeed = 4f;
    [SerializeField] public float attackDamage = 10f;
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public float attackCooldown = 2f;
    [SerializeField] public float anticipationTime = 0.4f;
    [SerializeField] public bool useLunge = false;
    [SerializeField] public float lungeForce = 8f;
    [SerializeField] public float lungeRange = 2f;
    [SerializeField] public float damageRange = 1.5f;
    [SerializeField] public bool canKite = false;
    [SerializeField] public float kiteDistance = 3f;
    [SerializeField] public float fireRange = 10f;
    [SerializeField] public float projectileSpeed = 8f;
    [SerializeField] public float homingStrength = 2f;
}
