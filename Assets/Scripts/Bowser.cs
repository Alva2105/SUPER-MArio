using UnityEngine;

public class Bowser : MonoBehaviour
{
    public Sprite[] walk;
    public Sprite[] breath;
    public GameObject firePrefab;
    public float minX;
    public float maxX;

    private SpriteRenderer sprite;
    private Transform player;
    private float direction = -1f;
    private float velocityY;
    private float groundY;
    private float fireTimer = 2f;
    private float jumpTimer = 3f;
    public bool falling { get; private set; }

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").transform;
        groundY = transform.position.y;
    }

    private void Update()
    {
        if (falling)
        {
            velocityY -= 30f * Time.deltaTime;
            transform.position += Vector3.up * velocityY * Time.deltaTime;
            return;
        }

        // walks back and forth on the bridge and jumps from time to time
        Vector3 position = transform.position;
        position.x += direction * 1.5f * Time.deltaTime;

        if (position.x < minX || position.x > maxX) {
            direction = -direction;
        }

        jumpTimer -= Time.deltaTime;

        if (jumpTimer <= 0f && position.y <= groundY)
        {
            velocityY = 9f;
            jumpTimer = Random.Range(2f, 4f);
        }

        velocityY -= 25f * Time.deltaTime;
        position.y = Mathf.Max(groundY, position.y + velocityY * Time.deltaTime);
        transform.position = position;

        // always faces mario
        sprite.flipX = player != null && player.position.x > position.x;

        fireTimer -= Time.deltaTime;
        int frame = Mathf.FloorToInt(Time.time * 4f) % 2;
        sprite.sprite = fireTimer < 0.4f ? breath[frame] : walk[frame];

        if (fireTimer <= 0f)
        {
            fireTimer = Random.Range(2f, 3.5f);
            Breathe();
        }
    }

    private void Breathe()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        AudioManager.Play("fireball");
        float side = sprite.flipX ? 1f : -1f;
        GameObject fire = Instantiate(firePrefab, transform.position + new Vector3(side * 1.5f, 0.5f, 0f), Quaternion.identity);
        fire.SetActive(true);
        fire.GetComponent<BowserFire>().Launch(side, player.position.y);
    }

    public void Fall()
    {
        falling = true;
        velocityY = 0f;
        GetComponent<Collider2D>().enabled = false;
        AudioManager.Play("bowser_fall");
        Destroy(gameObject, 3f);
    }
}
