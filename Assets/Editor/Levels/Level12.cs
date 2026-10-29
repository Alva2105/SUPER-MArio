using UnityEditor;
using UnityEngine;

// World 1-2: underground level, replica of the original layout
public static class Level12
{
    [MenuItem("Mario/Generar nivel 1-2 (subterraneo)")]
    public static void Build()
    {
        var b = new LevelBuilder("1-2") {
            groundPrefab = "UndergroundBlock",
            brickPrefab = "UndergroundBrick",
            coinSprite = LevelBuilder.LoadSprite("Coin_Underground"),
        };

        b.settings.theme = LevelSettings.Theme.Underground;
        b.settings.music = "music_underground";
        b.settings.timeLimit = 400;

        // Underground

        b.Ground(-4, 79);
        b.Ground(84, 125);
        b.Ground(130, 132);
        b.Ground(138, 154);

        // left wall and ceiling
        for (int y = 2; y <= 12; y++) b.Brick(0, y);
        b.Bricks(6, 125, 12);
        b.Bricks(138, 150, 12);

        b.Mario(3f);

        b.Mystery(10, 5, "MagicMushroom");
        for (int x = 11; x <= 14; x++) b.Mystery(x, 5);

        b.Enemy("Goomba", 16f);
        b.Enemy("Goomba", 17.5f);

        // steps of increasing height
        b.Solid("UndergroundBlock", 18, 2, 18, 2);
        b.Solid("UndergroundBlock", 20, 2, 20, 3);
        b.Solid("UndergroundBlock", 22, 2, 22, 4);
        b.Solid("UndergroundBlock", 24, 2, 24, 5);
        b.Solid("UndergroundBlock", 26, 2, 26, 5);
        b.Solid("UndergroundBlock", 28, 2, 28, 4);
        b.Solid("UndergroundBlock", 30, 2, 30, 3);
        b.Coin(24, 7);
        b.Coin(26, 7);
        b.Enemy("Goomba", 33f);

        // brick corridor with coins on top
        b.Bricks(38, 45, 5);
        b.Brick(46, 5, "Starman", 1);
        b.Coins(39, 45, 9);
        b.Enemy("Koopa", 44f);
        b.Enemy("Goomba", 48f);

        // closed room of bricks, the coins can be reached by breaking in
        b.Bricks(52, 59, 5);
        b.Bricks(52, 59, 9);
        for (int y = 6; y <= 8; y++) b.Brick(59, y);
        b.Coins(53, 58, 6);
        b.Brick(60, 5, "BlockCoin", 10);

        b.Enemy("Goomba", 62f);
        b.Enemy("Goomba", 63.5f);
        b.Enemy("Goomba", 65f);

        // brick wall and hanging blocks
        for (int x = 68; x <= 71; x++) {
            for (int y = 2; y <= 4; y++) b.Brick(x, y);
        }

        for (int x = 74; x <= 78; x++) {
            for (int y = 8; y <= 11; y++) b.Brick(x, y);
        }

        b.Brick(76, 7, "1upMushroom", 1);
        b.Coins(73, 79, 3);

        // pipes between pits
        b.Pipe(88, 3);
        b.Enemy("Goomba", 92f);
        b.Pipe(95, 4);
        b.Enemy("Goomba", 99f);
        b.Enemy("Goomba", 100.5f);
        b.Pipe(104, 5);
        b.Enemy("Koopa", 109f);
        b.Mystery(112, 6);
        b.Mystery(113, 6, "MagicMushroom");
        b.Mystery(114, 6);

        // final staircase before the lifts
        for (int i = 0; i < 4; i++) {
            b.Solid("UndergroundBlock", 118 + i, 2, 118 + i, 2 + i);
        }

        b.Solid("UndergroundBlock", 122, 2, 125, 5);

        // lifts over the pits: one going up, one going down
        for (int i = 0; i < 2; i++)
        {
            b.Lift(128f, -1f, 3f, Vector2.up, 15f, 2.5f, i * 7.5f, MovingPlatform.Mode.Loop);
            b.Lift(135f, 14f, 3f, Vector2.down, 15f, 2.5f, i * 7.5f, MovingPlatform.Mode.Loop);
        }

        b.Coins(130, 132, 5);

        // exit pipe and end wall
        Pipe exitPipe = b.SidePipe(150.5f, 1);
        b.Solid("UndergroundBlock", 155, 2, 158, 12);
        b.Enemy("Goomba", 145f);

        b.Zone("Subterraneo", -0.5f, 154.5f, Color.black, "music_underground");

        // Overworld exit

        b.groundPrefab = "Ground";
        b.coinSprite = null;

        b.Ground(196, 245);

        Pipe pipe = b.Pipe(200, 3);
        LevelBuilder.Connect(exitPipe, b.PipeExit(200, 3), Vector3.up);

        for (int i = 0; i < 8; i++) {
            b.Solid("HardBlock", 205 + i, 2, 205 + i, 2 + i);
        }

        b.Solid("HardBlock", 213, 2, 213, 9);
        b.FlagAndCastle(222, 228, 1, 2, 2);

        b.Decoration("Hill2", 198f);
        b.Decoration("Bush1", 218f);
        b.Decoration("Hill1", 236f);
        b.Decoration("Cloud1", 203f, 11f);
        b.Decoration("Cloud2", 219.5f, 12f);
        b.Decoration("Cloud3", 233f, 11f);

        b.Zone("Exterior", 195.5f, 236.5f, LevelBuilder.SkyColor, "music_overworld");

        b.settings.cameraMinX = -0.5f;
        b.settings.cameraMaxX = 236.5f;

        b.Camera(Color.black);
        b.DeathBarrier(-10f, 260f);
        b.Save();
    }

}
