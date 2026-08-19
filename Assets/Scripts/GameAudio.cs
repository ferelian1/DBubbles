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
    [SerializeField] private AudioClip starCollectClip;

    [Header("UI")]
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource blowAudioSource;

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
        if (bubbleBlowClip == null || blowAudioSource == null)
            return;

        if (!blowAudioSource.isPlaying)
        {
            blowAudioSource.clip = bubbleBlowClip;
            blowAudioSource.loop = true;
            blowAudioSource.Play();
        }
    }

    public void StopBubbleBlow()
    {
        if (blowAudioSource == null)
            return;

        if (blowAudioSource.isPlaying)
        {
            blowAudioSource.Stop();
        }
    }
    public void PlayStarCollect()
    {
        PlaySFX(starCollectClip);
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