using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureSlicer
{
    public static Sprite[,] SliceSprite(Sprite sprite, int columns, int rows)
    {
        Texture2D texture = sprite.texture;
        Sprite[,] result = new Sprite[columns, rows];
        float tWidth = texture.width;
        float tHeight = texture.height;
        float partWidth = tWidth / columns;
        float partHeight = tHeight / rows;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Rect spRct = new Rect(i * partWidth, (rows - j - 1) * partHeight, partWidth, partHeight);
                result[i, j] = Sprite.Create(texture, spRct, Vector2.zero);
            }
        }
        return result;
    }
}
