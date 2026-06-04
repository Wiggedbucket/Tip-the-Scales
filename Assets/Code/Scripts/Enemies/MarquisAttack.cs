using UnityEngine;

public class MarquisAttack : MonoBehaviour
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
    private void Update()
    {
        if (fsm.currentState != EnemyFSM.State.Attack) return;
    }
}
