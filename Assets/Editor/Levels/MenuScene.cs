using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Title screen of the original: board, copyright, 1 / 2 players with the
// mushroom cursor and TOP. Everything is placed in NES pixels (256x240) on a
// world space canvas, 16 pixels per unit like the rest of the sprites.
// Only edits the existing Menu scene, so the background is kept.
public static class MenuScene
{
    private const string ScenePath = "Assets/Scenes/Menu.unity";
    private const float CameraSize = 7.5f;           // 240 px tall, like the NES screen
    private const float CameraY = 7f;                // ground top (y 1.5) lands on pixel 208
    private const float LevelLeft = -0.5f;
    private static readonly Color Pink = new Color32(252, 188, 176, 255);

    [MenuItem("Mario/Actualizar menu de titulo")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "Menu" || root.name == "Titulo") {
                Object.DestroyImmediate(root);
            }
        }

        // the view starts at the left end of the level, like the original
        Camera camera = Camera.main;
        float cameraX = LevelLeft + CameraSize * 16f / 9f;
        camera.orthographicSize = CameraSize;
        camera.transform.position = new Vector3(cameraX, CameraY, -10f);

        SideScrolling scrolling = camera.GetComponent<SideScrolling>();

        if (scrolling != null) {
            Object.DestroyImmediate(scrolling);
        }

        // the widescreen view shows more than the 16 NES tiles, so the floating
        // blocks of the first screen are hidden to keep the title clean
        float cameraRight = cameraX + CameraSize * 16f / 9f + 1f;

        foreach (string group in new[] { "Stage/Blocks/Mystery", "Stage/Blocks/Bricks" })
        {
            GameObject parent = GameObject.Find(group);

            if (parent == null) continue;

            foreach (Transform block in parent.transform)
            {
                if (block.position.x < cameraRight) {
                    block.gameObject.SetActive(false);
                }
            }
        }

        // small mario stands in front of the hill, at pixel 40
        Player mario = Object.FindFirstObjectByType<Player>();

        if (mario != null)
        {
            mario.transform.position = new Vector3(LevelLeft + 3f, 2f, 0f);
            PrefabUtility.RecordPrefabInstancePropertyModifications(mario.transform);
        }

        GameObject title = new GameObject("Titulo", typeof(RectTransform));
        Canvas canvas = title.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 20;

        RectTransform rect = title.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(256f, 240f);
        rect.localScale = Vector3.one / 16f;
        rect.position = new Vector3(cameraX, CameraY, 0f);

        Image("Cartel", title.transform, "TitleBoard", 40, 32, 176, 88);
        Text("Copyright", title.transform, "©2026 VIDEOJUEGOS", 80, 120, Pink);
        RectTransform one = Text("1 Jugador", title.transform, "1 JUGADOR", 88, 144, Color.white);
        RectTransform two = Text("2 Jugadores", title.transform, "2 JUGADORES", 88, 160, Color.white);
        RectTransform cursor = Image("Cursor", title.transform, "TitleCursor", 72, 144, 8, 8);
        RectTransform top = Text("Top", title.transform, "TOP-000000", 96, 184, Color.white);

        MainMenu menu = title.AddComponent<MainMenu>();
        menu.cursor = cursor;
        menu.options = new[] { one, two };
        menu.topText = top.GetComponent<PixelText>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    // x and y are NES pixels from the top left corner of the screen
    private static RectTransform Place(GameObject go, Transform parent, float x, float y, float width, float height)
    {
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, -y);
        return rect;
    }

    private static RectTransform Image(string name, Transform parent, string sprite, float x, float y, float width, float height)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        Image image = go.GetComponent<Image>();
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Extra/{sprite}.png");
        image.raycastTarget = false;
        return Place(go, parent, x, y, width, height);
    }

    private static RectTransform Text(string name, Transform parent, string text, float x, float y, Color color)
    {
        PixelText label = PixelText.Create(parent, name, 8f, TextAnchor.UpperLeft);
        label.Text = text;
        label.color = color;
        return Place(label.gameObject, parent, x, y, text.Length * 8f, 8f);
    }

}
