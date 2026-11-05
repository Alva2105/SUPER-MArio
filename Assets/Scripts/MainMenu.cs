using UnityEngine;

// Title screen like the original: option 1 plays with Mario and option 2
// with Luigi, TOP shows the best score. Esc closes the game.
public class MainMenu : MonoBehaviour
{
    public RectTransform cursor;
    public RectTransform[] options;
    public PixelText topText;

    private int selected;

    private void Start()
    {
        int top = GameManager.Instance != null ? GameManager.Instance.topScore : 0;

        if (topText != null) {
            topText.Text = $"TOP-{top:000000}";
        }

        Select(0, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            Select(selected + 1, true);
        } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            Select(selected - 1, true);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            Confirm();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            Quit();
        }
    }

    private void Select(int index, bool playSound)
    {
        selected = (index + options.Length) % options.Length;

        if (playSound) {
            AudioManager.Play("select");
        }

        // the cursor stays to the left of the selected option
        Vector2 position = cursor.anchoredPosition;
        position.y = options[selected].anchoredPosition.y;
        cursor.anchoredPosition = position;
    }

    private void Confirm()
    {
        AudioManager.Play("select");
        GameManager.Instance.NewGame(selected);
    }

    private void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
