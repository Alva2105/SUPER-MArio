using UnityEngine;

// Fish that swims slowly to the left, optionally going up and down
public class CheepCheep : WaterEnemy
{
    public float speed = 1.2f;
    public float waveHeight = 0f;
    public float waveSpeed = 2f;

    private float baseY;
    private float elapsed;

    protected override void Awake()
    {
        base.Awake();
        baseY = transform.position.y;
    }

    protected override void Move(float deltaTime)
    {
        elapsed += deltaTime;

        Vector2 position = rigidbody.position;
        position.x -= speed * deltaTime;
        position.y = baseY + Mathf.Sin(elapsed * waveSpeed) * waveHeight;
        rigidbody.MovePosition(position);
        Face(-1f);

        // gone far behind the camera
        if (Camera.main != null && position.x < Camera.main.transform.position.x - 20f) {
            Destroy(gameObject);
        }
    }

}
