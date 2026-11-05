using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public RectTransform cursor;
    public RectTransform[] options;

    private int selected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            Select(selected + 1);
        } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            Select(selected - 1);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            Confirm();
        }
    }

    private void Select(int index)
    {
        selected = (index + options.Length) % options.Length;
        AudioManager.Play("select");

        // keep the mushroom cursor next to the selected option
        Vector2 position = cursor.anchoredPosition;
        position.y = options[selected].anchoredPosition.y;
        cursor.anchoredPosition = position;
    }

    private void Confirm()
    {
        AudioManager.Play("select");

        if (selected == 0)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

}
