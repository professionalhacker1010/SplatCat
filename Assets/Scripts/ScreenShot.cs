using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenShot
{
    static int depth = 11;

    public static Sprite CaptureScreen()
    {
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;

        RenderTexture renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32);
        Rect rect = new Rect(0, 0, width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        GameObject.Destroy(renderTexture);

        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        return sprite;
    }
}
