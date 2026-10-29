using UnityEditor;
using UnityEngine;

// Screen shown after losing every life
public static class GameOverScene
{
    [MenuItem("Mario/Generar escena Game Over")]
    public static void Build()
    {
        var b = new LevelBuilder("GameOver");
        Object.DestroyImmediate(b.settings.gameObject);

        Camera camera = b.Camera(Color.black);
        camera.transform.position = new Vector3(0f, 6.5f, -10f);
        Object.DestroyImmediate(camera.GetComponent<SideScrolling>());

        new GameObject("Game Over Screen").AddComponent<GameOverScreen>();

        b.Save();
    }

}
