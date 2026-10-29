using System.Collections;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    public GameObject item;
    public Sprite emptyBlock;
    public int maxHits = -1;
    private bool animating;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.DotTest(transform, Vector2.up)) {
                Hit(collision.gameObject.GetComponent<Player>());
            }
        }
    }

    private void Hit(Player player)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true; // show if hidden

        // plain bricks break when mario is big
        if (item == null && maxHits < 0 && player != null && player.big)
        {
            Break(spriteRenderer.sprite);
            return;
        }

        maxHits--;

        if (maxHits == 0) {
            spriteRenderer.sprite = emptyBlock;
        }

        if (item != null)
        {
            Instantiate(item, transform.position, Quaternion.identity);

            if (item.GetComponent<BlockCoin>() == null) {
                AudioManager.Play("powerup_appear");
            }
        }
        else
        {
            AudioManager.Play("bump");
        }

        StartCoroutine(Animate());
    }

    private void Break(Sprite sprite)
    {
        AudioManager.Play("break");
        GameManager.Instance.AddScore(50);

        Vector2[] velocities = {
            new Vector2(-3f, 12f), new Vector2(3f, 12f),
            new Vector2(-3f, 8f), new Vector2(3f, 8f),
        };

        foreach (Vector2 velocity in velocities) {
            Debris.Spawn(sprite, transform.position, velocity);
        }

        Destroy(gameObject);
    }

    private IEnumerator Animate()
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;

        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

}
