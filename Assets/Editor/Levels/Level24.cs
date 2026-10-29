using UnityEditor;
using UnityEngine;

// World 2-4: Bowser's castle. Original layout (not a replica), made with the
// tiles of the original castles: lava, fire bars, podoboos, bridge and axe.
public static class Level24
{
    [MenuItem("Mario/Generar nivel 2-4 (castillo de Bowser)")]
    public static void Build()
    {
        var b = new LevelBuilder("2-4") {
            groundSprite = LevelBuilder.LoadSprite("CastleBrick"),
            hardSprite = LevelBuilder.LoadSprite("CastleBlock"),
        };

        b.settings.theme = LevelSettings.Theme.Castle;
        b.settings.music = "music_castle";
        b.settings.timeLimit = 300;
        b.settings.cameraMinX = -0.5f;
        b.settings.cameraMaxX = 104.5f;

        // Start: a ? block and the first pit
        b.Block(-4, 0, 10, 1);
        b.Block(-4, 12, 106, 12);
        b.Block(-4, 2, -1, 11);
        b.Block(101, 5, 106, 11);
        b.Mario(2f);
        b.Mystery(6, 5, "MagicMushroom");
        Lava(b, 11, 14);
        Podoboo(b, 12.5f, 0f);

        // Tower: stairs up to a plateau guarded by a cross shaped fire bar
        b.Block(15, 0, 22, 1);
        for (int i = 0; i < 3; i++) b.Block(15 + i, 2, 15 + i, 2 + i);
        b.Block(18, 2, 22, 5);
        Firebar(b, 20, 9, 4, 80f, true);

        // Lava lake crossed on a moving platform, with podoboos jumping around it
        Lava(b, 23, 34);
        b.Lift(25.5f, 5.25f, 3f, Vector2.right, 6f, 2.5f, 0f, MovingPlatform.Mode.PingPong);
        Podoboo(b, 27f, 0.3f);
        Podoboo(b, 31f, 1.2f);

        // Tunnel: a long low corridor full of coins with a fire bar inside
        b.Block(35, 0, 44, 1);
        b.Block(37, 6, 44, 11);
        b.Coins(37, 44, 3);
        Firebar(b, 41, 5, 3, -140f);

        // Elevators over the lava: one goes up, the other one goes down
        Lava(b, 45, 52);
        b.Lift(47f, -1f, 3f, Vector2.up, 13f, 2f, 0f, MovingPlatform.Mode.Loop);
        b.Lift(47f, -1f, 3f, Vector2.up, 13f, 2f, 6.5f, MovingPlatform.Mode.Loop);
        b.Lift(51f, 12f, 3f, Vector2.down, 13f, 2f, 3f, MovingPlatform.Mode.Loop);

        // Zigzag: floating ledges between two fire bars, with a hidden extra life
        b.Block(53, 0, 64, 1);
        b.HardBlock(55, 4, 57, 4);
        b.HardBlock(59, 7, 61, 7);
        b.Coins(59, 61, 9);
        Firebar(b, 58, 2, 4, 120f);
        Firebar(b, 63, 9, 4, -120f);
        b.Mystery(56, 8, "1upMushroom");

        // Pillars rising from the lava
        Lava(b, 65, 70);
        b.Block(66, 0, 66, 3);
        b.Block(68, 0, 68, 5);
        b.Block(70, 0, 70, 3);
        Podoboo(b, 67f, 0f);
        Podoboo(b, 69f, 1f);

        // Last corridor before bowser: two fire bars turning in opposite directions
        b.Block(71, 0, 82, 1);
        b.Block(79, 2, 82, 4);
        b.Block(71, 10, 80, 11);
        Firebar(b, 74, 6, 5, 100f);
        Firebar(b, 77, 6, 5, -100f);
        b.Block(96, 0, 106, 4);

        // Bowser over the bridge, the axe on the other side
        Lava(b, 83, 95);
        Transform bridge = b.Create("Bridge", 89f, 4f, "Stage").transform;

        for (int x = 83; x <= 95; x++)
        {
            GameObject piece = b.SpriteObject("Bridge", x, 4f, 5, "Stage");
            piece.transform.SetParent(bridge, true);
            BoxCollider2D collider = piece.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 0.5f);
            collider.offset = new Vector2(0f, 0.25f);
        }

        b.SpriteObject("Chain", 95f, 5f, 4, "Stage");
        Bowser bowser = CreateBowser(b, 90f, 5.5f, 85f, 94f);

        GameObject axe = b.SpriteObject("Axe", 97f, 5f, 5, "Stage");
        BoxCollider2D trigger = axe.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        CastleAxe castleAxe = axe.AddComponent<CastleAxe>();
        castleAxe.bridge = bridge;
        castleAxe.bowser = bowser;

        b.Camera(Color.black);
        b.DeathBarrier(-10f, 120f);
        b.Save();
    }

    private static void Lava(LevelBuilder b, int x0, int x1)
    {
        b.Strip("LavaTop", x0, x1, 1f, 6);
        b.Strip("Lava", x0, x1, 0f, 6);

        GameObject go = b.Create("Lava", (x0 + x1) / 2f, 0.3f, "Stage");
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(x1 - x0 + 1, 1f);
        go.AddComponent<DeathBarrier>();
    }

    private static void Podoboo(LevelBuilder b, float x, float delay)
    {
        GameObject go = b.SpriteObject("Podoboo", x, -0.5f, 7, "Enemies");
        AddHazard(go, 0.35f);
        go.AddComponent<Podoboo>().delay = delay;
    }

    private static void Firebar(LevelBuilder b, int x, int y, int length, float speed, bool cross = false)
    {
        b.HardBlock(x, y, x, y);
        GameObject bar = b.Create("Firebar", x, y, "Enemies");
        bar.AddComponent<Firebar>().speed = speed;

        // a cross shaped bar has fireballs on both sides of the block
        for (int i = cross ? -length : 0; i < length; i++)
        {
            GameObject ball = b.SpriteObject("Firebar", x + i * 0.5f, y, 7, "Enemies");
            ball.transform.SetParent(bar.transform, true);
            AddHazard(ball, 0.2f);
        }
    }

    private static void AddHazard(GameObject go, float radius)
    {
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = radius;
        go.AddComponent<Hazard>();
    }

    private static Bowser CreateBowser(LevelBuilder b, float x, float y, float minX, float maxX)
    {
        GameObject fire = b.SpriteObject("BowserFire1", 0f, -20f, 7, "Enemies");
        fire.name = "Bowser Fire";
        AddHazard(fire, 0.3f);
        fire.AddComponent<BowserFire>();
        fire.SetActive(false);

        GameObject go = b.SpriteObject("Bowser1", x, y, 6, "Enemies");
        go.name = "Bowser";
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.6f, 1.8f);
        go.AddComponent<Hazard>();

        Bowser bowser = go.AddComponent<Bowser>();
        bowser.walk = Level12.Frames("Bowser1", "Bowser2");
        bowser.breath = Level12.Frames("Bowser3", "Bowser4");
        bowser.firePrefab = fire;
        bowser.minX = minX;
        bowser.maxX = maxX;
        return bowser;
    }

}
