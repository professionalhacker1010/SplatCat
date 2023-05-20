using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortfolioCamera : MonoBehaviour
{
    static int depth = 11;
    [SerializeField] public Camera PortCam;
    List<Sprite> spriteCache = new List<Sprite>();

    public void AddCachedSprite(Vector3 pivot = default(Vector3), int width = -1, int height = -1)
    {
        if (width == -1) width = PortCam.pixelWidth;
        if (height == -1) height = PortCam.pixelHeight;

        foreach (var cachedSprite in spriteCache)
        {
            if (cachedSprite.rect.width == width && cachedSprite.rect.height == height)
                return;
        }

        Rect rect = new Rect(0, 0, width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Sprite sprite = Sprite.Create(texture, rect, pivot);
        spriteCache.Add(sprite);

        Debug.Log("sprite cached");
    }

    public Sprite ScreenShotSprite(Vector3 pivot = default(Vector3), int width = -1, int height = -1)
    {
        if (width == -1) width = PortCam.pixelWidth;
        if (height == -1) height = PortCam.pixelHeight;

        Sprite sprite = null;
        foreach (var cachedSprite in spriteCache)
        {
            if (cachedSprite.rect.width == width && cachedSprite.rect.height == height)
                sprite = cachedSprite;
        }


        RenderTexture renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32);
        Rect rect = new Rect(0, 0, width, height);

        Texture2D texture = sprite == null ? new Texture2D(width, height, TextureFormat.RGBA32, false) : sprite.texture;

        PortCam.targetTexture = renderTexture;
        PortCam.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        PortCam.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        GameObject.Destroy(renderTexture);

        if (sprite == null)
        {
            sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            spriteCache.Add(sprite);
            Debug.Log("Sprite cached during runtime - should probably cache sprite on start");
        }
        return sprite;
    }

    public Texture2D ScreenShotTexture2D(Vector3 pivot = default(Vector3))
    {
        Sprite portfolio = ScreenShotSprite(pivot);

        Texture2D portfolioTexture = new Texture2D((int)portfolio.rect.width, (int)portfolio.rect.height);
        var portfolioPixels = portfolio.texture.GetPixels((int)portfolio.textureRect.x,
                                        (int)portfolio.textureRect.y,
                                        (int)portfolio.textureRect.width,
                                        (int)portfolio.textureRect.height);
        portfolioTexture.SetPixels(portfolioPixels);
        portfolioTexture.Apply();
        return portfolioTexture;
    }
}
