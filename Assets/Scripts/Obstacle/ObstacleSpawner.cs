using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObstacle
    {
        [Tooltip("Full obstacle pattern prefab. Its vertical positions and gap are already designed inside the prefab.")]
        public GameObject prefab;

        [Min(0f)]
        public float weight = 1f;
    }

    [System.Serializable]
    public class SpawnablePowerUp
    {
        public GameObject prefab;

        [Min(0f)]
        public float weight = 1f;
    }

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Obstacle Patterns")]
    [SerializeField]
    private List<SpawnableObstacle> obstaclePatterns =
        new List<SpawnableObstacle>();

    [Header("Power Ups")]
    [SerializeField] private bool spawnPowerUps = true;

    [Range(0f, 1f)]
    [SerializeField] private float powerUpChance = 0.15f;

    [SerializeField] private float powerUpYOffset = 0f;

    [SerializeField]
    private List<SpawnablePowerUp> powerUpPrefabs =
        new List<SpawnablePowerUp>();

    [Header("Endless Spawn")]
    [Tooltip("Constant horizontal speed of the bubble.")]
    [SerializeField] private float endlessHorizontalSpeed = 5f;

    [Tooltip("Base time between obstacle patterns.")]
    [SerializeField] private float secondsBetweenPatterns = 2f;

    [Tooltip("Minimum time between patterns as difficulty increases.")]
    [SerializeField] private float minimumSecondsBetweenPatterns = 1.4f;

    [Tooltip("How far ahead of the camera obstacles should be spawned.")]
    [SerializeField] private float spawnAheadDistance = 20f;

    [Tooltip("How far behind the camera an obstacle must be before it is destroyed.")]
    [SerializeField] private float destroyBehindDistance = 15f;

    [Header("Difficulty")]
    [Tooltip("Distance traveled before spawn timing starts getting faster.")]
    [SerializeField] private float difficultyStartDistance = 100f;

    private float nextSpawnX;
    private float startX;
    private bool initialized;

    private readonly List<GameObject> spawnedObjects =
        new List<GameObject>();

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
        }
    }

    private void Start()
    {
        Initialize();
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

        SpawnAhead();
        CleanupBehind();
    }

    private void Initialize()
    {
        float cameraRightEdge = GetCameraRightEdge();

        startX = cameraRightEdge;

        nextSpawnX =
            cameraRightEdge + spawnAheadDistance;

        initialized = true;
    }

    private void SpawnAhead()
    {
        float cameraRightEdge =
            GetCameraRightEdge();

        float requiredSpawnX =
            cameraRightEdge + spawnAheadDistance;

        while (nextSpawnX < requiredSpawnX)
        {
            SpawnObstacle(nextSpawnX);
            Debug.Log($"Spawned obstacle at x={nextSpawnX}");
            nextSpawnX +=
                GetCurrentSpacing();
        }
    }

    private void SpawnObstacle(float xPosition)
    {
        GameObject prefab =
            GetRandomObstacle();

        if (prefab == null)
            return;

        GameObject obstacle =
            Instantiate(
                prefab,
                new Vector3(xPosition, 0f, 0f),
                Quaternion.identity,
                gameObject.transform
            );

        spawnedObjects.Add(obstacle);

        TrySpawnPowerUp(xPosition);
    }

    private void TrySpawnPowerUp(float obstacleX)
    {
        if (!spawnPowerUps)
            return;

        if (powerUpPrefabs == null ||
            powerUpPrefabs.Count == 0)
        {
            return;
        }

        if (Random.value > powerUpChance)
            return;

        GameObject prefab =
            GetRandomPowerUp();

        if (prefab == null)
            return;

        float yPosition =
            Random.Range(-1f, 2f);

        GameObject powerUp =
            Instantiate(
                prefab,
                new Vector3(
                    obstacleX,
                    yPosition + powerUpYOffset,
                    0f
                ),
                Quaternion.identity
            );

        spawnedObjects.Add(powerUp);
    }

    private float GetCurrentSpacing()
    {
        float distanceTravelled =
            GetLeadingBubbleDistance();

        float difficulty =
            Mathf.InverseLerp(
                0f,
                difficultyStartDistance,
                distanceTravelled
            );

        float currentSeconds =
            Mathf.Lerp(
                secondsBetweenPatterns,
                minimumSecondsBetweenPatterns,
                difficulty
            );

        return endlessHorizontalSpeed *
               currentSeconds;
    }

    private float GetLeadingBubbleDistance()
    {
        if (GameManager.Instance == null)
            return 0f;

        Bubble leadingBubble =
            GameManager.Instance.GetLeadingBubble();

        if (leadingBubble == null)
            return 0f;

        return Mathf.Max(
            0f,
            leadingBubble.transform.position.x - startX
        );
    }

    private void CleanupBehind()
    {
        float cameraLeftEdge =
            GetCameraLeftEdge();

        float destroyX =
            cameraLeftEdge - destroyBehindDistance;

        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            GameObject spawnedObject =
                spawnedObjects[i];

            if (spawnedObject == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            if (spawnedObject.transform.position.x < destroyX)
            {
                Destroy(spawnedObject);
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    private float GetCameraRightEdge()
    {
        float halfWidth =
            targetCamera.orthographicSize *
            targetCamera.aspect;

        return targetCamera.transform.position.x +
               halfWidth;
    }

    private float GetCameraLeftEdge()
    {
        float halfWidth =
            targetCamera.orthographicSize *
            targetCamera.aspect;

        return targetCamera.transform.position.x -
               halfWidth;
    }

    private GameObject GetRandomObstacle()
    {
        if (obstaclePatterns == null ||
            obstaclePatterns.Count == 0)
        {
            Debug.LogWarning(
                "ObstacleSpawner: No obstacle patterns assigned."
            );

            return null;
        }

        float totalWeight = 0f;

        foreach (SpawnableObstacle obstacle in obstaclePatterns)
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
            return null;

        float randomValue =
            Random.Range(0f, totalWeight);

        foreach (SpawnableObstacle obstacle in obstaclePatterns)
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

    private GameObject GetRandomPowerUp()
    {
        float totalWeight = 0f;

        foreach (SpawnablePowerUp powerUp in powerUpPrefabs)
        {
            if (powerUp == null)
                continue;

            if (powerUp.prefab == null)
                continue;

            if (powerUp.weight <= 0f)
                continue;

            totalWeight += powerUp.weight;
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue =
            Random.Range(0f, totalWeight);

        foreach (SpawnablePowerUp powerUp in powerUpPrefabs)
        {
            if (powerUp == null)
                continue;

            if (powerUp.prefab == null)
                continue;

            if (powerUp.weight <= 0f)
                continue;

            randomValue -= powerUp.weight;

            if (randomValue <= 0f)
            {
                return powerUp.prefab;
            }
        }

        return null;
    }
}