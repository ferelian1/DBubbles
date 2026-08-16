using UnityEngine;
using Cinemachine;

/// <summary>
/// Controls the Cinemachine virtual camera target.
///
/// The player bubble is spawned dynamically, so the camera target
/// cannot be assigned from the Inspector at edit time.
///
/// When multiple bubbles exist, the camera follows the bubble
/// that is currently furthest to the right.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Target Update")]
    [SerializeField] private float targetUpdateInterval = 0.1f;

    private float targetTimer;

    private Transform currentTarget;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            Debug.LogError(
                "CameraFollowCinemachine: " +
                "Virtual Camera reference is missing.",
                this
            );

            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        // Update the target periodically instead of every frame.
        // This is enough because bubbles move continuously.
        targetTimer -= Time.deltaTime;

        if (targetTimer > 0f)
            return;

        targetTimer = targetUpdateInterval;

        UpdateTarget();
    }

    /// <summary>
    /// Finds the active bubble that is furthest to the right.
    ///
    /// This becomes important after splitting:
    ///
    /// Large
    ///   ↓
    /// Medium + Medium
    ///   ↓
    /// Small + Small + Small + Small
    ///
    /// The camera follows whichever bubble has progressed the furthest.
    /// </summary>
    private void UpdateTarget()
    {
        Bubble leadingBubble =
            GameManager.Instance.GetLeadingBubble();

        if (leadingBubble == null)
            return;

        Transform newTarget =
            leadingBubble.transform;

        if (currentTarget == newTarget)
            return;

        currentTarget = newTarget;

        virtualCamera.Follow = currentTarget;
    }
}