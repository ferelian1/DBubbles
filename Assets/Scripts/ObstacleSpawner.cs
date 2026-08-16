using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamically spawns obstacle patterns ahead of the camera.
///
/// IMPORTANT:
/// This spawner does NOT depend on a Player or Bubble reference.
/// The playable bubbles are spawned dynamically and can split into
/// multiple objects, so the camera is the better reference point.
///
/// Flow:
///
/// Main Camera moves
///       ↓
/// Spawner checks right edge of camera
///       ↓
/// If not enough obstacles exist ahead
///       ↓
/// Spawn another obstacle
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObstacle
    {
        [Tooltip("Obstacle prefab that will be spawned.")]
        public GameObject prefab;

        [Tooltip("Higher value = higher spawn chance.")]
        [Min(0f)]
        public float weight = 1f;
    }

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Obstacle Prefabs")]
    [SerializeField]
    private List<SpawnableObstacle> obstaclePrefabs =
        new List<SpawnableObstacle>();

    [Header("Spawn Distance")]
    [Tooltip(
        "How far ahead of the camera the spawner should maintain obstacles."
    )]
    [SerializeField] private float spawnAheadDistance = 15f;

    [Tooltip(
        "Minimum horizontal distance between two obstacle patterns."
    )]
    [SerializeField] private float obstacleSpacing = 6f;

    [Header("Vertical Spawn Range")]
    [SerializeField] private float minimumY = -1f;
    [SerializeField] private float maximumY = 2f;

    [Header("Level End")]
    [Tooltip(
        "Optional sink transform. " +
        "No obstacles will be spawned beyond this X position."
    )]
    [SerializeField] private Transform sink;

    [Tooltip(
        "How far before the sink the final obstacle can be spawned."
    )]
    [SerializeField] private float finalObstacleDistance = 10f;

    private float nextSpawnX;

    private bool initialized;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError(
                "ObstacleSpawner: No Camera found."
            );

            enabled = false;
            return;
        }
    }

    private void Start()
    {
        InitializeSpawnPosition();
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState !=
            GameManager.GameState.Playing)
        {
            return;
        }

        SpawnAheadOfCamera();
    }

    /// <summary>
    /// Initial spawn position is based on the camera's current
    /// right edge.
    ///
    /// This is important because the Bubble doesn't exist until
    /// GameManager.StartGame() creates it.
    /// </summary>
    private void InitializeSpawnPosition()
    {
        float cameraRightEdge =
            GetCameraRightEdge();

        nextSpawnX =
            cameraRightEdge + spawnAheadDistance;

        initialized = true;
    }

    /// <summary>
    /// Keeps generating obstacles until there are enough obstacles
    /// ahead of the visible camera area.
    /// </summary>
    private void SpawnAheadOfCamera()
    {
        float cameraRightEdge =
            GetCameraRightEdge();

        float requiredSpawnX =
            cameraRightEdge + spawnAheadDistance;

        while (nextSpawnX < requiredSpawnX)
        {
            // Don't create obstacles after the sink.
            if (IsBeyondLevelEnd(nextSpawnX))
            {
                return;
            }

            SpawnObstacle(nextSpawnX);

            nextSpawnX += obstacleSpacing;
        }
    }

    /// <summary>
    /// Returns the world-space X coordinate of the camera's
    /// right edge.
    /// </summary>
    private float GetCameraRightEdge()
    {
        float halfWidth =
            targetCamera.orthographicSize *
            targetCamera.aspect;

        return targetCamera.transform.position.x +
               halfWidth;
    }

    private bool IsBeyondLevelEnd(float spawnX)
    {
        if (sink == null)
            return false;

        float finalSpawnX =
            sink.position.x - finalObstacleDistance;

        return spawnX > finalSpawnX;
    }

    /// <summary>
    /// Creates one random obstacle pattern.
    /// </summary>
    private void SpawnObstacle(float xPosition)
    {
        GameObject prefab =
            GetRandomObstaclePrefab();

        if (prefab == null)
            return;

        float yPosition =
            Random.Range(
                minimumY,
                maximumY
            );

        Vector3 spawnPosition =
            new Vector3(
                xPosition,
                yPosition,
                0f
            );

        Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    /// <summary>
    /// Weighted random selection.
    ///
    /// Example:
    ///
    /// Fork       weight 5
    /// Knife      weight 3
    /// Fan        weight 2
    ///
    /// Fork will appear more frequently.
    /// </summary>
    private GameObject GetRandomObstaclePrefab()
    {
        if (obstaclePrefabs == null ||
            obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning(
                "ObstacleSpawner: " +
                "No obstacle prefabs assigned."
            );

            return null;
        }

        float totalWeight = 0f;

        foreach (SpawnableObstacle obstacle
                 in obstaclePrefabs)
        {
            if (obstacle == null)
                continue;

            if (obstacle.prefab == null)
                continue;

            if (obstacle.weight <= 0f)
                continue;

            totalWeight += obstacle.weight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning(
                "ObstacleSpawner: " +
                "All obstacle weights are zero."
            );

            return null;
        }

        float randomValue =
            Random.Range(
                0f,
                totalWeight
            );

        foreach (SpawnableObstacle obstacle
                 in obstaclePrefabs)
        {
            if (obstacle == null)
                continue;

            if (obstacle.prefab == null)
                continue;

            if (obstacle.weight <= 0f)
                continue;

            randomValue -= obstacle.weight;

            if (randomValue <= 0f)
            {
                return obstacle.prefab;
            }
        }

        return null;
    }
}