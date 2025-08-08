using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    [Header("Settings")] [SerializeField] private int poolSize = 10;
    [SerializeField] private AudioSource backgroundMusicSource;

    [Header("Volume Controls")] [Range(0f, 1f)] [SerializeField]
    private float soundEffectsVolume = 1f;

    private bool _soundEffectsVolumeToggle = true;
    private bool _musicVolumeToggle = true;
    private bool _soundToggle = true;

    [Range(0f, 1f)] [SerializeField] private float backgroundMusicVolume = 1f;

    private List<AudioSource> audioPool;
    private Dictionary<AudioClip, AudioSource> activeAudioSources = new Dictionary<AudioClip, AudioSource>();
    private int currentIndex = 0;

    private void Awake() {
        base.Awake();
        if (Instance == this) {
            InitializePool();
        }

        SoundBank.Instance.AddAudioClip("BackgroundMusic",
            Resources.Load<AudioClip>("Music/Space Invaders - Space Invaders"));
        PlayBackgroundMusic(SoundBank.Instance.GetAudioClip("BackgroundMusic"));
    }

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

        currentIndex = (currentIndex + 1) % poolSize;
    }

    public void PlaySoundEffect(string clipId, float? volume = null, bool loop = false) {
        PlaySoundEffect(SoundBank.Instance.GetAudioClip(clipId), volume, loop);
    }

    public void StopSoundEffect(string clipId) {
        StopSoundEffect(SoundBank.Instance.GetAudioClip(clipId));
    }

    private void StopSoundEffect(AudioClip clip) {
        if (clip == null) return;

        if (activeAudioSources.TryGetValue(clip, out AudioSource audioSource)) {
            audioSource.Stop();
            activeAudioSources.Remove(clip);
        } else {
            Debug.LogWarning($"AudioClip '{clip.name}' is not currently playing.");
        }
    }

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

    public void StopBackgroundMusic() {
        if (backgroundMusicSource.isPlaying) {
            backgroundMusicSource.Stop();
        }
    }

    public void SetSoundEffectsVolume() {
        soundEffectsVolume = Mathf.Clamp01(soundEffectsVolume);
    }

    public void SetBackgroundMusicVolume() {
        backgroundMusicVolume = Mathf.Clamp01(backgroundMusicVolume);
        if (backgroundMusicSource.isPlaying) {
            backgroundMusicSource.volume = (_musicVolumeToggle && _soundToggle) ? backgroundMusicVolume : 0f;
        }
    }

    public void ToggleSfxVolume(bool toggle) {
        _soundEffectsVolumeToggle = toggle;
    }

    public void ToggleMusicVolume(bool toggle) {
        _musicVolumeToggle = toggle;
        SetBackgroundMusicVolume();
    }

    public void ToggleSoundVolume(bool toggle) {
        _soundToggle = toggle;
        SetBackgroundMusicVolume();
    }

    public void PlaySFX() {
        PlaySoundEffect(SoundBank.Instance.GetAudioClip("Reload"));
    }
}