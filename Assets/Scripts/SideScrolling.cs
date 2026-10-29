using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;

    public float minX = 0f;
    public float maxX = 1000f;
    public float smoothTime = 0.08f;
    public float lookAhead = 1.5f;

    private float velocityX;
    private LevelSettings.CameraZone zone;

    private bool hasZones => LevelSettings.Current != null && LevelSettings.Current.zones.Length > 0;
    private float halfWidth => camera.orthographicSize * camera.aspect;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        LevelSettings settings = LevelSettings.Current;

        if (settings != null)
        {
            minX = settings.cameraMinX;
            maxX = settings.cameraMaxX;
        }
        else
        {
            minX = transform.position.x;
        }

        UpdateZone(false);
        Snap(player.position.x);
    }

    private void LateUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy) {
            return;
        }

        UpdateZone(true);

        // track the player moving to the right, never scroll back
        float target = Mathf.Max(transform.position.x, player.position.x + lookAhead);
        target = ClampX(target);

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, target, ref velocityX, smoothTime);
        cameraPosition.x = Mathf.Max(cameraPosition.x, transform.position.x);
        transform.position = cameraPosition;
    }

    private float ClampX(float x)
    {
        return Mathf.Clamp(x, minX + halfWidth, Mathf.Max(minX + halfWidth, maxX - halfWidth));
    }

    // switches the camera limits, background and music when the player changes area
    private void UpdateZone(bool playMusic)
    {
        if (!hasZones) return;

        LevelSettings.CameraZone next = LevelSettings.Current.FindZone(player.position);

        if (next == null || next == zone) return;

        zone = next;
        minX = zone.minX;
        maxX = zone.maxX;
        camera.backgroundColor = zone.background;

        Vector3 cameraPosition = transform.position;
        cameraPosition.y = zone.cameraY;
        transform.position = cameraPosition;
        Snap(player.position.x);

        if (playMusic && !string.IsNullOrEmpty(zone.music)) {
            AudioManager.PlayMusic(zone.music);
        }
    }

    // called by pipes after moving the player to another place
    public void OnPlayerTeleported(Vector3 destination)
    {
        if (hasZones)
        {
            zone = null;
            Vector3 position = player.position;
            player.position = destination;
            UpdateZone(true);
            player.position = position;
        }
        else
        {
            Vector3 position = player.position;
            player.position = destination;
            SetUnderground(destination.y < undergroundThreshold);
            player.position = position;
        }
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;

        // jump straight to the player when changing area
        if (player != null)
        {
            cameraPosition.x = Mathf.Max(player.position.x, minX + halfWidth);
            velocityX = 0f;
        }

        transform.position = cameraPosition;

        if (underground) {
            AudioManager.PlayMusic("music_underground");
        } else if (LevelSettings.Current != null) {
            AudioManager.PlayMusic(LevelSettings.Current.music);
        } else {
            AudioManager.PlayMusic("music_overworld");
        }
    }

    public void Snap(float x)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = ClampX(x);
        transform.position = cameraPosition;
        velocityX = 0f;
    }

}
