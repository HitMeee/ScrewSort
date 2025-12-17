using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Screw Sounds")]
    [SerializeField] private AudioClip screwLiftSound;
    [SerializeField] private AudioClip screwDropSound;
    [SerializeField] private AudioClip screwMoveSound;
    [SerializeField] private AudioClip screwPlaceSound;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip levelCompleteSound;

    [Header("Completion Sounds")]
    [SerializeField] private AudioClip boltCompleteSound; // Âm thanh khi hoàn thành bolt (5 same-color screws)

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeAudioSources()
    {
        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 0)
            {
                sfxSource = sources[0];
            }
            else
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        Debug.Log("🔊 SoundManager initialized for SFX only!");
        Debug.Log($"🔊 SFX Source: {(sfxSource != null ? "✅" : "❌")}");
    }

    [ContextMenu("🔄 Refresh Audio Sources")]
    public void RefreshAudioSources()
    {
        InitializeAudioSources();
        Debug.Log("🔄 Audio Sources refreshed!");
    }

    [ContextMenu("🎵 Test Button Click")]
    public void TestButtonClick()
    {
        PlayButtonClick();
    }

    [ContextMenu("🎵 Test Bolt Complete")]
    public void TestBoltComplete()
    {
        PlayBoltComplete();
    }

    public void UpdateVolumes()
    {
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // SCREW SOUNDS
    public void PlayScrewLift()
    {
        PlaySFX(screwLiftSound, "Screw Lift");
    }

    public void PlayScrewDrop()
    {
        PlaySFX(screwDropSound, "Screw Drop");
    }

    public void PlayScrewMove()
    {
        PlaySFX(screwMoveSound, "Screw Move");
    }

    public void PlayScrewPlace()
    {
        PlaySFX(screwPlaceSound, "Screw Place");
    }

    // UI SOUNDS
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound, "Button Click");
    }

    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteSound, "Level Complete");
    }

    // BOLT COMPLETION SOUND (only for full 5-screw completion)
    public void PlayBoltComplete()
    {
        PlaySFX(boltCompleteSound, "Bolt Complete", 0.9f);
    }

    // CORE SFX METHOD
    private void PlaySFX(AudioClip clip, string soundName, float volumeMultiplier = 1f)
    {
        if (sfxSource != null && clip != null)
        {
            float finalVolume = Mathf.Clamp01(sfxVolume * volumeMultiplier);
            sfxSource.PlayOneShot(clip, finalVolume);
            Debug.Log($"🔊 Playing: {soundName} (Volume: {finalVolume:F2})");
        }
        else if (sfxSource == null)
        {
            Debug.LogWarning("⚠️ SFX AudioSource is null!");
        }
        else if (clip == null)
        {
            Debug.LogWarning($"⚠️ {soundName} clip is null!");
        }
    }

    public void PlaySFXWithVolume(AudioClip clip, float volume)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }

    // VOLUME CONTROL
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // UTILITIES
    public bool HasSFXSource() => sfxSource != null;
    public bool IsSoundManagerReady() => sfxSource != null;
}