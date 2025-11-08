using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 currentCheckpoint;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private bool freezeDuringRespawn = true;

    private Transform player;
    private Rigidbody2D rb;

    // For multi-player switching
    private GameObject[] allPlayers;
    private int activePlayerIndex = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find all possible player objects in the scene
        allPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (allPlayers.Length == 0)
        {
            Debug.LogError("No GameObjects tagged 'Player' found! Please tag both Night and Day players.");
            return;
        }

        // Automatically set the active one (the one currently enabled)
        UpdateActivePlayerReference();
    }

    private void Start()
    {
        if (player != null)
            currentCheckpoint = player.position;
    }

    private void Update()
    {
        // Detect if you switched players mid-game
        if (HasActivePlayerChanged())
        {
            UpdateActivePlayerReference();
            Debug.Log("? Active player switched to: " + player.name);
        }
    }

    private bool HasActivePlayerChanged()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i].activeInHierarchy && i != activePlayerIndex)
                return true;
        }
        return false;
    }

    private void UpdateActivePlayerReference()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i].activeInHierarchy)
            {
                player = allPlayers[i].transform;
                rb = allPlayers[i].GetComponent<Rigidbody2D>();
                activePlayerIndex = i;
                break;
            }
        }

        if (player == null)
            Debug.LogWarning("No active player found!");
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        Debug.Log("Checkpoint updated to " + currentCheckpoint);
    }

    public void RespawnPlayer()
    {
        if (player == null)
        {
            UpdateActivePlayerReference();
        }

        if (player != null)
            StartCoroutine(RespawnRoutine());
        else
            Debug.LogError("No active player found to respawn!");
    }

    private IEnumerator RespawnRoutine()
    {
        PlayerMovement movementScript = player.GetComponent<PlayerMovement>();
        if (movementScript != null)
            movementScript.enabled = false;

        if (freezeDuringRespawn && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        yield return new WaitForSeconds(respawnDelay);

        if (rb != null)
            rb.MovePosition(currentCheckpoint);
        else
            player.position = currentCheckpoint;

        Debug.Log("? " + player.name + " teleported to " + currentCheckpoint);

        yield return new WaitForFixedUpdate();

        if (freezeDuringRespawn && rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        if (movementScript != null)
            movementScript.enabled = true;
    }
}
