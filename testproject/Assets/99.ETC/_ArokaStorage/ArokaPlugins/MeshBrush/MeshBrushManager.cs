using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Brush
{
    public static Texture2D SpreadBrush => MeshBrushManager.Instance.sampleBrush_Spread;
    public static Texture2D CircleBrush => MeshBrushManager.Instance.sampleBrush_Circle;
    public static Texture2D BoxBrush => MeshBrushManager.Instance.sampleBrush_Box;
}


public class MeshBrushManager : MonoBehaviour
{
    #region singleTone
    private static MeshBrushManager _instance = null;
    public static MeshBrushManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MeshBrushManager)) as MeshBrushManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("SkinAlphaMap")]
    public Texture2D sampleBrush_Spread;
    public Texture2D sampleBrush_Box;
    public Texture2D sampleBrush_Circle;

    [Range(0, 1000)]
    public int sampleBrushResizingSize = 200;

    //이니셜할때 붓사이즈 정할때
    public Texture2D ResizeTex(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.filterMode = FilterMode.Trilinear;

        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }

    public void BrushedOnHitCoord(RaycastHit hit, string texName, Texture2D _brushTex, Color _brushColor, Vector2 hitCoord)
    {
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        int brushWidth = _brushTex.width;
        int brushHeight = _brushTex.height;

        Texture2D tex = rend.material.GetTexture(texName) as Texture2D;
        // tex.wrapMode = TextureWrapMode.Clamp;
        // tex.filterMode = FilterMode.Trilinear;

        Vector2 pixelUV = hitCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        pixelUV.x -= (brushWidth * 0.5f);
        pixelUV.y -= (brushHeight * 0.5f);
        int fillAreaCenterX = (int)pixelUV.x;
        int fillAreaCenterY = (int)pixelUV.y;

        int brushAxisX = 0;
        int brushAxisY = 0;

        if (fillAreaCenterX < 0)
        {
            float perone = Mathf.Abs(fillAreaCenterX) / (_brushTex.width * 0.5f);
            brushWidth = (int)Mathf.Lerp(brushWidth, brushWidth * 0.5f, perone);
            fillAreaCenterX = 0;
            brushAxisX = _brushTex.width - brushWidth;
        }

        if (fillAreaCenterY < 0)
        {
            float perone = Mathf.Abs(fillAreaCenterY) / (_brushTex.height * 0.5f);
            brushHeight = (int)Mathf.Lerp(brushHeight, brushHeight * 0.5f, perone);
            fillAreaCenterY = 0;
            brushAxisY = _brushTex.height - brushHeight;
        }

        if (fillAreaCenterX + brushWidth > tex.width)
        {
            float perone = Mathf.Abs((fillAreaCenterX + brushWidth - tex.width)) / (_brushTex.width * 0.5f);
            brushAxisX = 0;
            brushWidth = (int)Mathf.Lerp(_brushTex.width, _brushTex.width * 0.5f, perone);
        }

        if (fillAreaCenterY + brushHeight > tex.height)
        {
            float perone = Mathf.Abs((fillAreaCenterY + brushHeight - tex.height)) / (_brushTex.height * 0.5f);
            brushAxisY = 0;
            brushHeight = (int)Mathf.Lerp(_brushTex.height, _brushTex.height * 0.5f, perone);
        }

        Color[] colors = tex.GetPixels(fillAreaCenterX, fillAreaCenterY, brushWidth, brushHeight);

        Color[] brushAlphaMap = _brushTex.GetPixels(brushAxisX, brushAxisY, brushWidth, brushHeight);

        for (int i = 0; i < colors.Length; i++)
        {
            float perone = (float)i / colors.Length;
            colors[i] = Color.Lerp(colors[i], _brushColor, brushAlphaMap[i].a);
        }

        tex.SetPixels(fillAreaCenterX, fillAreaCenterY, brushWidth, brushHeight, colors);
        tex.Apply();
    }


    Texture2D RotateTexture(Texture2D tex, float angle)
    {
        Texture2D rotImage = new Texture2D(tex.width, tex.height);
        int x, y;
        float x1, y1, x2, y2;

        int w = tex.width;
        int h = tex.height;
        float x0 = Rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
        float y0 = Rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

        float dx_x = Rot_x(angle, 1.0f, 0.0f);
        float dx_y = Rot_y(angle, 1.0f, 0.0f);
        float dy_x = Rot_x(angle, 0.0f, 1.0f);
        float dy_y = Rot_y(angle, 0.0f, 1.0f);

        x1 = x0;
        y1 = y0;

        for (x = 0; x < tex.width; x++)
        {
            x2 = x1;
            y2 = y1;
            for (y = 0; y < tex.height; y++)
            {

                x2 += dx_x;//rot_x(angle, x1, y1);
                y2 += dx_y;//rot_y(angle, x1, y1);
                rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), GetPixel(tex, x2, y2));
            }
            x1 += dy_x;
            y1 += dy_y;
        }

        rotImage.Apply();
        return rotImage;
    }

    private Color GetPixel(Texture2D tex, float x, float y)
    {
        Color pix;
        int x1 = (int)Mathf.Floor(x);
        int y1 = (int)Mathf.Floor(y);

        if (x1 > tex.width || x1 < 0 ||
           y1 > tex.height || y1 < 0)
        {
            pix = Color.clear;
        }
        else
        {
            pix = tex.GetPixel(x1, y1);
        }

        return pix;
    }

    private float Rot_x(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * cos + y * (-sin));
    }
    private float Rot_y(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * sin + y * cos);
    }


}
