using UnityEngine;
using Utilities;

public class HordeDirector : MonoBehaviour
{
    public int id = 0;

    [SerializeField] private EnemySpawner enemySpawner;

    private CombatPoints combatPoints;

    private int currentWave = 0;

    private float GameTime => GameState.Instance.GameTime;

    public int baseBudget = 10;

    private CountdownTimer countdownTimer;

    [Tooltip("The amount of time it takes for the next wave to start, gets ignored for first wave.")]
    private float waveTimeInterval = 20f;

    private void Start()
    {
        combatPoints = GameState.Instance.RoomCombatPointsList[id];

        countdownTimer = new CountdownTimer(waveTimeInterval);
        countdownTimer.OnTimerStop += StartWave;
        countdownTimer.Start();
    }

    private void Update()
    {
        countdownTimer.Tick(Time.deltaTime);
    }

    private void StartWave()
    {
        GameState.Instance.NextWave();

        int budget = CalculateBudget();

        StartCoroutine(enemySpawner.SpawnWave(budget, currentWave));

        countdownTimer.Reset(waveTimeInterval);
        countdownTimer.Start();
    }

    private int CalculateBudget()
    {
        int timeScaling = Mathf.FloorToInt(GameTime / 60f);

        int waveScaling = currentWave * 2;

        int scoreDifference = Mathf.Abs(combatPoints.demonPoints - combatPoints.angelPoints);

        int pressureScaling = Mathf.Clamp(scoreDifference / 10, 0, 10);

        return baseBudget
               + timeScaling
               + waveScaling
               + pressureScaling;
    }
}
