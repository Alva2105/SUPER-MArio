using UnityEditor;
using UnityEngine;

// World 2-2: water level, built from the original map
// (Assets/Editor/Levels/Maps/2-2_agua.txt)
public static class Level22
{
    public static readonly Color WaterColor = new Color32(32, 56, 236, 255);

    private const int Intro = -40;
    private const int Exit = 200;

    [MenuItem("Mario/Generar nivel 2-2 (acuatico)")]
    public static void Build()
    {
        SharedAssets.Create();

        var b = new LevelBuilder("2-2");
        b.settings.theme = LevelSettings.Theme.Water;
        b.settings.music = "music_water";
        b.settings.timeLimit = 400;
        b.settings.waterSurfaceY = 11.5f;

        // Entrance, the same as in the original

        b.LoadMap("exterior_entrada.txt", Intro, pipes: false);
        b.Ground(Intro + 14, Intro + 26);
        b.Wall(Intro + 25.5f, 2f, 16f);
        b.Place("Castle", Intro + 2, 2, "Stage");
        Pipe introPipe = b.SidePipe(Intro + 9.5f, 1, 0f);
        b.Pipe(Intro + 12, 5, 5);
        b.Decoration("Cloud1", Intro + 4.5f, 10.5f);
        b.Decoration("Cloud1", Intro + 10f, 6.5f);
        b.Mario(Intro + 5f);
        b.mario.AddComponent<AirBubbles>().bubble = LevelBuilder.LoadSprite("Bubble");
        b.Zone("Entrada", Intro - 0.5f, Intro + 25.5f, LevelBuilder.SkyColor, "music_overworld");

        // Under water

        b.groundSprite = LevelBuilder.LoadSprite("CoralRock");
        b.LoadMap("2-2_agua.txt", 0);
        b.groundSprite = null;

        LevelBuilder.Connect(introPipe, b.Create("Arrival", 2.5f, 11f, "Pipes").transform, Vector3.zero);

        b.Strip("WaterSurface", -0.5f, 191.5f, 12f, 0);
        b.Strip("SkyTile", -0.5f, 191.5f, 13f, 0);
        b.Strip("SkyTile", -0.5f, 191.5f, 14f, 0);

        Pipe exitPipe = b.SmallSidePipe("WaterPipe", 189, 6.5f, new Vector2(1f, 2f));

        Sprite[] grey = Level12.Frames("CheepGrey1", "CheepGrey2");

        Fish(b, 14f, 8f, null);
        Fish(b, 27f, 4f, grey, 0.6f);
        Fish(b, 38f, 7f, null);
        Fish(b, 52f, 9.5f, grey);
        Fish(b, 63f, 3.5f, null, 0.8f);
        Fish(b, 79f, 8f, grey);
        Fish(b, 92f, 5f, null, 0.5f);
        Fish(b, 104f, 9f, grey);
        Fish(b, 117f, 6f, null);
        Fish(b, 131f, 8f, grey, 0.6f);
        Fish(b, 146f, 4f, null);
        Fish(b, 158f, 9f, grey, 0.5f);
        Fish(b, 168f, 6f, null);
        Fish(b, 178f, 8f, grey);

        foreach (float x in new[] { 19f, 44f, 77f, 100f, 128f, 150f, 175f }) {
            b.Enemy("Blooper", x, 9f);
        }

        b.Zone("Agua", -0.5f, 191.5f, WaterColor, "music_water").water = true;

        // Exit to the overworld

        LevelBuilder.MapData exit = b.LoadMap("exterior_salida.txt", Exit);
        LevelBuilder.Connect(exitPipe, b.PipeExit(Exit, 3), Vector3.up);
        b.FlagAndCastle((int)exit.flagX, (int)exit.castleX, 1, 2, 4);
        b.Decoration("Cloud1", Exit + 2.5f, 11.5f);
        b.Decoration("Cloud1", Exit + 22.5f, 11f);
        b.Decoration("Hill1", Exit + 15f);
        b.Decoration("Hill1", Exit + 30f);
        b.Zone("Exterior", Exit - 0.5f, Exit + 31.5f, LevelBuilder.SkyColor, "music_overworld");

        b.settings.cameraMinX = Intro - 0.5f;
        b.settings.cameraMaxX = Exit + 31.5f;

        b.Camera(LevelBuilder.SkyColor);
        b.DeathBarrier(Intro - 10f, Exit + 40f);
        b.Save();
    }

    private static void Fish(LevelBuilder b, float x, float y, Sprite[] frames, float wave = 0f)
    {
        GameObject go = b.Enemy("CheepCheep", x, y);

        if (frames != null) {
            LevelBuilder.Reskin(go, frames);
        }

        CheepCheep fish = go.GetComponent<CheepCheep>();
        fish.waveHeight = wave;
        fish.speed = frames == null ? 1.5f : 1f;
        LevelBuilder.Record(fish);
    }

}
