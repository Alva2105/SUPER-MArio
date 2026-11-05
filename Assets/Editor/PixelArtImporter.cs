using UnityEditor;
using UnityEngine;

// same import settings as the original sprites: 16 px per unit, no filtering
public class PixelArtImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Sprites/Extra/") && !assetPath.StartsWith("Assets/Resources/Fonts/")
            && !assetPath.StartsWith("Assets/Resources/HUD/")
            && !assetPath.StartsWith("Assets/Resources/Sprites/")) return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        // small strips like 24x8 can be detected as a cubemap, they are always 2D
        importer.textureShape = TextureImporterShape.Texture2D;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteAlignment = (int)SpriteAlignment.Center;
        importer.SetTextureSettings(settings);
    }

}
