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
    public int players { get; private set; } = 1;

    // two player game: mario (0) and luigi (1) take turns like the original,
    // each one keeps his own lives, coins, score and level
    public int currentPlayer { get; private set; }
    public string playerName => currentPlayer == 0 ? "MARIO" : "LUIGI";

    private class PlayerState
    {
        public int world = 1;
        public int stage = 1;
        public int lives = 3;
        public int coins;
        public int score;
    }

    private readonly PlayerState[] states = { new PlayerState(), new PlayerState() };

    // best score, shown as TOP on the title screen
    public int topScore { get; private set; }
    private const string TopScoreKey = "TopScore";

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
        topScore = PlayerPrefs.GetInt(TopScoreKey, 0);

        lives = 3;
        coins = 0;
        score = 0;

        string scene = SceneManager.GetActiveScene().name;

        if (TryParseLevel(scene, out int w, out int s))
        {
            world = w;
            stage = s;
            SetupLevel();
        }
        else if (scene == "Menu")
        {
            SetupMenu();
        }
        else if (scene != "GameOver")
        {
            NewGame();
        }
    }

    private void Update()
    {
        // pause only exists inside a level
        if (inLevel && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))) {
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
            timeUp = true;

            // mario's children also use the Player tag, so search the component
            Player player = FindFirstObjectByType<Player>();

            if (player != null) {
                player.Death();
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
        } else if (scene.name == "Menu") {
            SetupMenu();
        } else {
            LeaveLevel();
            hud.SetVisible(false);
        }
    }

    private bool inLevel;

    private void SaveTopScore()
    {
        if (score <= topScore) return;

        topScore = score;
        PlayerPrefs.SetInt(TopScoreKey, topScore);
        PlayerPrefs.Save();
    }

    private void LeaveLevel()
    {
        SaveTopScore();
        StopAllCoroutines();
        Time.timeScale = 1f;
        timerRunning = false;
        inLevel = false;
        hud.SetPaused(false);
    }

    // title screen: top bar without the time, like the original
    private void SetupMenu()
    {
        LeaveLevel();
        world = 1;
        stage = 1;
        hud.SetVisible(true);
        hud.SetTimeVisible(false);
        AudioManager.PlayMusic("music_title");
    }

    public void BackToMenu()
    {
        // the title screen always shows mario
        currentPlayer = 0;

        if (Application.CanStreamedLevelBeLoaded("Menu")) {
            SceneManager.LoadScene("Menu");
        } else {
            NewGame();
        }
    }

    private void SetupLevel()
    {
        inLevel = true;
        timeUp = false;
        LevelSettings settings = FindAnyObjectByType<LevelSettings>();

        time = settings != null ? settings.timeLimit : 400;
        timerRunning = true;

        hud.SetVisible(true);
        hud.SetTimeVisible(true);
        StopAllCoroutines();
        StartCoroutine(LevelIntro(settings != null ? settings.music : "music_overworld"));
    }

    private IEnumerator LevelIntro(string music)
    {
        AudioManager.StopMusic();
        hud.ShowIntro($"MUNDO {world}-{stage}\n\n{playerName}  x  {lives}");
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        hud.HideIntro();
        Time.timeScale = 1f;
        AudioManager.PlayMusic(music);
    }

    public void NewGame()
    {
        NewGame(1);
    }

    public void NewGame(int players)
    {
        SaveTopScore();
        this.players = Mathf.Clamp(players, 1, 2);

        for (int i = 0; i < states.Length; i++) {
            states[i] = new PlayerState();
        }

        LoadPlayer(0);
        LoadLevel(1, 1);
    }

    private void SavePlayer()
    {
        PlayerState state = states[currentPlayer];
        state.world = world;
        state.stage = stage;
        state.lives = lives;
        state.coins = coins;
        state.score = score;
    }

    private void LoadPlayer(int player)
    {
        currentPlayer = player;
        PlayerState state = states[player];
        world = state.world;
        stage = state.stage;
        lives = state.lives;
        coins = state.coins;
        score = state.score;
    }

    private int OtherPlayer => 1 - currentPlayer;

    private bool OtherPlayerAlive => players == 2 && states[OtherPlayer].lives > 0;

    // after the game over screen: the other player goes on if he has lives,
    // holding Enter continues with the same player, otherwise back to the menu
    public void AfterGameOver(bool continueGame)
    {
        if (continueGame)
        {
            ContinueGame();
        }
        else if (OtherPlayerAlive)
        {
            LoadPlayer(OtherPlayer);
            LoadLevel(world, stage);
        }
        else
        {
            BackToMenu();
        }
    }

    public void GameOver()
    {
        SaveTopScore();
        timerRunning = false;
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded("GameOver")) {
            SceneManager.LoadScene("GameOver");
        } else {
            NewGame();
        }
    }

    // starts again on the level where the game was lost
    public void ContinueGame()
    {
        lives = 3;
        coins = 0;
        score = 0;

        if (world > 0) {
            LoadLevel(world, stage);
        } else {
            LoadLevel(1, 1);
        }
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
        SavePlayer();

        // like the original, running out of time shows "TIME UP" first
        if (timeUp) {
            StartCoroutine(TimeUpScreen());
        } else {
            ContinueAfterDeath();
        }
    }

    private bool timeUp;

    private IEnumerator TimeUpScreen()
    {
        timeUp = false;
        hud.ShowIntro("TIME UP");
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        hud.HideIntro();
        Time.timeScale = 1f;
        ContinueAfterDeath();
    }

    private void ContinueAfterDeath()
    {
        if (lives <= 0)
        {
            GameOver();
            return;
        }

        // on a death the turn passes to the other player
        if (OtherPlayerAlive) {
            LoadPlayer(OtherPlayer);
        }

        LoadLevel(world, stage);
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

    // adds the points and shows them floating over the given world position
    public void AddScore(int points, Vector3 position)
    {
        AddScore(points);
        hud.ShowPoints(points.ToString(), position);
    }

    // stomping enemies without touching the ground is worth more each time,
    // like on the original: 100, 200, 400, 500, 800, 1000, 2000, 4000, 5000, 8000, 1UP
    private static readonly int[] StompCombo = { 100, 200, 400, 500, 800, 1000, 2000, 4000, 5000, 8000 };

    public void AddStompScore(Player player, Vector3 position)
    {
        int combo = player != null ? player.stompCombo++ : 0;

        if (combo < StompCombo.Length)
        {
            AddScore(StompCombo[combo], position);
        }
        else
        {
            AddLife();
            hud.ShowPoints("1UP", position);
        }
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

    // coins that pop out of blocks also show their points
    public void AddCoin(Vector3 position)
    {
        AddCoin();
        hud.ShowPoints("200", position);
    }

    public void AddLife()
    {
        lives++;
        AudioManager.Play("1up");
    }

    private void TogglePause()
    {
        // mario changing size also stops the game, it can't be paused then
        if (hud.introVisible || Player.Transforming) return;

        bool paused = Time.timeScale > 0f;
        Time.timeScale = paused ? 0f : 1f;
        hud.SetPaused(paused);
        AudioManager.Play("select");
    }

}
