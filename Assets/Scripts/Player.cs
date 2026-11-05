using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;
    private PlayerSpriteRenderer activeRenderer;

    public CapsuleCollider2D capsuleCollider { get; private set; }
    public DeathAnimation deathAnimation { get; private set; }

    public bool big { get; private set; }
    public bool dead => deathAnimation.enabled;
    public bool starpower { get; private set; }
    public bool climbing { get; set; }

    // after being hit mario blinks and enemies can't hurt him for a while
    public bool invulnerable { get; private set; }
    public float invulnerableTime = 2f;

    // the whole game stops while mario changes size, like the original
    public static bool Transforming { get; private set; }

    // enemies stomped in a row without touching the ground
    public int stompCombo { get; set; }

    private Sprite midSprite;
    private Coroutine invulnerability;

    // frames of the original animations: S small, M middle, B big,
    // J big mario ducking (jump sprite) and s small mario jumping
    private const string GrowFrames = "SMSMSMBSMBSB";
    private const string ShrinkFrames = "JsJsJsJsJsS";
    private const float FrameTime = 0.067f;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        deathAnimation = GetComponent<DeathAnimation>();
        activeRenderer = smallRenderer;
        big = bigRenderer.enabled;
        // luigi (second player) has his own colors for the middle frame
        bool luigi = GameManager.Instance != null && GameManager.Instance.currentPlayer == 1;
        midSprite = Resources.Load<Sprite>(luigi ? "Sprites/Luigi_Mid_Grow" : "Sprites/Mario_Mid_Grow");
    }

    private void OnDestroy()
    {
        AudioManager.SetStar(false);

        if (Transforming)
        {
            Transforming = false;
            Time.timeScale = 1f;
        }

        SetEnemyCollision(true);
    }

    public void Hit()
    {
        if (!dead && !starpower && !invulnerable && !Transforming)
        {
            if (big) {
                Shrink();
            } else {
                Death();
            }
        }
    }

    public void Death()
    {
        if (dead) return;

        StopAllCoroutines();
        ClearOverrides();
        SetEnemyCollision(true);

        if (starMaterial != null) {
            SetStarMaterial(false);
        }

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;

        AudioManager.StopMusic();
        AudioManager.SetStar(false);
        AudioManager.Play("death");
        GameManager.Instance.ResetLevel(3f);
    }

    public void Grow()
    {
        if (big) return;

        StopInvulnerability();
        big = true;
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(SizeAnimation(GrowFrames, false));
    }

    public void Shrink()
    {
        AudioManager.Play("damage");

        big = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(SizeAnimation(ShrinkFrames, true));
    }

    private IEnumerator SizeAnimation(string frames, bool thenInvulnerable)
    {
        Transforming = true;
        Time.timeScale = 0f;

        foreach (char frame in frames)
        {
            ShowFrame(frame);
            yield return new WaitForSecondsRealtime(FrameTime);
        }

        ClearOverrides();
        smallRenderer.enabled = activeRenderer == smallRenderer;
        bigRenderer.enabled = activeRenderer == bigRenderer;

        Transforming = false;
        Time.timeScale = 1f;

        if (thenInvulnerable) {
            invulnerability = StartCoroutine(Invulnerability());
        }
    }

    private void ShowFrame(char frame)
    {
        bool showBig = frame == 'M' || frame == 'B' || frame == 'J';
        smallRenderer.enabled = !showBig;
        bigRenderer.enabled = showBig;

        switch (frame)
        {
            case 'M': bigRenderer.overrideSprite = midSprite != null ? midSprite : bigRenderer.idle; break;
            case 'J': bigRenderer.overrideSprite = bigRenderer.jump; break;
            case 's': smallRenderer.overrideSprite = smallRenderer.jump; break;
            default:
                smallRenderer.overrideSprite = null;
                bigRenderer.overrideSprite = null;
                break;
        }
    }

    private void ClearOverrides()
    {
        smallRenderer.overrideSprite = null;
        bigRenderer.overrideSprite = null;
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        SetEnemyCollision(false);

        float elapsed = 0f;

        while (elapsed < invulnerableTime)
        {
            elapsed += Time.deltaTime;
            activeRenderer.spriteRenderer.enabled = Mathf.FloorToInt(elapsed * 20f) % 2 == 0;
            yield return null;
        }

        StopInvulnerability();
    }

    private void StopInvulnerability()
    {
        if (invulnerability != null)
        {
            StopCoroutine(invulnerability);
            invulnerability = null;
        }

        smallRenderer.spriteRenderer.enabled = smallRenderer.enabled;
        bigRenderer.spriteRenderer.enabled = bigRenderer.enabled;
        SetEnemyCollision(true);
        invulnerable = false;
    }

    // mario passes through enemies while he is invulnerable
    private static void SetEnemyCollision(bool enabled)
    {
        int player = LayerMask.NameToLayer("Player");
        int enemy = LayerMask.NameToLayer("Enemy");
        int shell = LayerMask.NameToLayer("Shell");

        Physics2D.IgnoreLayerCollision(player, enemy, !enabled);
        Physics2D.IgnoreLayerCollision(player, shell, !enabled);
    }

    // the star lasts 35 ticks of 21 frames; on the last 4 ticks the colors
    // change every 4 frames instead of every frame, like the original
    private const float StarDuration = 12.25f;
    private const float SlowCycleTime = 1.4f;

    private Coroutine starRoutine;
    private Material starMaterial;
    private Material normalMaterial;

    public void Starpower()
    {
        if (starRoutine != null) {
            StopCoroutine(starRoutine);
        }

        starRoutine = StartCoroutine(StarpowerAnimation());
    }

    private void SetStarMaterial(bool star)
    {
        if (starMaterial == null)
        {
            normalMaterial = smallRenderer.spriteRenderer.sharedMaterial;
            starMaterial = new Material(Resources.Load<Shader>("Shaders/StarPalette"));

            bool luigi = GameManager.Instance != null && GameManager.Instance.currentPlayer == 1;
            Color[][] from = luigi ? StarColors.Luigi : StarColors.Mario;
            string[] roles = { "A", "B", "C" };

            for (int i = 0; i < roles.Length; i++)
            {
                starMaterial.SetColor($"_From{roles[i]}1", from[i][0]);
                starMaterial.SetColor($"_From{roles[i]}2", from[i][1]);
            }
        }

        smallRenderer.spriteRenderer.sharedMaterial = star ? starMaterial : normalMaterial;
        bigRenderer.spriteRenderer.sharedMaterial = star ? starMaterial : normalMaterial;
    }

    private IEnumerator StarpowerAnimation()
    {
        starpower = true;
        SetStarMaterial(true);
        AudioManager.SetStar(true);

        float elapsed = 0f;

        while (elapsed < StarDuration)
        {
            elapsed += Time.deltaTime;

            int frame = Mathf.FloorToInt(elapsed * 60f);
            bool slow = StarDuration - elapsed < SlowCycleTime;
            int step = (slow ? frame / 4 : frame) % 4;

            // step 0 is the player's own colors
            if (step == 0)
            {
                starMaterial.SetFloat("_Swap", 0f);
            }
            else
            {
                Color[] palette = StarColors.ForCurrentArea()[step - 1];
                starMaterial.SetColor("_ToA", palette[0]);
                starMaterial.SetColor("_ToB", palette[1]);
                starMaterial.SetColor("_ToC", palette[2]);
                starMaterial.SetFloat("_Swap", 1f);
            }

            yield return null;
        }

        SetStarMaterial(false);
        starpower = false;
        starRoutine = null;
        AudioManager.SetStar(false);
    }

}
