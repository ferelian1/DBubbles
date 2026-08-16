using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Bubble")]
    [SerializeField] private AudioClip bubblePopClip;
    [SerializeField] private AudioClip bubbleHitClip;
    [SerializeField] private AudioClip bubbleBlowClip;

    [Header("Level")]
    [SerializeField] private AudioClip levelCompleteClip;
    [SerializeField] private AudioClip gameOverClip;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Settings")]
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayBubblePop()
    {
        PlaySFX(bubblePopClip);
    }

    public void PlayBubbleHit()
    {
        PlaySFX(bubbleHitClip);
    }

    public void PlayBubbleBlow()
    {
        PlaySFX(bubbleBlowClip);
    }

    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteClip);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverClip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }
}