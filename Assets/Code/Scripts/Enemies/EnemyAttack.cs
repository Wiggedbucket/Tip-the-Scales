using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] Material attackIndicator;
    [SerializeField] Renderer rend;
    EnemyFSM fsm;
    EnemyStats stats;
    NavMeshAgent agent;
    bool isAttacking = false;
    void Start()
    {
        fsm = GetComponent<EnemyFSM>();
        stats = fsm.stats;
        agent = GetComponent<NavMeshAgent>();
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
        rend.material = attackIndicator;
        //Debug.Log(stats.enemyName + " attack sequence STARTED");
        Vector3 directionToPLayer = (fsm.player.position - transform.position).normalized;
        yield return new WaitForSeconds(stats.anticipationTime);

        if (stats.useLunge)
        {
            agent.enabled = false;
            float lungeDuration = 0.15f;
            float timer = 0f;
            while (timer < lungeDuration)
            {
                transform.position += directionToPLayer * stats.lungeForce * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            agent.enabled = true;

            float distanceAfterLunge = Vector3.Distance(transform.position, fsm.player.position);
            if (distanceAfterLunge <= stats.damageRange)
            {
                PlayerHealth playerHealth = fsm.player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(stats.attackDamage);
                    Debug.Log(stats.enemyName + " lunge hit! Distance was: " + distanceAfterLunge);
                }
            }
            else
            {
                Debug.Log(stats.enemyName + " lunge missed! Distance was: " + distanceAfterLunge);
            }
        }
        else
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, stats.attackRange);
            //Debug.Log("OverlapSphere found: " + hits.Length + " colliders");
            foreach (Collider hit in hits)
            {
                Debug.Log("Hit: " + hit.gameObject.name);
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(stats.attackDamage);
                }
            }
        }
            

        fsm.lastAttackTime = Time.time;
        fsm.currentState = EnemyFSM.State.Recover;
        isAttacking = false;
        //Debug.Log(stats.enemyName + " attack sequence COMPLETED");
    }
}
