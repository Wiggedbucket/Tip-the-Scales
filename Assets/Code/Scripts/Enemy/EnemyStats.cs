using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStats", menuName = "Tip the Scales/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField] public string enemyName = "Knight";
    [SerializeField] public float maxHealth = 50f;
    [SerializeField] public float moveSpeed = 4f;
    [SerializeField] public float attackDamage = 10f;
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public float attackCooldown = 2f;
    [SerializeField] public float anticipationTime = 0.4f;
}
