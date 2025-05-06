using UnityEngine;
using System.Collections.Generic;

public enum SoundNames
{
    Pick,
    Correct,
    Wrong,
    Error,
    Win
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class Sound
    {
        public SoundNames name;
        public AudioClip clip;
    }

    public List<Sound> sounds;

    private Dictionary<SoundNames, AudioClip> soundDict;
    private AudioSource[] audioSources;
    private const int audioSourceCount = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Setup AudioSources
        audioSources = new AudioSource[audioSourceCount];
        for (int i = 0; i < audioSourceCount; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
        }

        // Map sounds to dictionary
        soundDict = new Dictionary<SoundNames, AudioClip>();
        foreach (var sound in sounds)
        {
            soundDict[sound.name] = sound.clip;
        }
    }

    public static void Play(SoundNames soundName, float volume = 1f, float pitch = 1f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("SoundManager is not initialized.");
            return;
        }

        if (!Instance.soundDict.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"Sound '{soundName}' not found.");
            return;
        }

        // Find a free AudioSource
        foreach (var source in Instance.audioSources)
        {
            if (!source.isPlaying)
            {
                source.pitch = pitch;
                source.PlayOneShot(clip, volume);
                return;
            }
        }

        // No free sources
        Debug.Log("All audio sources are busy, sound skipped.");
    }

}
