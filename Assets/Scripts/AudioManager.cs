using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public string currentMusic { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null) {
            new GameObject("Audio Manager").AddComponent<AudioManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 0.5f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 0.8f;
    }

    private AudioClip GetClip(string name)
    {
        if (!clips.TryGetValue(name, out AudioClip clip))
        {
            clip = Resources.Load<AudioClip>($"Audio/{name}");
            clips[name] = clip;
        }

        return clip;
    }

    public static void Play(string name, float volume = 1f)
    {
        if (Instance == null) return;

        AudioClip clip = Instance.GetClip(name);

        if (clip != null) {
            Instance.sfxSource.PlayOneShot(clip, volume);
        }
    }

    public static void PlayMusic(string name, bool loop = true)
    {
        if (Instance == null) return;

        if (Instance.currentMusic == name && Instance.musicSource.isPlaying) {
            return;
        }

        Instance.currentMusic = name;
        Instance.musicSource.clip = Instance.GetClip(name);
        Instance.musicSource.loop = loop;
        Instance.musicSource.Play();
    }

    public static void StopMusic()
    {
        if (Instance == null) return;

        Instance.currentMusic = null;
        Instance.musicSource.Stop();
    }

    public static float Length(string name)
    {
        if (Instance == null) return 0f;

        AudioClip clip = Instance.GetClip(name);
        return clip != null ? clip.length : 0f;
    }

}
