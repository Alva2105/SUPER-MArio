using UnityEngine;

public class Debris : MonoBehaviour
{
    public Vector2 velocity;
    public float gravity = -40f;
    public float spin = 720f;

    public static Debris Spawn(Sprite sprite, Vector3 position, Vector2 velocity)
    {
        GameObject go = new GameObject("Debris");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.5f;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 10;

        Debris debris = go.AddComponent<Debris>();
        debris.velocity = velocity;
        debris.spin *= Mathf.Sign(velocity.x);
        Destroy(go, 2f);
        return debris;
    }

    private void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, -spin * Time.deltaTime);
    }

}
