using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false; // One-time activation
    [SerializeField] private SpriteRenderer flagSprite; // Optional visual change

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;

            // Set this checkpoint as the current spawn point
            CheckpointManager.Instance.SetCheckpoint(transform.position);

            // Optional: change color to show it's activated
            if (flagSprite != null)
                flagSprite.color = Color.green;

            Debug.Log("Checkpoint activated at " + transform.position);
        }
    }
}
