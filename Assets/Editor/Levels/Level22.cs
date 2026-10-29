using UnityEditor;
using UnityEngine;

// World 2-2: water level, replica of the original layout
public static class Level22
{
    public static readonly Color WaterColor = new Color(0.13f, 0.25f, 0.85f);

    [MenuItem("Mario/Generar nivel 2-2 (acuatico)")]
    public static void Build()
    {
        CreatePrefabs();

        var b = new LevelBuilder("2-2") {
            groundPrefab = "Ground",
            groundSprite = LevelBuilder.LoadSprite("Seabed"),
        };

        b.settings.theme = LevelSettings.Theme.Water;
        b.settings.music = "music_water";
        b.settings.timeLimit = 400;
        b.settings.waterSurfaceY = 10.5f;

        // Under water

        b.Ground(-4, 65);
        b.Ground(69, 110);
        b.Ground(115, 140);
        b.Ground(144, 185);
        b.Strip("WaterTop", -4f, 185f, 11f, 7);

        b.Mario(4f, 9f);
        b.mario.AddComponent<AirBubbles>().bubble = LevelBuilder.LoadSprite("Bubble");

        b.Coins(12, 15, 3);
        b.Block(18, 6, 21, 6);
        b.Coins(18, 21, 8);
        b.Block(26, 2, 26, 4);
        b.Block(32, 9, 35, 9);
        b.Coins(32, 35, 7);
        b.Block(44, 2, 45, 6);
        b.Block(50, 5, 56, 5);
        b.Coins(60, 63, 9);

        b.Block(75, 8, 78, 8);
        b.Block(80, 2, 80, 3);
        b.Coins(75, 78, 10);

        // low tunnel with coins inside
        b.Block(86, 7, 95, 8);
        b.Coins(87, 94, 3);

        b.Block(100, 4, 103, 4);
        b.Coins(100, 103, 6);

        b.Block(118, 2, 118, 7);
        b.Block(122, 7, 123, 9);
        b.Block(128, 9, 134, 9);
        b.Coins(128, 134, 7);

        b.Block(146, 2, 150, 3);
        b.Block(152, 2, 156, 5);
        b.Coins(158, 162, 4);
        b.Block(165, 8, 170, 8);
        b.Coins(165, 170, 10);

        // final wall with the exit pipe inside a passage
        b.Block(176, 2, 185, 3);
        b.Block(176, 7, 185, 13);
        b.Block(183, 4, 185, 6);
        Pipe exitPipe = b.SidePipe(178.5f, 3);

        foreach (int x in new[] { 8, 23, 40, 55, 72, 90, 107, 120, 137, 150, 163 }) {
            Coral(b, x, x % 3 == 0 ? 2 : 1);
        }

        Fish(b, 20f, 7f);
        Fish(b, 38f, 4f, 0.6f);
        Fish(b, 52f, 8.5f);
        Fish(b, 70f, 6f, 1f);
        Fish(b, 84f, 9.5f);
        Fish(b, 98f, 5f, 0.5f);
        Fish(b, 106f, 8f);
        Fish(b, 125f, 4f, 0.8f);
        Fish(b, 138f, 7f);
        Fish(b, 150f, 9f, 0.6f);
        Fish(b, 165f, 5f);
        Fish(b, 172f, 9f, 0.5f);

        b.Enemy("Blooper", 46f, 9f);
        b.Enemy("Blooper", 96f, 9.5f);
        b.Enemy("Blooper", 131f, 6f);
        b.Enemy("Blooper", 160f, 9f);

        b.Zone("Agua", -0.5f, 182.5f, WaterColor, "music_water").water = true;

        // Overworld exit

        b.groundSprite = null;
        b.Ground(212, 260);

        b.Pipe(216, 3);
        LevelBuilder.Connect(exitPipe, b.PipeExit(216, 3), Vector3.up);

        for (int i = 0; i < 4; i++) {
            b.Solid("HardBlock", 222 + i, 2, 222 + i, 2 + i);
        }

        b.FlagAndCastle(234, 240, 1, 2, 4);
        b.Decoration("Hill1", 214f);
        b.Decoration("Bush2", 230.5f);
        b.Decoration("Cloud2", 219.5f, 12f);
        b.Decoration("Cloud1", 236f, 11f);

        b.Zone("Exterior", 211.5f, 248.5f, LevelBuilder.SkyColor, "music_overworld");

        b.settings.cameraMinX = -0.5f;
        b.settings.cameraMaxX = 248.5f;

        b.Camera(WaterColor);
        b.DeathBarrier(-10f, 270f);
        b.Save();
    }

    private static void Coral(LevelBuilder b, int x, int height)
    {
        for (int i = 0; i < height; i++) {
            b.SpriteObject("Coral", x, 2 + i, 1);
        }
    }

    private static void Fish(LevelBuilder b, float x, float y, float wave = 0f)
    {
        CheepCheep fish = b.Enemy("CheepCheep", x, y).GetComponent<CheepCheep>();
        fish.waveHeight = wave;
        fish.speed = wave > 0f ? 1.5f : 1f;
        LevelBuilder.Record(fish);
    }

    public static void CreatePrefabs()
    {
        LevelBuilder.EnemyPrefab<CheepCheep>("CheepCheep",
            new[] { LevelBuilder.LoadSprite("CheepCheep1"), LevelBuilder.LoadSprite("CheepCheep2") },
            new Vector2(0.8f, 0.6f));

        LevelBuilder.EnemyPrefab<Blooper>("Blooper",
            new[] { LevelBuilder.LoadSprite("Blooper1") },
            new Vector2(0.8f, 1.2f),
            blooper => {
                blooper.thrustSprite = LevelBuilder.LoadSprite("Blooper1");
                blooper.sinkSprite = LevelBuilder.LoadSprite("Blooper2");
                blooper.maxY = 9.5f;
            });
    }

}
