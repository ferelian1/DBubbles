using UnityEngine;

/// <summary>
/// Creates a random vertical gap between a top and bottom hazard.
///
/// The prefab itself is spawned by ObstacleSpawner.
/// This component only decides where the two hazards should sit.
/// </summary>
public class ObstaclePair : MonoBehaviour
{
    [Header("Obstacle References")]
    [SerializeField] private Transform topObstacle;
    [SerializeField] private Transform bottomObstacle;

    [Header("Gap")]
    [SerializeField] private float gapSize = 3f;

    [Header("Gap Position")]
    [SerializeField] private float minimumGapCenter = -1f;
    [SerializeField] private float maximumGapCenter = 2f;

    private void Start()
    {
        GenerateGap();
    }

    /// <summary>
    /// Randomizes the center position of the playable gap.
    /// </summary>
    private void GenerateGap()
    {
        if (topObstacle == null ||
            bottomObstacle == null)
        {
            Debug.LogError(
                "ObstaclePair: " +
                "Top or bottom obstacle is missing.",
                this
            );

            return;
        }

        float gapCenter =
            Random.Range(
                minimumGapCenter,
                maximumGapCenter
            );

        float halfGap = gapSize * 0.5f;
        // Set the top obstacle's position.
        Vector3 topPosition = topObstacle.localPosition;
        // The top obstacle's position is the center of the gap
        // plus half the gap size.
        topPosition.y = gapCenter + halfGap;
        // The bottom obstacle's position is the center of the gap 
        // minus half the gap size.
        topObstacle.localPosition = topPosition;
        // Set the bottom obstacle's position.
        Vector3 bottomPosition = bottomObstacle.localPosition;
        // minus half the gap size.
        bottomPosition.y = gapCenter - halfGap;
        // Set the bottom obstacle's position.
        bottomObstacle.localPosition = bottomPosition;
    }
}