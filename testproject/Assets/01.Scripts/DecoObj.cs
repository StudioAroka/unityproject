using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DecoObj : MonoBehaviour
{
    public Vector3 RayPointsCenter
    {
        get
        {
            Vector3 vectorSum = Vector3.zero;
            for (int i = 0; i < rayPoints.Length; i++)
            {
                vectorSum += rayPoints[i].transform.position;
            }
            return vectorSum / 3f;
        }
    }
    [Title("PRESET")]
    public Transform[] rayPoints;
    public Transform graphicParent;

    [Title("RUNTIME")]
    public List<Hexagon> nowInstalledHexagons;

    public void InitializeDecoObj()
    {
        graphicParent.gameObject.SetActive(false);
    }

    public List<Hexagon> RayHexagons
    {
        get
        {
            List<Hexagon> hexagons = new List<Hexagon>();
            for (int i = 0; i < rayPoints.Length; i++)
            {
                Ray ray = new Ray();
                ray.origin = rayPoints[i].transform.position;
                ray.direction = Vector3.down;

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerManager.Instance.layerMask_hexagon))
                {
                    Hexagon hexagon = hit.transform.GetComponentInParent<Hexagon>();
                    if (hit.transform != null)
                    {
                        hexagons.Add(hexagon);
                    }
                }
            }
            return hexagons;
        }
    }
    private void OnDrawGizmos()
    {
        rayPoints[0].transform.localPosition = Vector3.zero;
    }

    public bool InstallAvailable(TileType tileType, List<Hexagon> rayHexagons)
    {
        if(rayHexagons.Count != rayPoints.Length)
        {
          //  Debug.LogWarning("(rayHexagons.Count != rayPoints.Length");
            return false;
        }
        if (!IsAllHexagonTileTypeIdenticalWith(rayHexagons, tileType))
        {
    //        Debug.LogWarning("!IsAllHexagonTileTypeIdenticalWith");
            return false;
        }
        if(!IsAllHexagonTileObjsInstallAvailable(rayHexagons))
        {
       //     Debug.LogWarning("!IsAllHexagonTileObjsInstallAvailable");
            return false;
        }
        if(!IsAllHexagonInSameStagePart(rayHexagons))
        {
        //    Debug.LogWarning("!IsAllHexagonInSameStagePart");
            return false;
        }

        return true;
    }
    bool IsAllHexagonTileTypeIdenticalWith(List<Hexagon> hexagons, TileType tileType)
    {
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].nowTileType != tileType)
            {
                return false;
            }
        }
        Debug.LogWarning("All Hexagons have identical tile type");
        return true;
    }
    bool IsAllHexagonInSameStagePart(List<Hexagon> hexagons)
    {
        StagePart parentStagePart = hexagons[0].ParentStagePart;
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].ParentStagePart != parentStagePart)
            {
                return false;
            }
        }
        Debug.LogWarning("All Hexagons have identical tile type");
        return true;
    }
    bool IsAllHexagonTileObjsInstallAvailable(List<Hexagon> hexagons)
    {
        for (int i = 0; i < hexagons.Count; i++)
        {
            if(hexagons[i].NowEquippedTileObj == null)
            {
                return false;
            }
            if (!hexagons[i].NowEquippedTileObj.decoInstallAvailable)
            {
                Debug.LogWarning("It Contains Not InstallAvailable TileObj");
                return false;
            }
            if (hexagons[i].NowEquippedTileObj.nowDecoInstalled)
            {
                Debug.LogWarning("It Contains Already Installed TileObj");
                return false;
            }
        }
        Debug.LogWarning("All Hexagons is Install available");
        return true;
    }

    public void InstallAtRayHexagons(bool b, List<Hexagon> rayHexagons)
    {
        if(b)
        {
            graphicParent.gameObject.SetActive(true);
            for(int i = 0; i < rayHexagons.Count; i++)
            {
                rayHexagons[i].NowEquippedTileObj.nowDecoInstalled = true;
                Hexagon hexagonToDecoInstall = rayHexagons[i];
                hexagonToDecoInstall.InstallBasicTileObjOnHexagon(0f);
            }
            nowInstalledHexagons = rayHexagons;
        }
        else
        { 
            graphicParent.gameObject.SetActive(false);
            for (int i = 0; i < nowInstalledHexagons.Count; i++)
            {
                nowInstalledHexagons[i].NowEquippedTileObj.nowDecoInstalled = false;
            }
            nowInstalledHexagons = null;
        }
    }
}
