using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Sprite flatSprite;

    private bool dead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dead && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) {
                Hit();
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Flatten(player);
            } else {
                player.Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!dead && other.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit();
        }
    }

    private void Flatten(Player player)
    {
        dead = true;
        // the stomp sound is played by the combo, higher on each enemy in a row
        GameManager.Instance.AddStompScore(player, transform.position);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }


    // hit from below through the block it stands on, like the original
    public void Bump()
    {
        if (!dead) {
            Hit();
        }
    }

    private void Hit()
    {
        dead = true;
        AudioManager.Play("kick");
        GameManager.Instance.AddScore(100, transform.position);

        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

}
