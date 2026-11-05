using System;
using System.Collections.Generic;
using UnityEngine;

// Luigi uses the same frames as Mario with his own colors. When it is
// Luigi's turn every Mario sprite on the prefab is replaced by its pair.
public class PlayerCharacter : MonoBehaviour
{
    public Sprite[] marioSprites;
    public Sprite[] luigiSprites;

    private Dictionary<Sprite, Sprite> pairs;

    private void Awake()
    {
        GameManager game = GameManager.Instance;

        if (game == null || game.currentPlayer != 1) return;

        pairs = new Dictionary<Sprite, Sprite>();

        for (int i = 0; i < Mathf.Min(marioSprites.Length, luigiSprites.Length); i++) {
            pairs[marioSprites[i]] = luigiSprites[i];
        }

        foreach (PlayerSpriteRenderer renderer in GetComponentsInChildren<PlayerSpriteRenderer>(true))
        {
            renderer.idle = Swap(renderer.idle);
            renderer.jump = Swap(renderer.jump);
            renderer.slide = Swap(renderer.slide);
            renderer.swim = Swap(renderer.swim);
            renderer.climb = Swap(renderer.climb);

            SpriteRenderer sprite = renderer.GetComponent<SpriteRenderer>();
            sprite.sprite = Swap(sprite.sprite);
        }

        foreach (AnimatedSprite animation in GetComponentsInChildren<AnimatedSprite>(true)) {
            animation.sprites = Swap(animation.sprites);
        }

        DeathAnimation death = GetComponent<DeathAnimation>();

        if (death != null) {
            death.deadSprite = Swap(death.deadSprite);
        }
    }

    private Sprite Swap(Sprite sprite)
    {
        return sprite != null && pairs.TryGetValue(sprite, out Sprite luigi) ? luigi : sprite;
    }

    private Sprite[] Swap(Sprite[] sprites)
    {
        return sprites != null ? Array.ConvertAll(sprites, Swap) : null;
    }

}
