using UnityEditor;
using UnityEngine;

// World 1-2: underground level, built from the original map
// (Assets/Editor/Levels/Maps/1-2_*.txt)
public static class Level12
{
    // horizontal position of each area inside the scene
    private const int Intro = -40;
    private const int Bonus = 400;
    private const int Exit = 200;

    [MenuItem("Mario/Generar nivel 1-2 (subterraneo)")]
    public static void Build()
    {
        SharedAssets.Create();

        var b = new LevelBuilder("1-2");
        b.settings.theme = LevelSettings.Theme.Underground;
        b.settings.music = "music_underground";
        b.settings.timeLimit = 400;

        // Entrance: mario walks from the castle into a sideways pipe

        b.LoadMap("exterior_entrada.txt", Intro, pipes: false);
        b.Ground(Intro + 14, Intro + 26);
        b.Wall(Intro + 25.5f, 2f, 16f);
        b.Place("Castle", Intro + 2, 2, "Stage");
        Pipe introPipe = b.SidePipe(Intro + 9.5f, 1, 0f);
        b.Pipe(Intro + 12, 5, 5);
        b.Decoration("Cloud1", Intro + 4.5f, 10.5f);
        b.Decoration("Cloud1", Intro + 10f, 6.5f);
        b.Mario(Intro + 5f);
        b.Zone("Entrada", Intro - 0.5f, Intro + 25.5f, LevelBuilder.SkyColor, "music_overworld");

        // Underground

        b.groundPrefab = "UndergroundBlock";
        b.brickPrefab = "UndergroundBrick";
        b.hardSprite = LevelBuilder.LoadSprite("UndergroundStair");
        b.coinSprite = LevelBuilder.LoadSprite("Coin_Underground");

        LevelBuilder.MapData main = b.LoadMap("1-2_subterraneo.txt", 0);
        LevelBuilder.Connect(introPipe, b.Create("Arrival", 2.5f, 11.5f, "Pipes").transform, Vector3.zero);

        // hidden items, in the same bricks as the original
        b.SetItem(10, 5, "MagicMushroom");
        b.SetItem(29, 6, "BlockCoin", 10);
        b.SetItem(46, 7, "Starman");
        b.SetItem(84, 7, "MagicMushroom");
        b.SetItem(150, 6, "1upMushroom");

        // first column of lifts goes down, the second one goes up
        const float top = 13.25f, bottom = -2.75f;

        foreach (Vector2 lift in main.lifts)
        {
            bool up = lift.x > 150f;

            // every lift loops from one end of the shaft to the other,
            // starting where the original map shows it
            if (up) {
                b.Lift(lift.x, bottom, 3f, Vector2.up, top - bottom, 2.5f, lift.y - bottom, MovingPlatform.Mode.Loop);
            } else {
                b.Lift(lift.x, top, 3f, Vector2.down, top - bottom, 2.5f, top - lift.y, MovingPlatform.Mode.Loop);
            }
        }

        Sprite[] goomba = Frames("Goomba_Under1", "Goomba_Under2");
        Sprite goombaFlat = LevelBuilder.LoadSprite("Goomba_UnderFlat");
        Sprite[] koopa = Frames("Koopa_Under1", "Koopa_Under2");
        Sprite shell = LevelBuilder.LoadSprite("Koopa_UnderShell");

        foreach (float x in new[] { 16f, 14.5f, 39f, 41f, 60f, 61.5f, 63f, 72f, 76f, 90f, 92f, 96f, 112f, 132f, 148f }) {
            LevelBuilder.Reskin(b.Enemy("Goomba", x), goomba, goombaFlat);
        }

        foreach (float x in new[] { 44f, 45.5f, 150f }) {
            LevelBuilder.Reskin(b.Enemy("Koopa", x), koopa, shell);
        }

        // pipes: the first one leads to the coin room and the third one brings mario back
        Sprite[] piranha = Frames("Piranha_Under1", "Piranha_Under2");
        b.Piranha(109, 5, piranha);

        Pipe bonusEntrance = main.pipes[103];
        LevelBuilder.Connect(bonusEntrance, b.Create("Bonus Arrival", Bonus + 1.5f, 11.5f, "Pipes").transform, Vector3.zero);

        // exit pipe and the warp zone behind the wall
        Pipe exitPipe = b.SidePipe(165.5f, 4);

        foreach (int x in new[] { 178, 182, 186 })
        {
            Pipe warp = main.pipes[x];
            warp.warpWorld = 2;
            warp.warpStage = 2;
            LevelBuilder.Record(warp);
        }

        b.SpriteObject("WarpZoneText", 183f, 8f, 1, "Warp Zone");
        b.SpriteObject("WarpNumber4", 178.5f, 6f, 1, "Warp Zone");
        b.SpriteObject("WarpNumber3", 182.5f, 6f, 1, "Warp Zone");
        b.SpriteObject("WarpNumber2", 186.5f, 6f, 1, "Warp Zone");

        b.Zone("Subterraneo", -0.5f, 191.5f, Color.black, "music_underground");

        // Coin room under the first pipe

        b.LoadMap("1-2_bonus.txt", Bonus);
        Pipe bonusExit = b.SidePipe(Bonus + 11.5f, 1, 11f);
        LevelBuilder.Connect(bonusExit, b.PipeExit(115, 3), Vector3.up);
        b.Zone("Cuarto de monedas", Bonus - 0.5f, Bonus + 15.5f, Color.black, "music_underground");

        // Exit to the overworld

        b.groundPrefab = "Ground";
        b.brickPrefab = "Brick";
        b.hardSprite = null;
        b.coinSprite = null;

        LevelBuilder.MapData exit = b.LoadMap("exterior_salida.txt", Exit);
        LevelBuilder.Connect(exitPipe, b.PipeExit(Exit, 3), Vector3.up);
        b.FlagAndCastle((int)exit.flagX, (int)exit.castleX, 1, 2, 2);
        b.Decoration("Cloud1", Exit + 2.5f, 11.5f);
        b.Decoration("Cloud1", Exit + 22.5f, 11f);
        b.Decoration("Hill1", Exit + 15f);
        b.Decoration("Hill1", Exit + 30f);
        b.Zone("Exterior", Exit - 0.5f, Exit + 31.5f, LevelBuilder.SkyColor, "music_overworld");

        b.settings.cameraMinX = Intro - 0.5f;
        b.settings.cameraMaxX = Bonus + 15.5f;

        b.Camera(LevelBuilder.SkyColor);
        b.DeathBarrier(Intro - 10f, Bonus + 30f);
        b.Save();
    }

    public static Sprite[] Frames(params string[] names)
    {
        return System.Array.ConvertAll(names, LevelBuilder.LoadSprite);
    }

}
