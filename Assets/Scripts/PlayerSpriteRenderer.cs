using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteRenderer : MonoBehaviour
{
    private PlayerMovement movement;
    private Player player;
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite idle;
    public Sprite jump;
    public Sprite slide;
    public AnimatedSprite run;

    [Header("Swimming and flag pole")]
    public Sprite[] swim;
    public Sprite[] climb;
    public float strokeFrameTime = 0.07f;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
        player = GetComponentInParent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // sliding down the flag pole
        if (player.climbing && climb != null && climb.Length > 0)
        {
            run.enabled = false;
            spriteRenderer.sprite = climb[Mathf.FloorToInt(Time.time * 8f) % climb.Length];
            return;
        }

        if (movement.swimming && !movement.grounded && swim != null && swim.Length > 0)
        {
            run.enabled = false;
            spriteRenderer.sprite = SwimFrame();
            return;
        }

        run.enabled = movement.running;

        if (movement.jumping) {
            spriteRenderer.sprite = jump;
        } else if (movement.sliding) {
            spriteRenderer.sprite = slide;
        } else if (!movement.running) {
            spriteRenderer.sprite = idle;
        }
    }

    // each stroke plays the whole kick, then mario drifts with the first frames
    private Sprite SwimFrame()
    {
        int frame = Mathf.FloorToInt((Time.time - movement.lastStroke) / strokeFrameTime);

        if (frame < swim.Length) {
            return swim[frame];
        }

        return swim[Mathf.FloorToInt(Time.time * 3f) % Mathf.Min(2, swim.Length)];
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
        run.enabled = false;
    }

}
