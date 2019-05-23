using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageSlicer
{
    /// <summary>
    /// Slice a Texture, cropping a square fit from inside.
    /// </summary>
    /// <param name="image">The image to slice, must be enable read/writing</param>
    /// <param name="blocksPerLine"></param>
    /// <returns></returns>
    public static Texture2D[,] GetSlices(Texture2D image, int blocksPerLine)
    {
        int imageSize = Mathf.Min(image.height, image.width);
        int xOffset = image.width - imageSize;
        int yOffset = image.height - imageSize;

        int blockSize = Mathf.FloorToInt(imageSize / blocksPerLine);
        var slices = new Texture2D[blocksPerLine,blocksPerLine];

        for (int i = 0; i < blocksPerLine; i++)
        {
            for (int j = 0; j < blocksPerLine; j++)
            {
                var texture = new Texture2D(blockSize,blockSize);
                var pixels = image.GetPixels(xOffset + i * blockSize, yOffset + j * blockSize, blockSize, blockSize);

                texture.SetPixels(pixels);
                texture.Apply();
                slices[i, j] = texture;
            }
        }

        return slices;
    }
}


/*

 Color[] pix = sourceTex.GetPixels(x, y, width, height);
        Texture2D destTex = new Texture2D(width, height);
        destTex.SetPixels(pix);
        destTex.Apply();

        // Set the current object's texture to show the
        // extracted rectangle.
        GetComponent<Renderer>().material.mainTexture = destTex;
*/
