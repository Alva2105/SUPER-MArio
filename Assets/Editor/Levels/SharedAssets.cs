using UnityEditor;
using UnityEngine;

// Prefabs and sprite setup shared by the generated levels
public static class SharedAssets
{
    [MenuItem("Mario/Actualizar prefabs")]
    public static void Create()
    {
        SetupMario();

        LevelBuilder.EnemyPrefab<CheepCheep>("CheepCheep",
            Level12.Frames("CheepCheep1", "CheepCheep2"),
            new Vector2(0.8f, 0.6f));

        LevelBuilder.EnemyPrefab<Blooper>("Blooper",
            Level12.Frames("Blooper1"),
            new Vector2(0.8f, 1.2f),
            blooper => {
                blooper.thrustSprite = LevelBuilder.LoadSprite("Blooper1");
                blooper.sinkSprite = LevelBuilder.LoadSprite("Blooper2");
                blooper.maxY = 10.5f;
            });

        LevelBuilder.EnemyPrefab<PiranhaPlant>("PiranhaPlant",
            Level12.Frames("Piranha1", "Piranha2"),
            new Vector2(0.75f, 1.2f),
            plant => {
                // drawn behind the pipe so it hides inside
                plant.GetComponent<SpriteRenderer>().sortingOrder = 3;
                Object.DestroyImmediate(plant.GetComponent<DeathAnimation>());
            });
    }

    // gives mario the original swimming and flag pole frames
    public static void SetupMario()
    {
        const string path = "Assets/Prefabs/Mario.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(path);

        foreach (PlayerSpriteRenderer renderer in root.GetComponentsInChildren<PlayerSpriteRenderer>(true))
        {
            string size = renderer.name == "Big" ? "Big" : "Small";
            renderer.swim = Level12.Frames($"Mario_{size}_Swim1", $"Mario_{size}_Swim2", $"Mario_{size}_Swim3", $"Mario_{size}_Swim4", $"Mario_{size}_Swim5");
            renderer.climb = Level12.Frames($"Mario_{size}_Climb1", $"Mario_{size}_Climb2");
        }

        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
    }

}
