using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    public int score { get; private set; }
    public float time { get; private set; }
    public bool timerRunning { get; private set; }

    public HUD hud { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        // allows any level to be played directly from the editor
        if (Instance == null) {
            new GameObject("Game Manager").AddComponent<GameManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            hud = HUD.Create(transform);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        lives = 3;
        coins = 0;
        score = 0;

        if (TryParseLevel(SceneManager.GetActiveScene().name, out int w, out int s))
        {
            world = w;
            stage = s;
            SetupLevel();
        }
        else
        {
            NewGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            TogglePause();
        }

        if (!timerRunning || Time.timeScale == 0f) {
            return;
        }

        // one game second lasts 0.4 real seconds, like the original
        time -= Time.deltaTime * 2.5f;

        if (time <= 0f)
        {
            time = 0f;
            timerRunning = false;

            GameObject player = GameObject.FindWithTag("Player");

            if (player != null) {
                player.GetComponent<Player>().Death();
            }
        }
    }

    private static bool TryParseLevel(string sceneName, out int world, out int stage)
    {
        world = stage = 0;
        string[] parts = sceneName.Split('-');
        return parts.Length == 2 && int.TryParse(parts[0], out world) && int.TryParse(parts[1], out stage);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (TryParseLevel(scene.name, out _, out _)) {
            SetupLevel();
        } else {
            timerRunning = false;
            hud.SetVisible(false);
        }
    }

    private void SetupLevel()
    {
        LevelSettings settings = FindAnyObjectByType<LevelSettings>();

        time = settings != null ? settings.timeLimit : 400;
        timerRunning = true;

        hud.SetVisible(true);
        StopAllCoroutines();
        StartCoroutine(LevelIntro(settings != null ? settings.music : "music_overworld"));
    }

    private IEnumerator LevelIntro(string music)
    {
        AudioManager.StopMusic();
        hud.ShowIntro($"MUNDO {world}-{stage}\n\nMARIO  x  {lives}");
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        hud.HideIntro();
        Time.timeScale = 1f;
        AudioManager.PlayMusic(music);
    }

    public void NewGame()
    {
        lives = 3;
        coins = 0;
        score = 0;

        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        // TODO: show game over screen

        NewGame();
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        Time.timeScale = 1f;

        string sceneName = $"{world}-{stage}";

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Level {sceneName} is not in the build, returning to 1-1");
            this.world = 1;
            this.stage = 1;
            sceneName = "1-1";
        }

        SceneManager.LoadScene(sceneName);
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        timerRunning = false;
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    // converts the remaining time into points at the end of a level
    public IEnumerator CountTimeBonus()
    {
        timerRunning = false;

        while (time > 0f)
        {
            float step = Mathf.Min(time, 2f);
            time -= step;
            score += Mathf.CeilToInt(step) * 50;
            yield return new WaitForSeconds(0.02f);
        }

        time = 0f;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void AddCoin()
    {
        coins++;
        score += 200;

        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }
    }

    public void AddLife()
    {
        lives++;
        AudioManager.Play("1up");
    }

    private void TogglePause()
    {
        if (hud.introVisible) return;

        bool paused = Time.timeScale > 0f;
        Time.timeScale = paused ? 0f : 1f;
        hud.SetPaused(paused);
        AudioManager.Play("select");
    }

}
