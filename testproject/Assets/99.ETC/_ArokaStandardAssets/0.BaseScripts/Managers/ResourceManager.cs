using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum TileType
{
    None,
    Forest,
    Village,
    Snow,
    Sea,
    Desert,
    Volcano
}
[System.Serializable]
public class TilePlan
{
    public TileType tileType;
    public Gradient tileColorGradient;
    public Material tileMat;
    public List<TileObj> tileObjs;
    public List<DecoObj> decoObjs;
    public TileObj RandomTileObj
    {
        get
        {
            List<TileObj> randTileObjPreminaryList = new List<TileObj>();
            for (int i = 0; i < tileObjs.Count; i++)
            {
                for (int j = 0; j < tileObjs[i].possibilityInt; j++)
                {
                    randTileObjPreminaryList.Add(tileObjs[i]);
                }
            }

            if (randTileObjPreminaryList.Count > 0)
            {
                return randTileObjPreminaryList[Random.Range(0, randTileObjPreminaryList.Count)];
            }
            else
            {
                return null;
            }
        }
    }

}
[System.Serializable]
public class TileObjPlan
{
    public TileObj tileObj;
    public int possibilityInt = 1;
}

public class ResourceManager : MonoBehaviour
{

    #region singleTone
    private static ResourceManager _instance = null;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion


    public Wall wallPrefab;
    public TilePlan[] tilePlans;
    public PuzzleLineRenderer lineRendererPrefab;
    public List<Color> groupColors = new List<Color>();
    public GameObject endPointDisplayerPrefab;

    public TilePlan GetTilePlan(TileType tileType)
    {
        for(int i = 0; i < tilePlans.Length; i++)
        {
            if(tilePlans[i].tileType == tileType)
            {
                return tilePlans[i];
            }
        }
        return null;
    }

    [Button("색깔 보정버튼")]
    public void Refresh()
    {
        for (int i = 0; i < tilePlans.Length; i++)
        {
            tilePlans[i].tileMat.color = tilePlans[i].tileColorGradient.colorKeys[0].color;
        }
    }



}