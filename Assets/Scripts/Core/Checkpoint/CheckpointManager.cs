using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 currentCheckpoint;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private bool freezeDuringRespawn = true;

    [Header("Player References")]
    [SerializeField] private GameObject dayPlayer;
    [SerializeField] private GameObject nightPlayer;

    private GameObject activePlayer;
    private Rigidbody2D activeRb;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dayPlayer == null || nightPlayer == null)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in allPlayers)
            {
                if (player.name.ToLower().Contains("day"))
                    dayPlayer = player;
                else if (player.name.ToLower().Contains("night"))
                    nightPlayer = player;
            }
        }

        UpdateActivePlayerReference();
    }

    private void Start()
    {
        if (activePlayer != null)
            currentCheckpoint = activePlayer.transform.position;
    }

    private void Update()
    {
        UpdateActivePlayerReference();
    }

    private void UpdateActivePlayerReference()
    {
        if (dayPlayer != null && dayPlayer.activeInHierarchy)
        {
            activePlayer = dayPlayer;
            activeRb = dayPlayer.GetComponent<Rigidbody2D>();
        }
        else if (nightPlayer != null && nightPlayer.activeInHierarchy)
        {
            activePlayer = nightPlayer;
            activeRb = nightPlayer.GetComponent<Rigidbody2D>();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        Debug.Log("Checkpoint updated to " + currentCheckpoint);
    }

    public void RespawnPlayer()
    {
        if (activePlayer == null)
            UpdateActivePlayerReference();

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        DisableMovement(dayPlayer);
        DisableMovement(nightPlayer);

        yield return new WaitForSeconds(respawnDelay);

        MoveEvenIfInactive(dayPlayer);
        MoveEvenIfInactive(nightPlayer);

        Debug.Log("Both players moved to checkpoint: " + currentCheckpoint);

        yield return new WaitForFixedUpdate();

        EnableMovement(dayPlayer);
        EnableMovement(nightPlayer);
    }

    private void DisableMovement(GameObject player)
    {
        if (player == null) return;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (freezeDuringRespawn && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void EnableMovement(GameObject player)
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (freezeDuringRespawn && rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = true;
    }

    private void MoveEvenIfInactive(GameObject player)
    {
        if (player == null) return;

        bool wasInactive = !player.activeSelf;

        if (wasInactive)
            player.SetActive(true);

        player.transform.position = currentCheckpoint;

        // Force physics sync
        if (player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.position = currentCheckpoint;
        }

        if (wasInactive)
            player.SetActive(false);
    }
}
