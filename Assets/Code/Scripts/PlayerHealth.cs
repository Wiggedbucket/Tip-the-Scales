using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Health health;

    [SerializeField]
    private GameObject player;

    private void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        if (GameState.Instance.InPermaDeathRange)
        {
            // Kill player
            // Show death screen (Show score on it)
            // Send death to the server (If in multiplayer)
        }
        else
        {
            PlayerCage.Instance.SendPlayerToJail(player);
            health.Revive();
        }
    }
}