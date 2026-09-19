using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Renderer renderer;

        [Range(0f, 1f)]
        public float speed = 0.05f;
    }

    [SerializeField] private Camera targetCamera;

    [SerializeField]
    private ParallaxLayer[] layers;

    private float startCameraX;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        startCameraX = targetCamera.transform.position.x;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        float cameraDistance =
            targetCamera.transform.position.x - startCameraX;

        // Keep the background container following the camera.
        transform.position = new Vector3(
            targetCamera.transform.position.x,
            transform.position.y,
            transform.position.z
        );

        foreach (ParallaxLayer layer in layers)
        {
            if (layer == null || layer.renderer == null)
                continue;

            float offset =
                cameraDistance * layer.speed;

            offset = Mathf.Repeat(offset, 1f);

            layer.renderer.material.SetTextureOffset(
                "_MainTex",
                new Vector2(offset, 0f)
            );
        }
    }
}