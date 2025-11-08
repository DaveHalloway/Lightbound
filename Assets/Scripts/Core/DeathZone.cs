using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit death zone!");

            // Lose one life
            LivesCount.LoseLife();

            // Respawn the player at the last checkpoint
            CheckpointManager.Instance.RespawnPlayer();
        }
    }
}
