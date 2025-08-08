using System.Collections.Generic;
using UnityEngine;

public interface ISoundBank {
    AudioClip GetAudioClip(string key);

    void AddAudioClip(string key, AudioClip clip);
    void ClearDictionary();
}

[CreateAssetMenu(fileName = "SoundBank", menuName = "Game/SoundBank")]
public class SoundBank : ScriptableObject, ISoundBank {
    private static ISoundBank _instance;
    public static ISoundBank Instance => _instance ??= Resources.Load<SoundBank>("Music/SoundBank");

    [SerializeField] private Dictionary<string, AudioClip> _audioClips = new();

    public AudioClip GetAudioClip(string key) {
        if (_audioClips.TryGetValue(key, out AudioClip clip)) {
            return clip;
        }

        Debug.LogWarning($"AudioClip with key {key} not found in SoundBank.");
        return null;
    }

    public void AddAudioClip(string key, AudioClip clip) {
        _audioClips.Add(key, clip);
    }

    public void ClearDictionary() {
        _audioClips.Clear();
    }
}