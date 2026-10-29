using UnityEngine;

// Base for enemies that swim: they can't be stomped, only defeated with the star
[RequireComponent(typeof(Rigidbody2D))]
public abstract class WaterEnemy : MonoBehaviour
{
    protected new Rigidbody2D rigidbody;
    protected Transform player;
    protected bool active;
    private bool dead;

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;

        GameObject go = GameObject.FindWithTag("Player");
        player = go != null ? go.transform : null;
    }

    private void OnBecameVisible()
    {
        active = true;
    }

    private void FixedUpdate()
    {
        if (active && !dead) {
            Move(Time.fixedDeltaTime);
        }
    }

    protected abstract void Move(float deltaTime);

    protected void Face(float directionX)
    {
        if (directionX > 0f) {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        } else if (directionX < 0f) {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead || !other.CompareTag("Player")) return;

        Player mario = other.GetComponent<Player>();

        if (mario.starpower) {
            Hit();
        } else {
            mario.Hit();
        }
    }

    public void Hit()
    {
        dead = true;
        AudioManager.Play("kick");
        GameManager.Instance.AddScore(200);

        AnimatedSprite animation = GetComponent<AnimatedSprite>();

        if (animation != null) {
            animation.enabled = false;
        }

        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

}
