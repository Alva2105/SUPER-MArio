using System.Collections;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    public Transform flag;
    public Transform poleBottom;
    public Transform castle;
    public float speed = 6f;
    public int nextWorld = 1;
    public int nextStage = 1;

    private bool reached;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!reached && other.CompareTag("Player"))
        {
            reached = true;

            // higher grabs are worth more points
            float height = Mathf.Clamp01((other.transform.position.y - poleBottom.position.y) / 8f);
            GameManager.Instance.AddScore(Mathf.RoundToInt(Mathf.Lerp(100f, 5000f, height) / 100f) * 100);
            GameManager.Instance.StopTimer();

            AudioManager.StopMusic();
            AudioManager.Play("flagpole");

            StartCoroutine(MoveTo(flag, poleBottom.position));
            StartCoroutine(LevelCompleteSequence(other.transform));
        }
    }

    private IEnumerator LevelCompleteSequence(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;

        yield return MoveTo(player, poleBottom.position);

        AudioManager.Play("stageclear");

        yield return MoveTo(player, player.position + Vector3.right);
        yield return MoveTo(player, player.position + Vector3.right + Vector3.down);
        yield return MoveTo(player, castle.position);

        player.gameObject.SetActive(false);

        yield return GameManager.Instance.CountTimeBonus();
        yield return new WaitForSeconds(Mathf.Max(1f, AudioManager.Length("stageclear") - 2f));

        GameManager.Instance.LoadLevel(nextWorld, nextStage);
    }

    private IEnumerator MoveTo(Transform subject, Vector3 position)
    {
        while (Vector3.Distance(subject.position, position) > 0.125f)
        {
            subject.position = Vector3.MoveTowards(subject.position, position, speed * Time.deltaTime);
            yield return null;
        }

        subject.position = position;
    }

}
