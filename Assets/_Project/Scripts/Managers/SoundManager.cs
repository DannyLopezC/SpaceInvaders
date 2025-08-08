using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized audio management system for handling sound effects and background music.
/// Implements an object pool for efficient audio playback and supports volume toggling.
/// </summary>
public class SoundManager : Singleton<SoundManager> {
    [Header("Settings")] [SerializeField] private int poolSize = 10; // Number of pooled AudioSources for SFX
    [SerializeField] private AudioSource backgroundMusicSource; // Dedicated AudioSource for background music

    [Header("Volume Controls")] [Range(0f, 1f)] [SerializeField]
    private float soundEffectsVolume = 1f; // Default SFX volume

    private bool _soundEffectsVolumeToggle = true; // Enables/disables SFX volume
    private bool _musicVolumeToggle = true; // Enables/disables music volume
    private bool _soundToggle = true; // Global sound on/off toggle

    [Range(0f, 1f)] [SerializeField] private float backgroundMusicVolume = 1f; // Default background music volume

    // Pool for reusing AudioSources to avoid creating/destroying them at runtime
    private List<AudioSource> audioPool;

    // Tracks active audio clips and their assigned AudioSources
    private Dictionary<AudioClip, AudioSource> activeAudioSources = new Dictionary<AudioClip, AudioSource>();

    // Index to determine the next pooled AudioSource to use
    private int currentIndex = 0;

    /// <summary>
    /// Initializes the sound manager, sets up audio pooling, and plays background music.
    /// </summary>
    private void Awake() {
        base.Awake();
        if (Instance == this) {
            InitializePool();
        }

        // Preload background music into the SoundBank
        SoundBank.Instance.AddAudioClip("BackgroundMusic",
            Resources.Load<AudioClip>("Music/Space Invaders - Space Invaders"));

        // Start playing background music immediately
        PlayBackgroundMusic(SoundBank.Instance.GetAudioClip("BackgroundMusic"));
    }

    /// <summary>
    /// Creates a pool of reusable AudioSources for sound effects.
    /// </summary>
    private void InitializePool() {
        audioPool = new List<AudioSource>();
        for (int i = 0; i < poolSize; i++) {
            GameObject audioObject = new GameObject("PooledAudioSource_" + i);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioObject.transform.parent = transform;
            audioPool.Add(audioSource);
        }
    }

    /// <summary>
    /// Plays a sound effect from the pool using an AudioClip reference.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volume">Optional override for volume.</param>
    /// <param name="loop">Whether the sound should loop.</param>
    private void PlaySoundEffect(AudioClip clip, float? volume = null, bool loop = false) {
        if (clip == null) {
            Debug.LogWarning("AudioClip is null.");
            return;
        }

        AudioSource audioSource = audioPool[currentIndex];
        audioSource.clip = clip;
        audioSource.volume = (_soundEffectsVolumeToggle && _soundToggle) ? (volume ?? soundEffectsVolume) : 0f;
        audioSource.loop = loop;
        audioSource.Play();

        activeAudioSources[clip] = audioSource;

        // Move to the next pooled AudioSource
        currentIndex = (currentIndex + 1) % poolSize;
    }

    /// <summary>
    /// Plays a sound effect by clip ID from the SoundBank.
    /// </summary>
    public void PlaySoundEffect(string clipId, float? volume = null, bool loop = false) {
        PlaySoundEffect(SoundBank.Instance.GetAudioClip(clipId), volume, loop);
    }

    /// <summary>
    /// Stops a sound effect by its SoundBank clip ID.
    /// </summary>
    public void StopSoundEffect(string clipId) {
        StopSoundEffect(SoundBank.Instance.GetAudioClip(clipId));
    }

    /// <summary>
    /// Stops a sound effect by AudioClip reference.
    /// </summary>
    private void StopSoundEffect(AudioClip clip) {
        if (clip == null) return;

        if (activeAudioSources.TryGetValue(clip, out AudioSource audioSource)) {
            audioSource.Stop();
            activeAudioSources.Remove(clip);
        } else {
            Debug.LogWarning($"AudioClip '{clip.name}' is not currently playing.");
        }
    }

    /// <summary>
    /// Plays background music using a dedicated AudioSource.
    /// </summary>
    public void PlayBackgroundMusic(AudioClip clip, float? volume = null, bool loop = true) {
        if (backgroundMusicSource == null) {
            Debug.LogWarning("Background music AudioSource is not assigned.");
            return;
        }

        backgroundMusicSource.clip = clip;
        backgroundMusicSource.volume = (_musicVolumeToggle && _soundToggle) ? (volume ?? backgroundMusicVolume) : 0f;
        backgroundMusicSource.loop = loop;
        backgroundMusicSource.Play();
    }

    /// <summary>
    /// Stops the background music if it is playing.
    /// </summary>
    public void StopBackgroundMusic() {
        if (backgroundMusicSource.isPlaying) {
            backgroundMusicSource.Stop();
        }
    }

    /// <summary>
    /// Clamps and applies the SFX volume.
    /// </summary>
    public void SetSoundEffectsVolume() {
        soundEffectsVolume = Mathf.Clamp01(soundEffectsVolume);
    }

    /// <summary>
    /// Clamps and applies the background music volume.
    /// </summary>
    public void SetBackgroundMusicVolume() {
        backgroundMusicVolume = Mathf.Clamp01(backgroundMusicVolume);
        if (backgroundMusicSource.isPlaying) {
            backgroundMusicSource.volume = (_musicVolumeToggle && _soundToggle) ? backgroundMusicVolume : 0f;
        }
    }

    /// <summary>
    /// Toggles sound effects on/off without affecting global sound toggle.
    /// </summary>
    public void ToggleSfxVolume(bool toggle) {
        _soundEffectsVolumeToggle = toggle;
    }

    /// <summary>
    /// Toggles background music on/off without affecting global sound toggle.
    /// </summary>
    public void ToggleMusicVolume(bool toggle) {
        _musicVolumeToggle = toggle;
        SetBackgroundMusicVolume();
    }

    /// <summary>
    /// Toggles all sounds on/off (both music and SFX).
    /// </summary>
    public void ToggleSoundVolume(bool toggle) {
        _soundToggle = toggle;
        SetBackgroundMusicVolume();
    }
}