using System.Collections;
using UnityEngine;

// Comes out of a pipe and hides again; it stays inside while mario is close
[RequireComponent(typeof(Rigidbody2D))]
public class PiranhaPlant : MonoBehaviour
{
    public float height = 1.5f;
    public float moveTime = 1f;
    public float waitOut = 1.5f;
    public float waitIn = 2f;
    public float safeDistance = 2f;

    private new Rigidbody2D rigidbody;
    private Transform player;
    private Vector2 hidden;
    private bool dead;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        hidden = rigidbody.position;

        GameObject go = GameObject.FindWithTag("Player");
        player = go != null ? go.transform : null;
    }

    private void Start()
    {
        StartCoroutine(Cycle());
    }

    private IEnumerator Cycle()
    {
        yield return new WaitForSeconds(Random.Range(0f, waitIn));

        while (!dead)
        {
            while (PlayerNearby()) {
                yield return new WaitForSeconds(0.25f);
            }

            yield return Move(hidden, hidden + Vector2.up * height);
            yield return new WaitForSeconds(waitOut);
            yield return Move(hidden + Vector2.up * height, hidden);
            yield return new WaitForSeconds(waitIn);
        }
    }

    private bool PlayerNearby()
    {
        return player != null && Mathf.Abs(player.position.x - hidden.x) < safeDistance;
    }

    private IEnumerator Move(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < moveTime && !dead)
        {
            elapsed += Time.deltaTime;
            rigidbody.MovePosition(Vector2.Lerp(from, to, elapsed / moveTime));
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        if (other.CompareTag("Player"))
        {
            Player mario = other.GetComponent<Player>();

            if (mario.starpower) {
                Hit();
            } else {
                mario.Hit();
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Shell"))
        {
            Hit();
        }
    }

    public void Hit()
    {
        dead = true;
        StopAllCoroutines();
        AudioManager.Play("kick");
        GameManager.Instance.AddScore(200);
        Destroy(gameObject);
    }

}
