using System.Collections;
using UnityEngine;

// Lava bubble that jumps out of the lava
public class Podoboo : MonoBehaviour
{
    public float height = 7f;
    public float delay = 0f;

    private IEnumerator Start()
    {
        Vector3 start = transform.position;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(delay);

        while (true)
        {
            float duration = 2f, elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = start + Vector3.up * height * 4f * t * (1f - t);
                sprite.flipY = t > 0.5f;
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
}
