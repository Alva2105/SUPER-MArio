using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Replica of the original screen: black background, the top bar without
// the time and "GAME OVER" in the middle while the jingle plays.
public class GameOverScreen : MonoBehaviour
{
    public float extraDelay = 1.5f;

    private void Start()
    {
        AudioManager.StopMusic();
        AudioManager.Play("gameover");

        GameManager game = GameManager.Instance;

        if (game != null)
        {
            game.hud.SetVisible(true);
            game.hud.SetTimeVisible(false);
        }

        BuildUI();
        StartCoroutine(Sequence());
    }

    private void BuildUI()
    {
        GameObject root = new GameObject("Canvas");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f);
        scaler.matchWidthOrHeight = 0.5f;

        PixelText title = HUD.CreateText(root.transform, 0f, 1f, 0f, 1f, 20, TextAnchor.MiddleCenter);
        // like the original, the name only appears in a two player game
        GameManager game = GameManager.Instance;
        bool twoPlayers = game != null && game.players == 2;
        title.Text = twoPlayers ? $"{game.playerName}\n\nGAME OVER" : "GAME OVER";
    }

    private IEnumerator Sequence()
    {
        yield return new WaitForSeconds(AudioManager.Length("gameover") + extraDelay);

        GameManager game = GameManager.Instance;

        if (game == null) yield break;

        game.hud.SetTimeVisible(true);

        // like holding start on the original, Enter continues on the same world;
        // otherwise the other player takes over or it goes back to the menu
        game.AfterGameOver(Input.GetKey(KeyCode.Return));
    }

}
