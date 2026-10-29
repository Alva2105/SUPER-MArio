using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static readonly Color CoinColor = new Color(0.99f, 0.6f, 0.22f);

    private PixelText scoreText;
    private PixelText coinIcon;
    private PixelText coinsText;
    private PixelText worldText;
    private PixelText timeText;
    private PixelText livesText;
    private GameObject topBar;
    private GameObject introPanel;
    private PixelText introText;
    private GameObject pausePanel;
    private bool timeVisible = true;

    public bool introVisible => introPanel.activeSelf;

    public static HUD Create(Transform parent)
    {
        GameObject root = new GameObject("HUD");
        root.transform.SetParent(parent, false);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f);
        scaler.matchWidthOrHeight = 0.5f;

        return root.AddComponent<HUD>();
    }

    private void Awake()
    {
        introPanel = CreatePanel("Intro", Color.black);
        introText = CreateText(introPanel.transform, 0f, 1f, 0f, 1f, 24, TextAnchor.MiddleCenter);
        introPanel.SetActive(false);

        pausePanel = CreatePanel("Pause", new Color(0f, 0f, 0f, 0.6f));
        PixelText pauseText = CreateText(pausePanel.transform, 0f, 1f, 0f, 1f, 32, TextAnchor.MiddleCenter);
        pauseText.Text = "PAUSA";
        PixelText pauseHint = CreateText(pausePanel.transform, 0f, 1f, 0f, 0.4f, 16, TextAnchor.MiddleCenter);
        pauseHint.Text = "P O ESC PARA CONTINUAR";
        pausePanel.SetActive(false);

        // the top bar is drawn above the intro panel, like on the original
        topBar = new GameObject("Top Bar", typeof(RectTransform));
        topBar.transform.SetParent(transform, false);
        Stretch(topBar.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -76f), new Vector2(0f, -20f));

        scoreText = CreateText(topBar.transform, 0.07f, 0.27f, 0f, 1f, 20, TextAnchor.UpperLeft);
        coinIcon = CreateText(topBar.transform, 0.30f, 0.34f, 0f, 1f, 20, TextAnchor.UpperLeft);
        coinIcon.Text = "\n$";
        coinIcon.Color = CoinColor;
        coinsText = CreateText(topBar.transform, 0.32f, 0.46f, 0f, 1f, 20, TextAnchor.UpperLeft);
        worldText = CreateText(topBar.transform, 0.47f, 0.62f, 0f, 1f, 20, TextAnchor.UpperLeft);
        timeText = CreateText(topBar.transform, 0.65f, 0.80f, 0f, 1f, 20, TextAnchor.UpperLeft);
        livesText = CreateText(topBar.transform, 0.85f, 0.97f, 0f, 1f, 20, TextAnchor.UpperLeft);
    }

    private void LateUpdate()
    {
        GameManager game = GameManager.Instance;

        if (game == null) return;

        scoreText.Text = $"MARIO\n{game.score:000000}";
        coinsText.Text = $"\nx{game.coins:00}";
        worldText.Text = game.world > 0 ? $"MUNDO\n {game.world}-{game.stage}" : "MUNDO";
        timeText.Text = timeVisible ? $"TIEMPO\n  {Mathf.CeilToInt(game.time):000}" : "TIEMPO";
        livesText.Text = $"VIDAS\n x{game.lives}";
    }

    public void SetVisible(bool visible)
    {
        topBar.SetActive(visible);

        if (!visible)
        {
            introPanel.SetActive(false);
            pausePanel.SetActive(false);
        }
    }

    public void SetTimeVisible(bool visible)
    {
        timeVisible = visible;
    }

    public void ShowIntro(string text)
    {
        introText.Text = text;
        introPanel.SetActive(true);
    }

    public void HideIntro()
    {
        introPanel.SetActive(false);
    }

    public void SetPaused(bool paused)
    {
        pausePanel.SetActive(paused);
    }

    private GameObject CreatePanel(string name, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(transform, false);
        Stretch(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    public static PixelText CreateText(Transform parent, float minX, float maxX, float minY, float maxY, float size, TextAnchor anchor)
    {
        PixelText text = PixelText.Create(parent, "Text", size, anchor);
        Stretch(text.GetComponent<RectTransform>(), new Vector2(minX, minY), new Vector2(maxX, maxY), Vector2.zero, Vector2.zero);
        return text;
    }

    public static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

}
