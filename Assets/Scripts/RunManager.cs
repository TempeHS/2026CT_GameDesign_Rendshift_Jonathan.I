using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [Header("Respawn")]
    public Transform spawnPoint;
    public GameObject playerPrefab;

    private GameObject currentPlayer;
    public Transform CurrentPlayerTransform => currentPlayer != null ? currentPlayer.transform : null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewRun();
    }

    public void StartNewRun()
    {
        SpawnPlayer();
    }

    public void OnPlayerDeath()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }
}
