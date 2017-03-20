using UnityEngine;
using System.Collections;

public static class SpriteSheetLoader
{
    public static Sprite LoadSpriteFromSheet(string sheetPath, string spriteName) //Loads a standalone sprite from a sheet within Resources
    {
        Sprite returnSprite = null;

        Sprite[] spriteSheet = Resources.LoadAll<Sprite>(sheetPath);

        for(int i = 0; i < spriteSheet.Length; i++)
        {

            if(spriteSheet[i].name == spriteName)
            {
                returnSprite = spriteSheet[i];
                break;
            }
        }

        return returnSprite;
    }
}
