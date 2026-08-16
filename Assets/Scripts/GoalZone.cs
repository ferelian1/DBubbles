using UnityEngine;

/// <summary>
/// Trigger area representing the kitchen sink drain.
/// Any bubble reaching it immediately wins the level.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bubble bubble =
            other.GetComponentInParent<Bubble>();

        if (bubble == null)
            return;

        if (GameManager.Instance == null)
            return;

        GameManager.Instance.Victory();
    }
}