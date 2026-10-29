using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public enum Mode
    {
        Loop,
        PingPong,
    }

    public Mode mode = Mode.Loop;
    public Vector2 direction = Vector2.up;
    public float speed = 2f;
    public float distance = 15f;
    public float startOffset = 0f;

    private new Rigidbody2D rigidbody;
    private Vector2 origin;
    private float traveled;
    private PlayerMovement rider;

    public Vector2 velocity { get; private set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        origin = rigidbody.position;

        // spread several platforms along the same path
        traveled = Mathf.Repeat(startOffset, distance);
        rigidbody.position = origin + direction.normalized * traveled;
    }

    private void FixedUpdate()
    {
        Vector2 step = direction.normalized * speed * Time.fixedDeltaTime;
        traveled += step.magnitude;

        if (traveled >= distance)
        {
            traveled = 0f;

            if (mode == Mode.Loop)
            {
                // wrap back to the start of the path
                rigidbody.position = origin;
                velocity = Vector2.zero;
                DropRider();
                return;
            }

            origin = rigidbody.position;
            direction = -direction;
        }

        velocity = step / Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + step);

        if (rider != null) {
            rider.platformVelocity = velocity;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.DotTest(transform, Vector2.down)) {
            rider = collision.gameObject.GetComponent<PlayerMovement>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (rider != null && collision.gameObject == rider.gameObject) {
            DropRider();
        }
    }

    private void DropRider()
    {
        if (rider != null)
        {
            rider.platformVelocity = Vector2.zero;
            rider = null;
        }
    }

}
