using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Pairs every Mario sprite with its Luigi version (Sprites/Extra/Luigi_*,
// same frames from the Mario & Luigi sheet of The Spriters Resource) and
// stores them on the Mario prefab for the two player game.
public static class LuigiSetup
{
    private const string PrefabPath = "Assets/Prefabs/Mario.prefab";

    [MenuItem("Mario/Configurar Luigi")]
    public static void Build()
    {
        var mario = new List<Sprite>();
        var luigi = new List<Sprite>();

        foreach (string folder in new[] { "Assets/Sprites", "Assets/Sprites/Extra" })
        {
            foreach (string path in Directory.GetFiles(folder, "Mario_*.png"))
            {
                string luigiPath = "Assets/Sprites/Extra/" + Path.GetFileName(path).Replace("Mario_", "Luigi_");
                Sprite a = AssetDatabase.LoadAssetAtPath<Sprite>(path.Replace('\\', '/'));
                Sprite b = AssetDatabase.LoadAssetAtPath<Sprite>(luigiPath);

                if (a == null || b == null)
                {
                    Debug.LogWarning($"Falta el par de {path}");
                    continue;
                }

                mario.Add(a);
                luigi.Add(b);
            }
        }

        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        PlayerCharacter character = root.GetComponent<PlayerCharacter>();

        if (character == null) {
            character = root.AddComponent<PlayerCharacter>();
        }

        character.marioSprites = mario.ToArray();
        character.luigiSprites = luigi.ToArray();

        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        PrefabUtility.UnloadPrefabContents(root);

        Debug.Log($"[Luigi] {mario.Count} sprites emparejados");
    }

}
