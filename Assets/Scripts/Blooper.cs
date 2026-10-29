using UnityEngine;

// Squid that sinks slowly and then pushes itself towards mario
public class Blooper : WaterEnemy
{
    public Sprite sinkSprite;
    public Sprite thrustSprite;
    public float sinkSpeed = 1.5f;
    public float thrustSpeed = 6f;
    public float thrustTime = 0.4f;
    public float minY = 2.5f;
    public float maxY = 10f;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private bool thrusting;
    private float thrustDirection;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 1f;
    }

    protected override void Move(float deltaTime)
    {
        Vector2 position = rigidbody.position;
        timer -= deltaTime;

        if (thrusting)
        {
            position += new Vector2(thrustDirection * 0.8f, 1f).normalized * thrustSpeed * deltaTime;

            if (timer <= 0f || position.y >= maxY)
            {
                thrusting = false;
                timer = Random.Range(0.6f, 1.2f);
                spriteRenderer.sprite = sinkSprite;
            }
        }
        else
        {
            position.y -= sinkSpeed * deltaTime;

            // push again once it is level with mario or near the floor
            bool levelWithPlayer = player != null && position.y <= player.position.y + 0.5f;

            if (timer <= 0f && (levelWithPlayer || position.y <= minY))
            {
                thrusting = true;
                timer = thrustTime;
                thrustDirection = player != null ? Mathf.Sign(player.position.x - position.x) : -1f;
                spriteRenderer.sprite = thrustSprite;
            }
        }

        position.y = Mathf.Clamp(position.y, minY, maxY);
        rigidbody.MovePosition(position);
    }

}
