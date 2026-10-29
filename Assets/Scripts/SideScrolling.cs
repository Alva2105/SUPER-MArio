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
    }

    private void LateUpdate()
    {
        if (player == null || !player.gameObject.activeInHierarchy) {
            return;
        }

        // track the player moving to the right, never scroll back
        float halfWidth = camera.orthographicSize * camera.aspect;
        float target = Mathf.Max(transform.position.x, player.position.x + lookAhead);
        target = Mathf.Clamp(target, minX + halfWidth, Mathf.Max(minX + halfWidth, maxX - halfWidth));

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, target, ref velocityX, smoothTime);
        cameraPosition.x = Mathf.Max(cameraPosition.x, transform.position.x);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;

        // jump straight to the player when changing area
        if (player != null)
        {
            float halfWidth = camera.orthographicSize * camera.aspect;
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
        float halfWidth = camera.orthographicSize * camera.aspect;
        cameraPosition.x = Mathf.Clamp(x, minX + halfWidth, Mathf.Max(minX + halfWidth, maxX - halfWidth));
        transform.position = cameraPosition;
        velocityX = 0f;
    }

}
