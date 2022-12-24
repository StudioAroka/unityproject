using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum clusionType
{
    None,
    Exclusion,
    Inclusion
}
[System.Serializable]
public class LineData
{
    public bool isDecorationLine;
    public TileType tileType;
    public List<Hexagon> hexagons = new List<Hexagon>();
}
public class Destination : MonoBehaviour
{
    public clusionType clusionType = clusionType.None;
    public int stagePartNum = -1;
    public TileType tileType = TileType.None;
    public bool isSetStagePart = false;
    public bool isDecoration = false;
    public List<LineData> lineDatas = new List<LineData>();
    public List<Hexagon> divisionTargets = new List<Hexagon>();
    public List<Wall> walls = new List<Wall>();
    public void AddDivisionTarget(Hexagon _hexagon)
    {
        if (!divisionTargets.Contains(_hexagon))
        {
            divisionTargets.Add(_hexagon);
        }
    }

    public void SetTileType(TileType _tileType)
    {
        transform.GetComponent<Hexagon>().Rend.material = ResourceManager.Instance.GetTilePlan(_tileType).tileMat;
        clusionType = clusionType.Inclusion;
        tileType = _tileType;
    }

    public void SetStagePart(int _stagePartNum)
    {
        isSetStagePart = true;
        stagePartNum = _stagePartNum;
    }

    public GameObject decorationSymbol;

    public void SetDecorationSymbol(bool b)
    {
        if(decorationSymbol != null)
        {
            DestroyImmediate(decorationSymbol);
        }
        if (b)
        {
            decorationSymbol = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            decorationSymbol.transform.SetParent(transform);
            decorationSymbol.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (decorationSymbol != null)
            {
                DestroyImmediate(decorationSymbol);
            }
        }
    }

    public void SetDecoration()
    {
        isDecoration = true;
        SetDecorationSymbol(isDecoration);
        isSetStagePart = false;
        clusionType = clusionType.Inclusion;
    }

    public void SetExclusion()
    {
        SetDecorationSymbol(false);
        transform.GetComponent<Hexagon>().Rend.material.color = Color.gray * 0.05f;
        isSetStagePart = false;
        stagePartNum = -1;
        tileType = TileType.None;
        clusionType = clusionType.Exclusion;
    }

    public bool IsSetDestination()
    {   
        return !(clusionType == clusionType.None) && isSetStagePart;
    }
}

public static class DestinationHelper
{
    public static Destination GetDestination(this Hexagon hexagon)
    {
        Destination destination = hexagon.GetComponent<Destination>();
        if (destination == null)
        {
            destination = hexagon.gameObject.AddComponent<Destination>();
        }
        return destination;
    }


    public static void SetTileType(this List<Hexagon> hexgons, TileType _tileType)
    {

        for (int i = 0; i < hexgons.Count; i++)
        {
            Hexagon hexagon_i = hexgons[i];
            Destination destination = hexagon_i.GetComponent<Destination>();
            if (destination == null)
            {
                destination = hexagon_i.gameObject.AddComponent<Destination>();
            }

           

            destination.SetTileType(_tileType);
        }
    }

    public static void SetStagePart(this List<Hexagon> hexgons, int _stagePartNum)
    {
        for (int i = 0; i < hexgons.Count; i++)
        {
            Hexagon hexagon_i = hexgons[i];
            Destination destination = hexagon_i.GetComponent<Destination>();
            if (destination == null)
            {
                destination = hexagon_i.gameObject.AddComponent<Destination>();
            }
            destination.SetStagePart(_stagePartNum);
        }
    }



    public static void SetExclusion(this List<Hexagon> hexgons)
    {
        for (int i = 0; i < hexgons.Count; i++)
        {
            Hexagon hexagon_i = hexgons[i];
            Destination destination = hexagon_i.GetComponent<Destination>();
            if (destination == null)
            {
                destination = hexagon_i.gameObject.AddComponent<Destination>();
            }
            destination.SetExclusion();
        }
    }

    public static void SetDecoration(this List<Hexagon> hexgons)
    {
        for (int i = 0; i < hexgons.Count; i++)
        {
            Hexagon hexagon_i = hexgons[i];
            Destination destination = hexagon_i.GetComponent<Destination>();
            if (destination == null)
            {
                destination = hexagon_i.gameObject.AddComponent<Destination>();
            }
            destination.SetDecoration();
        }
    }


}
