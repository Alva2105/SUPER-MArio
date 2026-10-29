using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text scoreText;
    private Text coinsText;
    private Text worldText;
    private Text timeText;
    private Text livesText;
    private GameObject topBar;
    private GameObject introPanel;
    private Text introText;
    private GameObject pausePanel;

    public bool introVisible => introPanel.activeSelf;

    public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

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
        topBar = new GameObject("Top Bar", typeof(RectTransform));
        topBar.transform.SetParent(transform, false);
        Stretch(topBar.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -70f), Vector2.zero);

        scoreText = CreateText(topBar.transform, 0.02f, 0.22f, TextAnchor.UpperLeft);
        coinsText = CreateText(topBar.transform, 0.24f, 0.42f, TextAnchor.UpperCenter);
        worldText = CreateText(topBar.transform, 0.44f, 0.62f, TextAnchor.UpperCenter);
        timeText = CreateText(topBar.transform, 0.64f, 0.80f, TextAnchor.UpperCenter);
        livesText = CreateText(topBar.transform, 0.82f, 0.98f, TextAnchor.UpperRight);

        introPanel = CreatePanel("Intro", Color.black);
        introText = CreateText(introPanel.transform, 0f, 1f, TextAnchor.MiddleCenter, 36);
        Stretch(introText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        introPanel.SetActive(false);

        pausePanel = CreatePanel("Pause", new Color(0f, 0f, 0f, 0.6f));
        Text pauseText = CreateText(pausePanel.transform, 0f, 1f, TextAnchor.MiddleCenter, 48);
        Stretch(pauseText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        pauseText.text = "PAUSA\n<size=20>P / ESC para continuar</size>";
        pausePanel.SetActive(false);

        // the top bar is drawn above the intro panel
        topBar.transform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        GameManager game = GameManager.Instance;

        if (game == null) return;

        scoreText.text = $"MARIO\n{game.score:000000}";
        coinsText.text = $"MONEDAS\n x{game.coins:00}";
        worldText.text = $"MUNDO\n{game.world}-{game.stage}";
        timeText.text = $"TIEMPO\n{Mathf.CeilToInt(game.time):000}";
        livesText.text = $"VIDAS\nx{game.lives}";
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

    public void ShowIntro(string text)
    {
        introText.text = text;
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

    public static Text CreateText(Transform parent, float minX, float maxX, TextAnchor anchor, int size = 24)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(Outline));
        go.transform.SetParent(parent, false);
        Stretch(go.GetComponent<RectTransform>(), new Vector2(minX, 0f), new Vector2(maxX, 1f), new Vector2(0f, 0f), new Vector2(0f, -8f));

        Text text = go.GetComponent<Text>();
        text.font = DefaultFont;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        go.GetComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.8f);
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
