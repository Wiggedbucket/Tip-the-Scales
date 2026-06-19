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
            MatchEndMenu.Instance.OpenMenu(false);

            GameState.Instance.PlayerIsPermaDead = true;

            // Send death to the server (If in multiplayer)
        }
        else
        {
            PlayerCage.Instance.SendPlayerToJail(player);
            health.Revive();
        }
    }
}