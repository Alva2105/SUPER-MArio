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

    [System.Serializable]
    public class CameraZone
    {
        public string name;
        public float minX;
        public float maxX;
        public float minY = -10f;
        public float maxY = 30f;
        public float cameraY = 6.5f;
        public Color background = Color.black;
        public string music;
        public bool water;

        public bool Contains(Vector2 point)
        {
            return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
        }
    }

    // separate areas of the level, each one with its own camera limits
    public CameraZone[] zones = new CameraZone[0];

    public static LevelSettings Current { get; private set; }

    public float waterSurfaceY = 10.5f;

    public bool IsWater(Vector2 point)
    {
        CameraZone zone = FindZone(point);
        return zone != null && zone.water;
    }

    public CameraZone FindZone(Vector2 point)
    {
        foreach (CameraZone zone in zones)
        {
            if (zone.Contains(point)) {
                return zone;
            }
        }

        return null;
    }

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
