using UnityEngine;

// Row of fireballs spinning around a block
public class Firebar : MonoBehaviour
{
    public float speed = 90f;

    private void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
