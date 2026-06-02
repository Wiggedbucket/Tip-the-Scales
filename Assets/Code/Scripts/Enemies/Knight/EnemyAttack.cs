using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    EnemyFSM fsm;
    EnemyStats stats;
    bool isAttacking = false;
    void Start()
    {
        fsm = GetComponent<EnemyFSM>();
        stats = fsm.stats;
    }
    private void Update()
    {
        if (fsm.currentState == EnemyFSM.State.Attack && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }
    IEnumerator AttackSequence()
    {
        isAttacking = true;
        Debug.Log(stats.enemyName + " winds up attack...");
        yield return new WaitForSeconds(stats.anticipationTime);

        Debug.Log(stats.enemyName + " attacks!");
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.attackRange);
        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(stats.attackDamage);
            }
        }
        fsm.lastAttackTime = Time.time;
        fsm.currentState = EnemyFSM.State.Recover;
        isAttacking = false;
    }
}
