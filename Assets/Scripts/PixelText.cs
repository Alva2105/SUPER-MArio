using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI text drawn with the 8x8 pixel font in Resources/Fonts/PixelFont
[RequireComponent(typeof(RectTransform))]
public class PixelText : MonoBehaviour
{
    public const string Characters = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-x!¡.,:></?©'$";
    private const int Columns = 16;
    private const int Cell = 8;

    private static Dictionary<char, Sprite> glyphs;

    [SerializeField] private string text = "";
    public float size = 24f;
    public float lineSpacing = 1.5f;
    public Color color = Color.white;
    public TextAnchor alignment = TextAnchor.UpperLeft;

    private readonly List<Image> images = new List<Image>();
    private RectTransform rectTransform;
    private bool dirty = true;

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                dirty = true;
            }
        }
    }

    public Color Color
    {
        get => color;
        set
        {
            color = value;

            foreach (Image image in images) {
                image.color = value;
            }
        }
    }

    public static PixelText Create(Transform parent, string name, float size, TextAnchor alignment)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var label = go.AddComponent<PixelText>();
        label.size = size;
        label.alignment = alignment;
        return label;
    }

    private static void LoadGlyphs()
    {
        glyphs = new Dictionary<char, Sprite>();
        Texture2D texture = Resources.Load<Texture2D>("Fonts/PixelFont");

        if (texture == null) return;

        texture.filterMode = FilterMode.Point;

        for (int i = 0; i < Characters.Length; i++)
        {
            int x = (i % Columns) * Cell;
            int y = texture.height - (i / Columns + 1) * Cell;
            glyphs[Characters[i]] = Sprite.Create(texture, new Rect(x, y, Cell, Cell), new Vector2(0f, 1f), Cell);
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (glyphs == null) {
            LoadGlyphs();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        dirty = true;
    }

    private void LateUpdate()
    {
        if (dirty) {
            Rebuild();
        }
    }

    private static char Normalize(char c)
    {
        // lowercase letters use the uppercase glyphs, except the small "x"
        if (c == 'x') return c;
        return char.ToUpperInvariant(c);
    }

    private void Rebuild()
    {
        dirty = false;

        string[] lines = text.Split('\n');
        float lineHeight = size * lineSpacing;
        float blockHeight = size + lineHeight * (lines.Length - 1);
        Rect rect = rectTransform.rect;

        float top;

        switch (alignment)
        {
            case TextAnchor.MiddleLeft: case TextAnchor.MiddleCenter: case TextAnchor.MiddleRight:
                top = -(rect.height - blockHeight) / 2f;
                break;

            case TextAnchor.LowerLeft: case TextAnchor.LowerCenter: case TextAnchor.LowerRight:
                top = -(rect.height - blockHeight);
                break;

            default:
                top = 0f;
                break;
        }

        int used = 0;

        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row];
            float width = line.Length * size;
            float left;

            switch (alignment)
            {
                case TextAnchor.UpperCenter: case TextAnchor.MiddleCenter: case TextAnchor.LowerCenter:
                    left = (rect.width - width) / 2f;
                    break;

                case TextAnchor.UpperRight: case TextAnchor.MiddleRight: case TextAnchor.LowerRight:
                    left = rect.width - width;
                    break;

                default:
                    left = 0f;
                    break;
            }

            for (int i = 0; i < line.Length; i++)
            {
                if (!glyphs.TryGetValue(Normalize(line[i]), out Sprite sprite) || line[i] == ' ') {
                    continue;
                }

                Image image = GetImage(used++);
                image.sprite = sprite;
                image.color = color;

                RectTransform glyph = image.rectTransform;
                glyph.sizeDelta = new Vector2(size, size);
                glyph.anchoredPosition = new Vector2(Mathf.Round(left + i * size), Mathf.Round(top - row * lineHeight));
            }
        }

        for (int i = used; i < images.Count; i++) {
            images[i].enabled = false;
        }
    }

    private Image GetImage(int index)
    {
        if (index < images.Count)
        {
            images[index].enabled = true;
            return images[index];
        }

        var go = new GameObject("Glyph", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        Image image = go.GetComponent<Image>();
        image.raycastTarget = false;

        RectTransform glyph = image.rectTransform;
        glyph.anchorMin = glyph.anchorMax = new Vector2(0f, 1f);
        glyph.pivot = new Vector2(0f, 1f);

        images.Add(image);
        return image;
    }

}
