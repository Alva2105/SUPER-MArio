using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// F12 saves a screenshot. Command line options for testing builds:
//   -level 1-2          start directly on a level
//   -shots <folder>     capture the level at the positions given by -shotx
//   -shotx 10,40,80     player x positions to capture (then the game quits)
public class DevTools : MonoBehaviour
{
    private static string shotFolder;
    private static float[] shotPositions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        var go = new GameObject("Dev Tools");
        DontDestroyOnLoad(go);
        var tools = go.AddComponent<DevTools>();

        string level = GetArg("-level");
        shotFolder = GetArg("-shots");
        string xs = GetArg("-shotx");

        if (xs != null) {
            shotPositions = Array.ConvertAll(xs.Split(','), float.Parse);
        }

        if (level != null) {
            tools.StartCoroutine(tools.LoadLevel(level));
        } else if (shotFolder != null) {
            tools.StartCoroutine(tools.CaptureLevel());
        }
    }

    private static string GetArg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name) {
                return args[i + 1];
            }
        }

        return null;
    }

    private IEnumerator LoadLevel(string level)
    {
        yield return null;

        string[] parts = level.Split('-');

        if (parts.Length == 2) {
            GameManager.Instance.LoadLevel(int.Parse(parts[0]), int.Parse(parts[1]));
        } else {
            SceneManager.LoadScene(level);
        }

        if (shotFolder != null)
        {
            yield return null;
            yield return CaptureLevel();
        }
    }

    private IEnumerator CaptureLevel()
    {
        Directory.CreateDirectory(shotFolder);
        string scene = SceneManager.GetActiveScene().name;

        yield return new WaitForSecondsRealtime(2.5f);
        ScreenCapture.CaptureScreenshot(Path.Combine(shotFolder, $"{scene}_start.png"));
        yield return new WaitForSecondsRealtime(0.5f);

        GameObject player = GameObject.FindWithTag("Player");
        SideScrolling cam = Camera.main != null ? Camera.main.GetComponent<SideScrolling>() : null;

        if (player != null && shotPositions != null)
        {
            foreach (float x in shotPositions)
            {
                Vector3 position = player.transform.position;
                position.x = x;
                player.transform.position = position;
                player.GetComponent<Rigidbody2D>().position = position;

                if (cam != null) {
                    cam.Snap(x);
                }

                yield return new WaitForSecondsRealtime(0.6f);
                ScreenCapture.CaptureScreenshot(Path.Combine(shotFolder, $"{scene}_x{x}.png"));
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) {
            ScreenCapture.CaptureScreenshot($"captura_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        }
    }

}
