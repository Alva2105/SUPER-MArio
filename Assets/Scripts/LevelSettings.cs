using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public enum Theme
    {
        Overworld,
        Underground,
        Water,
        Castle,
    }

    public Theme theme = Theme.Overworld;
    public string music = "music_overworld";
    public int timeLimit = 400;

    [Header("Camera")]
    public float cameraMinX = 0f;
    public float cameraMaxX = 1000f;
    public bool followVertical = false;

    public static LevelSettings Current { get; private set; }

    private void Awake()
    {
        Current = this;
    }

    private void OnDestroy()
    {
        if (Current == this) {
            Current = null;
        }
    }

}
