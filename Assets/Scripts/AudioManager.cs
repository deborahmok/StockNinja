using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip sfxGain;
    public AudioClip sfxLoss;
    public AudioClip sfxBankrupt;
    public AudioClip sfxBonus;
    public AudioClip sfxSwipe;
    public AudioClip sfxLastTen;
    public AudioClip bgmLoop;

    bool lastTenPlayed = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (!musicSource || !bgmLoop) return;
        musicSource.clip = bgmLoop;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySwipe()
    {
        PlaySFX(sfxSwipe);
    }

    public void PlayGain()     => PlaySFX(sfxGain);
    public void PlayLoss()     => PlaySFX(sfxLoss);
    public void PlayBankrupt() => PlaySFX(sfxBankrupt);
    public void PlayBonus()    => PlaySFX(sfxBonus);

    public void PlayLastTenOnce()
    {
        if (lastTenPlayed) return;
        lastTenPlayed = true;
        PlaySFX(sfxLastTen);
    }

    public void ResetLastTen()
    {
        lastTenPlayed = false;
    }

    void PlaySFX(AudioClip clip)
    {
        if (!sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip);
    }
}