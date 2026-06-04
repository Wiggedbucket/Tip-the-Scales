using UnityEngine;

public class viscountAttack : MonoBehaviour
{
    EnemyFSM fsm;
    EnemyStats stats;
    float lastFireTime;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    void Start()
    {
        fsm = GetComponent<EnemyFSM>();
        stats = fsm.stats;
    }
    void Update()
    {
        if (fsm.currentState != EnemyFSM.State.Attack) return;
        if (Time.time < fsm.lastKiteEndTime + 0.3f) return;
        if (Time.time < lastFireTime + stats.attackCooldown) return;
        Fire();
        lastFireTime = Time.time;
    
    }

    void Fire()
    {
        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Initialize(fsm.player, stats.homingStrength, stats.projectileSpeed, stats.attackDamage);
    }
}
