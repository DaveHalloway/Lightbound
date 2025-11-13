using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it’s a player (either day or night)
        if (other.CompareTag("Player"))
        {
            // Only affect the active player
            if (!other.gameObject.activeInHierarchy)
                return;

            Debug.Log("Active player hit death zone!");

            // Lose one life (handled statically)
            LivesCount.LoseLife();

            // Respawn the active player at last checkpoint
            if (CheckpointManager.Instance != null)
                CheckpointManager.Instance.RespawnPlayer();
            else
                Debug.LogWarning("CheckpointManager instance not found!");
        }
    }
}
