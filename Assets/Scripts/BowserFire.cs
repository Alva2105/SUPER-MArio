using UnityEngine;

public class BowserFire : MonoBehaviour
{
    private float direction;
    private float targetY;

    public void Launch(float direction, float targetY)
    {
        this.direction = direction;
        this.targetY = targetY;
        GetComponent<SpriteRenderer>().flipX = direction > 0f;
        Destroy(gameObject, 6f);
    }

    private void Update()
    {
        Vector3 position = transform.position;
        position.x += direction * 6f * Time.deltaTime;
        position.y = Mathf.MoveTowards(position.y, targetY, 2f * Time.deltaTime);
        transform.position = position;
    }
}
