using UnityEngine;
using UnityEditor;
using System.IO;

// example PostProcessor for adjusting automatic Sprite Import settings
// save this in any "Editor" Folder
public class SpriteImportProcessor : AssetPostprocessor
{
	void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
	{
		TextureImporter importer = assetImporter as TextureImporter;

		// only change sprite import settings on first import, so we can change those settings for individual files

		string name = importer.assetPath.ToLower();
		if (File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(name)))
		{
			return;
		}

		// adjust values for pixel art

		importer.spritePixelsPerUnit = 1;
		importer.mipmapEnabled = false;
		importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

		// access the TextureImporterSettings to change the spriteAlignment value

		TextureImporterSettings textureSettings = new TextureImporterSettings();
		importer.ReadTextureSettings(textureSettings);

		//textureSettings.spritePivot = new Vector2(0.5f, 0f);
		textureSettings.spriteAlignment = (int)SpriteAlignment.Center;

		importer.SetTextureSettings(textureSettings);
		
		importer.SaveAndReimport();
	}
}