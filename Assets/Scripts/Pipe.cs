using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform connection;
    public KeyCode enterKeyCode = KeyCode.S;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.zero;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (connection != null && other.CompareTag("Player"))
        {
            if (!entering && (Input.GetKey(enterKeyCode) || Input.GetKey(AlternateKey()))) {
                StartCoroutine(Enter(other.transform));
            }
        }
    }

    private bool entering;

    // arrow keys work too
    private KeyCode AlternateKey()
    {
        switch (enterKeyCode)
        {
            case KeyCode.S: return KeyCode.DownArrow;
            case KeyCode.D: return KeyCode.RightArrow;
            case KeyCode.A: return KeyCode.LeftArrow;
            case KeyCode.W: return KeyCode.UpArrow;
            default: return enterKeyCode;
        }
    }

    private IEnumerator Enter(Transform player)
    {
        entering = true;

        player.GetComponent<PlayerMovement>().enabled = false;
        AudioManager.Play("pipe");

        Vector3 enteredPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;

        yield return Move(player, enteredPosition, enteredScale);
        yield return new WaitForSeconds(1f);

        var sideSrolling = Camera.main.GetComponent<SideScrolling>();

        if (exitDirection != Vector3.zero)
        {
            player.position = connection.position - exitDirection;
            sideSrolling.OnPlayerTeleported(player.position);
            AudioManager.Play("pipe");
            yield return Move(player, connection.position + exitDirection, Vector3.one);
        }
        else
        {
            player.position = connection.position;
            player.localScale = Vector3.one;
            sideSrolling.OnPlayerTeleported(player.position);
        }

        player.GetComponent<PlayerMovement>().enabled = true;
        entering = false;
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t);
            player.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        player.position = endPosition;
        player.localScale = endScale;
    }

}
