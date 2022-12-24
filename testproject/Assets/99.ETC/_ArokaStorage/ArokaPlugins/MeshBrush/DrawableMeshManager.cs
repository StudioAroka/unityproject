using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class TextureGroupHistroy
{
    public Texture2D texture2D;
    public TextureGroup textureGroup;

    public TextureGroupHistroy(TextureGroup _textureGroup, Texture2D _texture2D)
    {
        texture2D = _texture2D;
        textureGroup = _textureGroup;
    }
}

public enum TextureGroup
{
    None,
    Brim,
    Crown,
    C
}

public class DrawableMeshManager : MonoBehaviour
{

    #region singleTone
    private static DrawableMeshManager _instance = null;
    public static DrawableMeshManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(DrawableMeshManager)) as DrawableMeshManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    [ReadOnly]
    public List<TextureGroupHistroy> textureGroupHistroies = new List<TextureGroupHistroy>();

    public void RegistTextureGroupHistory(TextureGroup _textureGroup, Texture2D _originTex)
    {
        if (GetTexture2D_InHistory(_textureGroup) == null)
        {
            textureGroupHistroies.Add(new TextureGroupHistroy(_textureGroup, Instantiate(_originTex)));
        }
    }

    public Texture2D GetTexture2D_InHistory(TextureGroup _textureGroup)
    {
        for (int i = 0; i < textureGroupHistroies.Count; i++)
        {
            if (_textureGroup == textureGroupHistroies[i].textureGroup)
            {
                return textureGroupHistroies[i].texture2D;
            }
        }
        return null;
    }

    public void ClearHistory()
    {
        textureGroupHistroies.Clear();
    }

}
