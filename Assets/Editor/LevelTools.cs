using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelTools
{
    public const string ScenesFolder = "Assets/Scenes";

    public static readonly string[] SceneOrder = {
        "1-1",
        "1-2",
        "2-2",
        "2-4",
        "GameOver",
    };

    [MenuItem("Mario/Configurar nivel 1-1")]
    public static void SetupLevel11()
    {
        var scene = EditorSceneManager.OpenScene($"{ScenesFolder}/1-1.unity");

        LevelSettings settings = Object.FindAnyObjectByType<LevelSettings>();

        if (settings == null) {
            settings = new GameObject("Level Settings").AddComponent<LevelSettings>();
        }

        Bounds bounds = LevelBounds();
        settings.theme = LevelSettings.Theme.Overworld;
        settings.music = "music_overworld";
        settings.timeLimit = 400;
        settings.cameraMinX = bounds.min.x;
        settings.cameraMaxX = bounds.max.x;

        // the flag of 1-1 leads to the underground level
        FlagPole flag = Object.FindAnyObjectByType<FlagPole>();
        flag.nextWorld = 1;
        flag.nextStage = 2;
        LevelBuilder.Record(flag);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        UpdateBuildSettings();
    }

    // horizontal extent of the solid ground in the open scene
    public static Bounds LevelBounds()
    {
        var colliders = Object.FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None)
            .Where(c => !c.isTrigger && c.gameObject.layer == 0)
            .ToArray();

        Bounds bounds = colliders[0].bounds;

        foreach (var c in colliders) {
            bounds.Encapsulate(c.bounds);
        }

        return bounds;
    }

    [MenuItem("Mario/Generar todos los niveles")]
    public static void BuildAllLevels()
    {
        SetupLevel11();
        Level12.Build();
        Level22.Build();
        GameOverScene.Build();
        UpdateBuildSettings();
    }

    public static void BuildCastleAndPlayer()
    {
        Level24.Build();
        BuildWindows();
    }

    [MenuItem("Mario/Actualizar Build Settings")]
    public static void UpdateBuildSettings()
    {
        EditorBuildSettings.scenes = SceneOrder
            .Select(name => $"{ScenesFolder}/{name}.unity")
            .Where(File.Exists)
            .Select(path => new EditorBuildSettingsScene(path, true))
            .ToArray();
    }

    [MenuItem("Mario/Compilar Windows")]
    public static void BuildWindows()
    {
        UpdateBuildSettings();

        string output = System.Environment.GetEnvironmentVariable("MARIO_BUILD_PATH");

        if (string.IsNullOrEmpty(output)) {
            output = "Builds/Windows/SuperMario.exe";
        }

        var options = new BuildPlayerOptions {
            scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"Build result: {report.summary.result} ({report.summary.totalSize} bytes)");

        if (Application.isBatchMode && report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) {
            EditorApplication.Exit(1);
        }
    }

}
