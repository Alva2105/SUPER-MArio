using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Builds level scenes from code. Coordinates are tile centers: the ground
// occupies rows 0 and 1, so objects standing on it are placed at y = 2.
public class LevelBuilder
{
    public static readonly Color SkyColor = new Color(0.36f, 0.58f, 0.99f);

    private readonly Scene scene;
    private readonly string sceneName;
    private readonly Dictionary<string, Transform> groups = new Dictionary<string, Transform>();

    public LevelSettings settings { get; private set; }
    public Camera camera { get; private set; }
    public GameObject mario { get; private set; }

    public string groundPrefab = "Ground";
    public string brickPrefab = "Brick";
    public Sprite groundSprite;
    public string hardPrefab = "HardBlock";
    public Sprite hardSprite;
    public Sprite coinSprite;

    public LevelBuilder(string sceneName)
    {
        this.sceneName = sceneName;
        scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        settings = new GameObject("Level Settings").AddComponent<LevelSettings>();
    }

    public static GameObject LoadPrefab(string name)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{name}.prefab");
    }

    public static Sprite LoadSprite(string name)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{name}.png");

        if (sprite == null) {
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Extra/{name}.png");
        }

        return sprite;
    }

    public Transform Group(string name)
    {
        if (!groups.TryGetValue(name, out Transform group))
        {
            group = new GameObject(name).transform;
            groups[name] = group;
        }

        return group;
    }

    public GameObject Place(string prefab, float x, float y, string group)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefab(prefab), scene);
        go.transform.SetParent(Group(group), false);
        go.transform.position = new Vector3(x, y, 0f);
        return go;
    }

    public GameObject Create(string name, float x, float y, string group)
    {
        var go = new GameObject(name);
        go.transform.SetParent(Group(group), false);
        go.transform.position = new Vector3(x, y, 0f);
        return go;
    }

    // tiled rectangle of solid blocks, both corners inclusive
    public GameObject Solid(string prefab, int x0, int y0, int x1, int y1, string group = "Ground")
    {
        if (x1 < x0) (x0, x1) = (x1, x0);
        if (y1 < y0) (y0, y1) = (y1, y0);

        Vector2 size = new Vector2(x1 - x0 + 1, y1 - y0 + 1);
        GameObject go = Place(prefab, (x0 + x1) / 2f, (y0 + y1) / 2f, group);
        Resize(go, size);
        return go;
    }

    public GameObject Ground(int x0, int x1, int top = 1)
    {
        return Block(x0, top - 1, x1, top);
    }

    // solid made of the ground tile of the current theme
    public GameObject Block(int x0, int y0, int x1, int y1, string group = "Ground")
    {
        GameObject go = Solid(groundPrefab, x0, y0, x1, y1, group);

        if (groundSprite != null)
        {
            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = groundSprite;
            Record(renderer);
        }

        return go;
    }

    // non solid tiled strip, used for water and lava surfaces
    public GameObject Strip(string sprite, float x0, float x1, float y, int sortingOrder, string group = "Background")
    {
        GameObject go = Create(sprite, (x0 + x1) / 2f, y, group);

        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(sprite);
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = new Vector2(x1 - x0 + 1f, 1f);
        renderer.sortingOrder = sortingOrder;
        return go;
    }

    public GameObject SpriteObject(string sprite, float x, float y, int sortingOrder = -1, string group = "Background")
    {
        GameObject go = Create(sprite, x, y, group);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(sprite);
        renderer.sortingOrder = sortingOrder;
        return go;
    }

    // creates (or replaces) an enemy prefab made only from code
    public static GameObject EnemyPrefab<T>(string name, Sprite[] frames, Vector2 colliderSize, System.Action<T> setup = null) where T : Component
    {
        var go = new GameObject(name);
        go.layer = LayerMask.NameToLayer("Enemy");

        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = frames[0];
        renderer.sortingOrder = 4;

        if (frames.Length > 1)
        {
            var animation = go.AddComponent<AnimatedSprite>();
            animation.sprites = frames;
            animation.framerate = 0.2f;
        }

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = colliderSize;

        var body = go.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;

        var death = go.AddComponent<DeathAnimation>();
        death.spriteRenderer = renderer;
        death.enabled = false;

        T script = go.AddComponent<T>();
        setup?.Invoke(script);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"Assets/Prefabs/{name}.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    public static void Resize(GameObject go, Vector2 size)
    {
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = size;

        var collider = go.GetComponent<BoxCollider2D>();

        if (collider != null) {
            collider.size = size;
        }

        Record(renderer);
        Record(collider);
    }

    public static void Record(Object component)
    {
        if (component != null) {
            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }
    }

    public GameObject Brick(int x, int y, string item = null, int hits = -1)
    {
        GameObject go = Place(brickPrefab, x, y, "Bricks");
        BlockHit block = go.GetComponent<BlockHit>();

        if (block == null)
        {
            block = go.AddComponent<BlockHit>();
            block.emptyBlock = LoadSprite("EmptyBlock");
        }

        block.item = item != null ? LoadPrefab(item) : null;
        block.maxHits = item != null ? hits : -1;
        Record(block);
        return go;
    }

    public void Bricks(int x0, int x1, int y)
    {
        for (int x = x0; x <= x1; x++) {
            Brick(x, y);
        }
    }

    public GameObject Mystery(int x, int y, string item = "BlockCoin")
    {
        GameObject go = Place("MysteryBlock", x, y, "Mystery");
        BlockHit block = go.GetComponent<BlockHit>();
        block.item = LoadPrefab(item);
        block.maxHits = 1;
        Record(block);
        return go;
    }

    public GameObject Coin(int x, int y)
    {
        GameObject go = Place("Coin", x, y, "Coins");

        if (coinSprite != null)
        {
            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = coinSprite;
            Record(renderer);
        }

        return go;
    }

    public void Coins(int x0, int x1, int y)
    {
        for (int x = x0; x <= x1; x++) {
            Coin(x, y);
        }
    }

    public GameObject Enemy(string prefab, float x, float y = 2f)
    {
        return Place(prefab, x, y, "Enemies");
    }

    // vertical pipe whose left column is x and whose top row is "top"
    public Pipe Pipe(int x, int top, int bottom = -1)
    {
        GameObject go = Place("Pipe", x + 0.5f, top, "Pipes");

        Transform body = go.transform.Find("Bottom");
        float height = top - 0.5f - (bottom - 0.5f);

        if (height <= 0f)
        {
            body.gameObject.SetActive(false);
            return go.GetComponent<Pipe>();
        }

        body.localPosition = new Vector3(0f, -0.5f - height / 2f, 0f);
        Resize(body.gameObject, new Vector2(2f, height));
        Record(body);

        return go.GetComponent<Pipe>();
    }

    // empty point where a pipe drops the player, at the top of the exit pipe
    public Transform PipeExit(int x, int top)
    {
        return Create("Exit", x + 0.5f, top, "Pipes").transform;
    }

    public static void Connect(Pipe pipe, Transform destination, Vector3 exitDirection)
    {
        pipe.connection = destination;
        pipe.exitDirection = exitDirection;
        Record(pipe);
    }

    // sideways pipe; the opening starts at the edge x and sits on the floor
    public Pipe SidePipe(float edgeX, int floorTop, float tunnelHeight = -1f)
    {
        GameObject go = Place("PipeConnection", edgeX, floorTop + 1.5f, "Pipes");

        // the vertical part going up can be shorter or removed
        if (tunnelHeight >= 0f)
        {
            Transform tunnel = go.transform.Find("Tunnel");

            if (tunnelHeight == 0f)
            {
                tunnel.gameObject.SetActive(false);
            }
            else
            {
                tunnel.localPosition = new Vector3(3f, 2f + tunnelHeight / 2f, 0f);
                Resize(tunnel.gameObject, new Vector2(2f, tunnelHeight));
                Record(tunnel);
            }
        }

        return go.GetComponent<Pipe>();
    }

    // small sideways pipe made from a single sprite (used under water)
    public Pipe SmallSidePipe(string sprite, int x, float y, Vector2 size)
    {
        GameObject root = Create("Side Pipe", x - 0.5f, y, "Pipes");
        root.layer = LayerMask.NameToLayer("Ignore Raycast");
        var trigger = root.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(0.5f, 0.75f);

        GameObject body = SpriteObject(sprite, x, y, 4, "Pipes");
        body.transform.SetParent(root.transform, true);
        body.AddComponent<BoxCollider2D>().size = size;

        Pipe pipe = root.AddComponent<Pipe>();
        pipe.enterKeyCode = KeyCode.D;
        pipe.enterDirection = Vector3.right;
        return pipe;
    }

    public GameObject Piranha(int pipeX, int pipeTop, Sprite[] frames = null)
    {
        GameObject go = Enemy("PiranhaPlant", pipeX + 0.5f, pipeTop - 0.5f);

        if (frames != null) {
            Reskin(go, frames);
        }

        return go;
    }

    // swaps the sprites of an enemy, for example the blue ones of the underground
    public static void Reskin(GameObject go, Sprite[] frames, Sprite special = null)
    {
        var renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = frames[0];
        Record(renderer);

        var animation = go.GetComponent<AnimatedSprite>();

        if (animation != null)
        {
            animation.sprites = frames;
            Record(animation);
        }

        if (special == null) return;

        var goomba = go.GetComponent<Goomba>();
        var koopa = go.GetComponent<Koopa>();

        if (goomba != null) { goomba.flatSprite = special; Record(goomba); }
        if (koopa != null) { koopa.shellSprite = special; Record(koopa); }
    }

    public class MapData
    {
        public readonly Dictionary<int, Pipe> pipes = new Dictionary<int, Pipe>();
        public readonly List<Vector2> lifts = new List<Vector2>();
        public float flagX = float.NaN;
        public float castleX = float.NaN;
        public int width;
    }

    // Reads a text map (see Assets/Editor/Levels/Maps), one character per tile
    public MapData LoadMap(string file, int offsetX, bool pipes = true)
    {
        string[] lines = File.ReadAllLines($"Assets/Editor/Levels/Maps/{file}")
            .Where(line => !line.StartsWith(";") && line.Length > 0)
            .ToArray();

        int height = lines.Length;
        int width = lines.Max(line => line.Length);
        var map = new MapData { width = width };

        char At(int x, int y)
        {
            if (y < 0 || y >= height) return '.';
            string line = lines[height - 1 - y];
            return x >= 0 && x < line.Length ? line[x] : '.';
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (At(x, y) == 'F') map.flagX = x + offsetX;
                if (At(x, y) == 'K') map.castleX = x + offsetX;
            }
        }

        // solid blocks are merged into rectangles
        var used = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char c = At(x, y);

                if (used[x, y] || (c != '#' && c != 'S' && c != 'H')) continue;

                // the flag pole already has its own base block
                if (c == 'H' && x + offsetX == map.flagX && y == 2) continue;

                int x1 = x;
                while (x1 + 1 < width && At(x1 + 1, y) == c && !used[x1 + 1, y]) x1++;

                int y1 = y;
                while (y1 + 1 < height && Enumerable.Range(x, x1 - x + 1).All(i => At(i, y1 + 1) == c && !used[i, y1 + 1])) y1++;

                for (int i = x; i <= x1; i++) {
                    for (int j = y; j <= y1; j++) used[i, j] = true;
                }

                if (c == '#') {
                    Block(x + offsetX, y, x1 + offsetX, y1);
                } else if (c == 'S') {
                    HardBlock(x + offsetX, y, x1 + offsetX, y1);
                } else {
                    Solid("HardBlock", x + offsetX, y, x1 + offsetX, y1, "Hard");
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int wx = x + offsetX;

                switch (At(x, y))
                {
                    case 'B': Brick(wx, y); break;
                    case '?': Mystery(wx, y); break;
                    case 'o': Coin(wx, y); break;
                    case 'w': SpriteObject("Seaweed", wx, y, 1); break;

                    case 'T':
                        if (pipes) map.pipes[wx] = Pipe(wx, y);
                        break;

                    case 'L':
                        // a lift starts on the first tile of each run and is 3 tiles wide
                        if (At(x - 1, y) != 'L') map.lifts.Add(new Vector2(wx + 1.5f, y + 0.25f));
                        break;
                }
            }
        }

        return map;
    }

    public GameObject HardBlock(int x0, int y0, int x1, int y1)
    {
        GameObject go = Solid(hardPrefab, x0, y0, x1, y1, "Hard");

        if (hardSprite != null)
        {
            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.sprite = hardSprite;
            Record(renderer);
        }

        return go;
    }

    // finds a brick or block placed by the map to change what it contains
    public BlockHit BlockAt(int x, int y)
    {
        foreach (BlockHit block in Object.FindObjectsByType<BlockHit>(FindObjectsSortMode.None))
        {
            Vector3 p = block.transform.position;

            if (Mathf.Approximately(p.x, x) && Mathf.Approximately(p.y, y)) {
                return block;
            }
        }

        return null;
    }

    public void SetItem(int x, int y, string item, int hits = 1)
    {
        BlockHit block = BlockAt(x, y);
        block.item = LoadPrefab(item);
        block.maxHits = hits;
        Record(block);
    }

    // invisible wall that keeps mario inside an area
    public void Wall(float x, float y0, float y1)
    {
        GameObject go = Create("Wall", x, (y0 + y1) / 2f, "Stage");
        go.AddComponent<BoxCollider2D>().size = new Vector2(1f, y1 - y0);
    }


    public GameObject Lift(float x, float y, float width, Vector2 direction, float distance, float speed, float offset, MovingPlatform.Mode mode)
    {
        GameObject go = Create("Lift", x, y, "Lifts");

        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite("Lift");
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = new Vector2(width, 0.5f);
        renderer.sortingOrder = 5;

        go.AddComponent<BoxCollider2D>().size = new Vector2(width, 0.5f);

        var body = go.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;

        var platform = go.AddComponent<MovingPlatform>();
        platform.mode = mode;
        platform.direction = direction;
        platform.distance = distance;
        platform.speed = speed;
        platform.startOffset = offset;
        return go;
    }

    public GameObject Decoration(string prefab, float x, float y = 2f)
    {
        return Place(prefab, x, y, "Background");
    }

    public void FlagAndCastle(int flagX, int castleX, int groundTop, int nextWorld, int nextStage)
    {
        GameObject castle = Place("Castle", castleX, groundTop + 1, "Stage");
        GameObject flag = Place("Flag Pole", flagX, groundTop + 1, "Stage");

        FlagPole pole = flag.GetComponent<FlagPole>();
        pole.castle = castle.transform;
        pole.nextWorld = nextWorld;
        pole.nextStage = nextStage;
        Record(pole);
    }

    public GameObject Mario(float x, float y = 2f)
    {
        mario = (GameObject)PrefabUtility.InstantiatePrefab(LoadPrefab("Mario"), scene);
        mario.transform.position = new Vector3(x, y, 0f);
        return mario;
    }

    public Camera Camera(Color background)
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(8f, 6.5f, -10f);

        camera = go.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 7f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = background;

        go.AddComponent<AudioListener>();
        go.AddComponent<SideScrolling>();
        return camera;
    }

    public void DeathBarrier(float minX, float maxX, float y = -4f)
    {
        GameObject go = Create("Death Barrier", (minX + maxX) / 2f, y, "Stage");
        go.layer = LayerMask.NameToLayer("Ignore Raycast");

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(maxX - minX, 2f);

        go.AddComponent<DeathBarrier>();
    }

    public LevelSettings.CameraZone Zone(string name, float minX, float maxX, Color background, string music, float minY = -10f, float maxY = 30f)
    {
        var zone = new LevelSettings.CameraZone {
            name = name, minX = minX, maxX = maxX, minY = minY, maxY = maxY,
            background = background, music = music,
        };

        var zones = new List<LevelSettings.CameraZone>(settings.zones) { zone };
        settings.zones = zones.ToArray();
        return zone;
    }

    public void Save()
    {
        string path = $"{LevelTools.ScenesFolder}/{sceneName}.unity";
        EditorSceneManager.SaveScene(scene, path);
        LevelTools.UpdateBuildSettings();
        Debug.Log($"Level saved: {path}");
    }

}
