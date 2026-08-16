using UnityEngine;

public class GameVFX : MonoBehaviour
{
    public static GameVFX Instance { get; private set; }

    [Header("Bubble")]
    [SerializeField] private GameObject bubblePopVFX;
    [SerializeField] private GameObject bubbleHitVFX;

    [Header("Level")]
    [SerializeField] private GameObject levelCompleteVFX;

    [Header("Blow")]
    [Tooltip("Looping particle used while the player is holding the blow input.")]
    [SerializeField] private ParticleSystem blowVFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        
        Instance = this;

        if (blowVFX != null)
        {
            blowVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // --------------------------------------------------
    // Blow VFX
    // --------------------------------------------------

    public void StartBlow(Vector3 position)
    {
        if (blowVFX == null)
            return;

        blowVFX.transform.position = position;

        if (!blowVFX.isPlaying)
        {
            blowVFX.Play();
        }
    }

    public void UpdateBlowPosition(Vector3 position)
    {
        if (blowVFX == null)
            return;

        blowVFX.transform.position = position;
    }

    public void StopBlow()
    {
        if (blowVFX == null)
            return;

        blowVFX.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }

    // --------------------------------------------------
    // One-shot VFX
    // --------------------------------------------------

    public void SpawnBubblePop(Vector3 position)
    {
        SpawnVFX(bubblePopVFX, position);
    }

    public void SpawnBubbleHit(Vector3 position)
    {
        SpawnVFX(bubbleHitVFX, position);
    }

    public void SpawnLevelComplete(Vector3 position)
    {
        SpawnVFX(levelCompleteVFX, position);
    }

    private void SpawnVFX(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        GameObject effect =
            Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

        Destroy(effect, GetVFXLifetime(effect));
    }

    private float GetVFXLifetime(GameObject effect)
    {
        ParticleSystem[] particleSystems =
            effect.GetComponentsInChildren<ParticleSystem>();

        if (particleSystems.Length == 0)
            return 2f;

        float longestLifetime = 0f;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.MainModule main =
                particleSystem.main;

            float duration = main.duration;
            float lifetime = main.startLifetime.constantMax;

            float totalLifetime =
                duration + lifetime;

            if (totalLifetime > longestLifetime)
            {
                longestLifetime = totalLifetime;
            }
        }

        return longestLifetime;
    }
}