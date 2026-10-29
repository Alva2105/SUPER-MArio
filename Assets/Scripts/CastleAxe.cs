using System.Collections;
using UnityEngine;

// Touching the axe collapses the bridge and bowser falls into the lava
public class CastleAxe : MonoBehaviour
{
    public Transform bridge;
    public Bowser bowser;

    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;

        used = true;
        StartCoroutine(Victory(other.gameObject));
    }

    private IEnumerator Victory(GameObject mario)
    {
        GameManager.Instance.StopTimer();
        AudioManager.StopMusic();
        mario.GetComponent<PlayerMovement>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        // the bridge breaks piece by piece, from the axe towards bowser
        for (int i = bridge.childCount - 1; i >= 0; i--)
        {
            Destroy(bridge.GetChild(i).gameObject);
            AudioManager.Play("bridge", 0.6f);
            yield return new WaitForSeconds(0.06f);
        }

        if (bowser != null) {
            bowser.Fall();
        }

        yield return new WaitForSeconds(2f);
        AudioManager.Play("castleclear");
        yield return GameManager.Instance.CountTimeBonus();

        GameManager.Instance.hud.ShowIntro("GRACIAS MARIO!\n\n\nTU AVENTURA HA TERMINADO\n\n\nPUNTAJE " + GameManager.Instance.score.ToString("000000"));
        yield return new WaitForSeconds(6f);
        GameManager.Instance.hud.HideIntro();
        GameManager.Instance.NewGame();
    }
}
