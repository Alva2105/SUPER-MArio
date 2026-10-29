using UnityEngine;

// Small bubbles that come out of mario while he swims
public class AirBubbles : MonoBehaviour
{
    public Sprite bubble;
    public float interval = 1.2f;

    private PlayerMovement movement;
    private float timer;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (bubble == null || !movement.enabled || !movement.swimming) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = interval;
            Spawn();
        }
    }

    private void Spawn()
    {
        Vector3 offset = new Vector3(transform.eulerAngles.y > 90f ? -0.3f : 0.3f, 0.4f, 0f);
        GameObject go = new GameObject("Bubble");
        go.transform.position = transform.position + offset;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = bubble;
        renderer.sortingOrder = 6;

        go.AddComponent<Bubble>();
    }

    private class Bubble : MonoBehaviour
    {
        private float elapsed;
        private float startX;

        private void Start()
        {
            startX = transform.position.x;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            Vector3 position = transform.position;
            position.y += 2f * Time.deltaTime;
            position.x = startX + Mathf.Sin(elapsed * 8f) * 0.1f;
            transform.position = position;

            float surface = LevelSettings.Current != null ? LevelSettings.Current.waterSurfaceY + 0.5f : 12f;

            if (position.y > surface || elapsed > 4f) {
                Destroy(gameObject);
            }
        }
    }

}
