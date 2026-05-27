using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyRegistry Registry => EnemyRegistryDatabase.Registry;

    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField] private float spawnDelay = 0.2f;

    [SerializeField] private int maxEnemiesAlive = 500;

    private readonly List<GameObject> aliveEnemies = new();

    public IEnumerator SpawnWave(int budget, int wave)
    {
        CleanupEnemies();

        int safety = 1000;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Debug.Log($"Starting wave with a budget of {budget}");
        while (budget > 0 && safety-- > 0)
        {
            if (aliveEnemies.Count >= maxEnemiesAlive)
                break;

            EnemyData enemy = GetEnemyForBudget(budget, wave);

            if (enemy.enemy == null)
                break;

            SpawnEnemy(enemy, spawnPoint);

            budget -= enemy.cost;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private EnemyData GetEnemyForBudget(int budget, int wave)
    {
        List<EnemyData> validEnemies = new();

        float totalWeight = 0f;

        foreach (EnemyData enemy in Registry.enemies)
        {
            if (enemy.enemy == null)
                continue;

            if (enemy.cost > budget)
                continue;

            if (wave < enemy.minimumWave)
                continue;

            if (enemy.maximumWave > 0 && wave > enemy.maximumWave)
                continue;

            float curveValue = enemy.appearanceChance.Evaluate(Mathf.Clamp01((float)wave / enemy.maximumWave));

            float finalWeight = curveValue * enemy.weight;

            if (finalWeight <= 0f)
                continue;

            validEnemies.Add(enemy);

            totalWeight += finalWeight;
        }

        if (validEnemies.Count == 0)
            return default;

        float random = Random.Range(0f, totalWeight);

        float current = 0f;

        foreach (EnemyData enemy in validEnemies)
        {
            float curveValue = enemy.appearanceChance.Evaluate(Mathf.Clamp01((float)wave / enemy.maximumWave));

            float finalWeight = curveValue * enemy.weight;

            current += finalWeight;

            if (random <= current)
                return enemy;
        }

        return validEnemies[0];
    }

    private void SpawnEnemy(EnemyData enemyData, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyData.enemy, spawnPoint.position, spawnPoint.rotation);

        aliveEnemies.Add(enemy);
    }

    private void CleanupEnemies()
    {
        aliveEnemies.RemoveAll(e => e == null);
    }
}