using UnityEngine;
using DG.Tweening;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float minMoveDuration = 1f;
    [SerializeField] private float maxMoveDuration = 3f;

    private void Start()
    {
        float duration =
            Random.Range(minMoveDuration, maxMoveDuration);

        float targetY =
            transform.position.y + moveDistance;

        transform.DOMoveY(targetY, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}