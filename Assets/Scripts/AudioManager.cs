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

    // level music changes to its hurried version when little time is left,
    // and to the star music while mario has the star, like the original
    public static bool Hurry { get; private set; }
    public static bool Star { get; private set; }

    private static readonly Dictionary<string, string> HurriedMusic = new Dictionary<string, string>
    {
        { "music_overworld", "music_hurried_overworld" },
        { "music_underground", "music_hurried_underground" },
        { "music_water", "music_hurried_underwater" },
        { "music_castle", "music_hurried_castle" },
        { "music_star", "music_hurried_star" },
    };

    // clip that is really playing; currentMusic keeps the level music asked for
    private string playingMusic;

    private static string Resolve(string name)
    {
        // only the level music changes, the title and jingles stay the same
        if (name == null || !HurriedMusic.ContainsKey(name)) return name;

        if (Star) name = "music_star";

        return Hurry ? HurriedMusic[name] : name;
    }

    public static void PlayMusic(string name, bool loop = true)
    {
        if (Instance == null) return;

        string clipName = Resolve(name);
        Instance.currentMusic = name;

        if (Instance.playingMusic == clipName && Instance.musicSource.isPlaying) {
            return;
        }

        Instance.playingMusic = clipName;
        Instance.musicSource.clip = Instance.GetClip(clipName);
        Instance.musicSource.loop = loop;
        Instance.musicSource.Play();
    }

    public static void StopMusic()
    {
        if (Instance == null) return;

        Instance.currentMusic = null;
        Instance.playingMusic = null;
        Instance.musicSource.Stop();
    }

    public static void SetHurry(bool hurry)
    {
        Hurry = hurry;
        RefreshMusic();
    }

    public static void SetStar(bool star)
    {
        Star = star;
        RefreshMusic();
    }

    private static void RefreshMusic()
    {
        if (Instance != null && Instance.currentMusic != null) {
            PlayMusic(Instance.currentMusic);
        }
    }

    public static float Length(string name)
    {
        if (Instance == null) return 0f;

        AudioClip clip = Instance.GetClip(name);
        return clip != null ? clip.length : 0f;
    }

    // a few extra sources so effects with their own pitch can overlap
    private readonly AudioSource[] pitchedSources = new AudioSource[3];
    private int nextPitchedSource;

    public static void Play(string name, float volume, float pitch)
    {
        if (Instance == null) return;

        AudioClip clip = Instance.GetClip(name);

        if (clip == null) return;

        int index = Instance.nextPitchedSource;
        Instance.nextPitchedSource = (index + 1) % Instance.pitchedSources.Length;

        if (Instance.pitchedSources[index] == null) {
            Instance.pitchedSources[index] = Instance.gameObject.AddComponent<AudioSource>();
        }

        AudioSource source = Instance.pitchedSources[index];
        source.pitch = pitch;
        source.volume = Instance.sfxSource.volume;
        source.PlayOneShot(clip, volume);
    }

}
