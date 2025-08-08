using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for a sound bank that stores and retrieves audio clips by string key.
/// </summary>
public interface ISoundBank {
    /// <summary>
    /// Retrieves an AudioClip associated with the given key.
    /// </summary>
    /// <param name="key">The unique string identifier for the clip.</param>
    /// <returns>The AudioClip if found; otherwise, null.</returns>
    AudioClip GetAudioClip(string key);

    /// <summary>
    /// Adds a new AudioClip to the sound bank.
    /// </summary>
    /// <param name="key">The unique string identifier for the clip.</param>
    /// <param name="clip">The AudioClip to store.</param>
    void AddAudioClip(string key, AudioClip clip);

    /// <summary>
    /// Removes all audio clips from the sound bank.
    /// </summary>
    void ClearDictionary();
}

/// <summary>
/// A ScriptableObject that stores a dictionary of audio clips,
/// allowing global access to game sounds through a singleton instance.
/// </summary>
/// <remarks>
/// The SoundBank is loaded from Resources/Music/SoundBank at runtime.
/// Audio clips are stored in a dictionary with string keys for quick access.
/// </remarks>
[CreateAssetMenu(fileName = "SoundBank", menuName = "Game/SoundBank")]
public class SoundBank : ScriptableObject, ISoundBank {
    /// <summary>
    /// Singleton instance of the sound bank, loaded from the Resources folder.
    /// </summary>
    private static ISoundBank _instance;

    public static ISoundBank Instance => _instance ??= Resources.Load<SoundBank>("Music/SoundBank");

    /// <summary>
    /// Dictionary mapping string keys to audio clips.
    /// </summary>
    [SerializeField] private Dictionary<string, AudioClip> _audioClips = new();

    /// <inheritdoc/>
    public AudioClip GetAudioClip(string key) {
        if (_audioClips.TryGetValue(key, out AudioClip clip)) {
            return clip;
        }

        Debug.LogWarning($"AudioClip with key '{key}' not found in SoundBank.");
        return null;
    }

    /// <inheritdoc/>
    public void AddAudioClip(string key, AudioClip clip) {
        _audioClips.Add(key, clip);
    }

    /// <inheritdoc/>
    public void ClearDictionary() {
        _audioClips.Clear();
    }
}