using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
    #region Singleton Setup
    public static GameState Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            // Registers the first valid instance before the rest of the scene startup flow.
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    [Range(-1f, 1f)]
    public float Scale = 0f;

    [Range(-1f, 1f)]
    public float ScaleTreshold = -0.8f;

    public bool InPermaDeathRange => Scale <= ScaleTreshold;

    public bool PlayerIsPermaDead = false;

    public CombatPoints GlobalCombatPoints
    {
        get
        {
            CombatPoints total = new();

            foreach (CombatPoints room in RoomCombatPointsList)
            {
                total.angelPoints += room.angelPoints;
                total.demonPoints += room.demonPoints;
            }

            return total;
        }
    }
    public List<CombatPoints> RoomCombatPointsList;

    public float GameTime = 0f;

    public bool IsPaused = false;

    public EventBinding<EnemyDiedEvent> enemyDiedBinding;

    private void OnEnable()
    {
        enemyDiedBinding = new EventBinding<EnemyDiedEvent>(EnemyDeath);
        EventBus<EnemyDiedEvent>.Register(enemyDiedBinding);
    }

    private void OnDisable()
    {
        EventBus<EnemyDiedEvent>.Deregister(enemyDiedBinding);
    }

    private void Update()
    {
        GameTime += Time.deltaTime;

        CalculateScale();
    }

    private void CalculateScale()
    {
        float pointsLight = GlobalCombatPoints.angelPoints;
        float pointsDark = GlobalCombatPoints.demonPoints;

        float rawScore = (pointsLight - pointsDark) / 100f;

        float clamped = Mathf.Clamp(rawScore, -1f, 1f);
        float newScore = (float)System.Math.Round(clamped, 2);

        Scale = newScore;
    }

    private void EnemyDeath(EnemyDiedEvent e)
    {
        //Debug.Log("GameState received enemy death event");
        if(e.RoomID < 0 || e.RoomID >= RoomCombatPointsList.Count)
        {
            return;
        }

        CombatPoints room = RoomCombatPointsList[e.RoomID];
        room.angelPoints += e.Points;
        RoomCombatPointsList[e.RoomID] = room;
    }

    [ContextMenu("Toggle Paused State")]
    public void TogglePauseGame()
    {
        IsPaused = !IsPaused;

        EventBus<PauseGameStateChangedEvent>.Raise(new PauseGameStateChangedEvent()
        {
            IsPaused = IsPaused,
        });

        if (!GameMode.IsMultiplayer)
            Time.timeScale = IsPaused ? 0f : 1f;
    }
}

[System.Serializable]
public struct CombatPoints
{
    public int angelPoints;
    public int demonPoints;
}