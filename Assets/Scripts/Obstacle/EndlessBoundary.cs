using UnityEngine;

public class EndlessBoundary : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [Header("Segments")]
    [SerializeField] private Transform[] topSegments;
    [SerializeField] private Transform[] bottomSegments;

    [Header("Settings")]
    [SerializeField] private float segmentWidth = 20f;
    [SerializeField] private float recycleBehindDistance = 15f;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        if (targetCamera == null)
            return;

        RecycleSegments(topSegments);
        RecycleSegments(bottomSegments);
    }

    private void RecycleSegments(Transform[] segments)
    {
        if (segments == null)
            return;

        float cameraLeft =
            targetCamera.transform.position.x;

        foreach (Transform segment in segments)
        {
            if (segment == null)
                continue;

            if (segment.position.x <
                cameraLeft - recycleBehindDistance)
            {
                segment.position +=
                    Vector3.right *
                    (segmentWidth * segments.Length);
            }
        }
    }
}