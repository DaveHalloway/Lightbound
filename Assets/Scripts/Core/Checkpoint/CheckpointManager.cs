using System.Collections;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public static System.Action OnRespawned;

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

            foreach (GameObject p in allPlayers)
            {
                if (p.name.ToLower().Contains("day"))
                    dayPlayer = p;
                else if (p.name.ToLower().Contains("night"))
                    nightPlayer = p;
            }
        }

        UpdateActivePlayer();
    }

    private void Start()
    {
        if (activePlayer != null)
            currentCheckpoint = activePlayer.transform.position;
    }

    private void Update()
    {
        UpdateActivePlayer();
    }

    private void UpdateActivePlayer()
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

    public void SetCheckpoint(Vector3 pos)
    {
        currentCheckpoint = pos;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        DisableMovement(dayPlayer);
        DisableMovement(nightPlayer);

        yield return new WaitForSeconds(respawnDelay);

        MovePlayer(dayPlayer);
        MovePlayer(nightPlayer);

        yield return new WaitForFixedUpdate();

        EnableMovement(dayPlayer);
        EnableMovement(nightPlayer);

        OnRespawned?.Invoke();
    }

    private void DisableMovement(GameObject player)
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

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

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
    }

    private void MovePlayer(GameObject player)
    {
        if (player == null) return;

        bool wasInactive = !player.activeSelf;
        if (wasInactive) player.SetActive(true);

        player.transform.position = currentCheckpoint;

        if (player.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.position = currentCheckpoint;
        }

        if (wasInactive) player.SetActive(false);
    }
}
