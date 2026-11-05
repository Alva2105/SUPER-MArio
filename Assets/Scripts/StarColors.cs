using UnityEngine;

// Star colors of the original, taken from the "Star Colors" section of the
// Mario & Luigi sheet (The Spriters Resource). The cycle goes player color,
// star 1, star 2, star 3 and starts again; each area has its own colors.
// Every palette is { hat and shirt, skin, hair and overalls }.
public static class StarColors
{
    private static Color C(byte r, byte g, byte b) => new Color32(r, g, b, 255);

    private static readonly Color White = C(255, 254, 255);
    private static readonly Color Orange = C(234, 158, 34);
    private static readonly Color Red = C(181, 49, 32);
    private static readonly Color Pink = C(254, 204, 197);
    private static readonly Color Brown = C(153, 78, 0);
    private static readonly Color Teal = C(0, 124, 141);
    private static readonly Color LightGrey = C(173, 173, 173);

    public static readonly Color[][] Overworld = {
        new[] { C(12, 147, 0), White, Orange },
        new[] { Red, White, Orange },
        new[] { C(0, 0, 0), Pink, Brown },
    };

    public static readonly Color[][] Underground = {
        new[] { Teal, Pink, Brown },
        new[] { Red, White, Orange },
        new[] { C(0, 64, 77), C(181, 235, 242), Teal },
    };

    public static readonly Color[][] Castle = {
        new[] { Teal, Pink, Brown },
        new[] { Red, White, Orange },
        new[] { C(102, 102, 102), White, LightGrey },
    };

    public static readonly Color[][] Underwater = {
        new[] { LightGrey, White, Orange },
        new[] { Red, White, Orange },
        new[] { C(0, 0, 0), White, LightGrey },
    };

    // colors used by the sprites (FCEUX for the base frames, Nestopia for the
    // swim and climb frames), per role: { first source, second source }
    public static readonly Color[][] Mario = {
        new[] { C(216, 40, 0), Red },
        new[] { C(252, 152, 56), Orange },
        new[] { C(136, 112, 0), C(107, 109, 0) },
    };

    public static readonly Color[][] Luigi = {
        new[] { C(252, 252, 252), White },
        new[] { C(252, 152, 56), Orange },
        new[] { C(0, 168, 0), C(56, 135, 0) },
    };

    // castle and water are known by the music; underground areas have the
    // black background (the 1-2 entrance already plays the underground music)
    public static Color[][] ForCurrentArea()
    {
        string music = AudioManager.Instance != null ? AudioManager.Instance.currentMusic : null;

        if (music != null && music.Contains("castle")) return Castle;
        if (music != null && music.Contains("water")) return Underwater;

        Camera camera = Camera.main;

        if (camera != null && camera.backgroundColor.maxColorComponent < 0.1f) return Underground;
        return Overworld;
    }

}
