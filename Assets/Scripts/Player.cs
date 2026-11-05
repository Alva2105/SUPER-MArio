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

    // the whole game stops while mario changes size, like the original
    public static bool Transforming { get; private set; }

    // enemies stomped in a row without touching the ground
    public int stompCombo { get; set; }

    private Sprite midSprite;

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
        if (Transforming)
        {
            Transforming = false;
            Time.timeScale = 1f;
        }
    }

    public void Hit()
    {
        if (!dead && !starpower && !Transforming)
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

        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;

        AudioManager.StopMusic();
        AudioManager.Play("death");
        GameManager.Instance.ResetLevel(3f);
    }

    public void Grow()
    {
        if (big) return;

        big = true;
        activeRenderer = bigRenderer;

        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0f, 0.5f);

        StartCoroutine(SizeAnimation(GrowFrames));
    }

    public void Shrink()
    {
        AudioManager.Play("pipe");

        big = false;
        activeRenderer = smallRenderer;

        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0f, 0f);

        StartCoroutine(SizeAnimation(ShrinkFrames));
    }

    private IEnumerator SizeAnimation(string frames)
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

    public void Starpower()
    {
        StartCoroutine(StarpowerAnimation());
    }

    private IEnumerator StarpowerAnimation()
    {
        starpower = true;

        float elapsed = 0f;
        float duration = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0) {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }

}
