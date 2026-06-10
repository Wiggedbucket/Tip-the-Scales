using System.Collections.Generic;
using UnityEngine;

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

    public bool isServerConnected = false;

    [Range(-1f, 1f)]
    public float Scale = 0f;

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

    [ContextMenu("Toggle Paused State")]
    public void TogglePauseGame()
    {
        IsPaused = !IsPaused;

        EventBus<PauseGameStateChangedEvent>.Raise(new PauseGameStateChangedEvent()
        {
            IsPaused = IsPaused,
        });

        Time.timeScale = IsPaused ? 0f : 1f;
    }
}

[System.Serializable]
public struct CombatPoints
{
    public int angelPoints;
    public int demonPoints;
}