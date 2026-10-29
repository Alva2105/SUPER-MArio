using UnityEngine;

// Damages mario on touch (fire bars, podoboos, bowser and his fire)
public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            other.GetComponent<Player>().Hit();
        }
    }
}
