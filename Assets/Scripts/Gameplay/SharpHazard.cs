using UnityEngine;

/// <summary>
/// Any sharp obstacle that splits or pops bubbles.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SharpHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bubble bubble =
            other.GetComponentInParent<Bubble>();

        if (bubble == null)
            return;
        GameAudio.Instance.PlayBubbleHit();
        bubble.HitHazard();
    }

    // Also support normal collision in case the collider
    // is accidentally configured as a non-trigger.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bubble bubble =
            collision.collider.GetComponentInParent<Bubble>();

        if (bubble == null)
            return;

        bubble.HitHazard();
    }
}