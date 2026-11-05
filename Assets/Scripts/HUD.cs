using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static readonly Color CoinColor = new Color(0.99f, 0.6f, 0.22f);

    private PixelText scoreText;
    private PixelText coinIcon;
    private Image coinImage;
    private Sprite[] coinFrames;
    private PixelText coinsText;
    private PixelText worldText;
    private PixelText timeText;
    private PixelText livesText;
    private GameObject topBar;
    private GameObject introPanel;
    private PixelText introText;
    private PixelText introWorld;
    private Image introCharacter;
    private PixelText introLives;
    private GameObject pausePanel;
    private RectTransform popupLayer;
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
        // floating points go below the intro and pause panels
        popupLayer = new GameObject("Popups", typeof(RectTransform)).GetComponent<RectTransform>();
        popupLayer.SetParent(transform, false);
        Stretch(popupLayer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        introPanel = CreatePanel("Intro", Color.black);
        introText = CreateText(introPanel.transform, 0f, 1f, 0f, 1f, 24, TextAnchor.MiddleCenter);

        // level intro of the original: the world above, the character x lives below
        introWorld = CreateText(introPanel.transform, 0f, 1f, 0.5f, 0.5f, 20, TextAnchor.MiddleCenter);
        introWorld.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 65f);
        introWorld.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 85f);

        introCharacter = new GameObject("Character", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        introCharacter.transform.SetParent(introPanel.transform, false);
        introCharacter.preserveAspect = true;
        introCharacter.raycastTarget = false;
        RectTransform character = introCharacter.rectTransform;
        character.anchorMin = character.anchorMax = new Vector2(0.5f, 0.5f);
        character.sizeDelta = new Vector2(40f, 40f);
        character.anchoredPosition = new Vector2(-64f, 0f);

        introLives = CreateText(introPanel.transform, 0.5f, 0.5f, 0.5f, 0.5f, 20, TextAnchor.MiddleLeft);
        RectTransform lives = introLives.GetComponent<RectTransform>();
        lives.offsetMin = new Vector2(-30f, -10f);
        lives.offsetMax = new Vector2(90f, 10f);
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
        CreateCoinIcon();
        coinsText = CreateText(topBar.transform, 0.32f, 0.46f, 0f, 1f, 20, TextAnchor.UpperLeft);
        worldText = CreateText(topBar.transform, 0.47f, 0.62f, 0f, 1f, 20, TextAnchor.UpperLeft);
        timeText = CreateText(topBar.transform, 0.65f, 0.80f, 0f, 1f, 20, TextAnchor.UpperLeft);
        livesText = CreateText(topBar.transform, 0.85f, 0.97f, 0f, 1f, 20, TextAnchor.UpperLeft);
    }

    private void LateUpdate()
    {
        GameManager game = GameManager.Instance;

        if (game == null) return;

        scoreText.Text = $"{game.playerName}\n{game.score:000000}";
        coinsText.Text = $"\nx{game.coins:00}";
        worldText.Text = game.world > 0 ? $"MUNDO\n {game.world}-{game.stage}" : "MUNDO";
        timeText.Text = timeVisible ? $"TIEMPO\n  {Mathf.CeilToInt(game.time):000}" : "TIEMPO";
        livesText.Text = $"VIDAS\n x{game.lives}";

        AnimateCoinIcon();
    }

    // frames of the original HUD coin: bright, then it darkens and comes back
    private static readonly int[] CoinCycle = { 0, 0, 0, 1, 2, 1 };

    // the HUD coin icon from the original sheet (Resources/HUD/CoinIcon: 3 frames of 8x8)
    private void CreateCoinIcon()
    {
        Texture2D texture = Resources.Load<Texture2D>("HUD/CoinIcon");

        if (texture == null) return;

        texture.filterMode = FilterMode.Point;
        int frames = texture.width / texture.height;
        coinFrames = new Sprite[frames];

        for (int i = 0; i < frames; i++) {
            coinFrames[i] = Sprite.Create(texture, new Rect(i * texture.height, 0f, texture.height, texture.height), new Vector2(0.5f, 0.5f), 16f);
        }

        GameObject icon = new GameObject("Coin Icon", typeof(RectTransform), typeof(Image));
        icon.transform.SetParent(topBar.transform, false);

        // same place and size as the "$" glyph it replaces, on the second line
        RectTransform rect = icon.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.30f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(0f, -coinIcon.size * coinIcon.lineSpacing);
        rect.sizeDelta = new Vector2(coinIcon.size, coinIcon.size);

        coinImage = icon.GetComponent<Image>();
        coinImage.sprite = coinFrames[0];
        coinIcon.gameObject.SetActive(false);
    }

    private void AnimateCoinIcon()
    {
        if (coinImage == null) return;

        int step = Mathf.FloorToInt(Time.unscaledTime / 0.13f) % CoinCycle.Length;
        coinImage.sprite = coinFrames[Mathf.Min(CoinCycle[step], coinFrames.Length - 1)];
    }

    public void ShowPoints(string text, Vector3 worldPosition)
    {
        if (!topBar.activeSelf) return;

        ScorePopup.Create(popupLayer, text, worldPosition);
    }

    private void ClearPoints()
    {
        foreach (Transform popup in popupLayer) {
            Destroy(popup.gameObject);
        }
    }

    public void SetVisible(bool visible)
    {
        topBar.SetActive(visible);

        if (!visible)
        {
            ClearPoints();
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
        SetLevelIntroVisible(false);
        introText.Text = text;
        introPanel.SetActive(true);
    }

    // "MUNDO 1-1" and the character's sprite with his lives, like the original
    public void ShowLevelIntro(int world, int stage, Sprite character, int lives)
    {
        introText.Text = "";
        introWorld.Text = $"MUNDO {world}-{stage}";
        introCharacter.sprite = character;
        introLives.Text = $"x  {lives}";
        SetLevelIntroVisible(true);
        introPanel.SetActive(true);
    }

    private void SetLevelIntroVisible(bool visible)
    {
        introWorld.gameObject.SetActive(visible);
        introCharacter.gameObject.SetActive(visible && introCharacter.sprite != null);
        introLives.gameObject.SetActive(visible);
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
