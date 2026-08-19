using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private float followSpeed = 0.2f;

    [Header("Axis")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;

    private Vector3 initialPosition;
    private float initialTargetX;
    private float initialTargetY;

    private void Start()
    {
        initialPosition = transform.position;

        Bubble leadingBubble =
            GameManager.Instance != null
                ? GameManager.Instance.GetLeadingBubble()
                : null;

        if (leadingBubble != null)
        {
            initialTargetX = leadingBubble.transform.position.x;
            initialTargetY = leadingBubble.transform.position.y;
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance == null)
            return;

        Bubble leadingBubble =
            GameManager.Instance.GetLeadingBubble();

        if (leadingBubble == null)
            return;

        Vector3 targetPosition = initialPosition;

        if (followX)
        {
            float deltaX =
                leadingBubble.transform.position.x - initialTargetX;

            targetPosition.x =
                initialPosition.x + deltaX * followSpeed;
        }

        if (followY)
        {
            float deltaY =
                leadingBubble.transform.position.y - initialTargetY;

            targetPosition.y =
                initialPosition.y + deltaY * followSpeed;
        }

        transform.position = targetPosition;
    }
}