using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class TileObjSituation
{
    public TileType tileType;
    public TileObj nowTileObj;
    public Quaternion localRotWhenInstall;

    public TileObjSituation(TileType _tileType, TileObj _nowTileObj, Quaternion _localRotWhenInstall)
    {
        tileType = _tileType;
        nowTileObj = _nowTileObj;
        localRotWhenInstall = _localRotWhenInstall;
    }
}
[SelectionBase]
public class Hexagon : MonoBehaviour
{

    [Title("AFTERBAKE")]
    public TileType nowTileType;
    public Vector3Int coord;
    public bool isStartHexagon;
    public bool isLastHexagon;
    public List<Hexagon> nearHexagons;
    public bool isDecorationHexagon;

    [Title("PRESET")]
    public GameObject edgeDisplayer;
    public Collider ColObj => transform.GetComponentInChildren<Collider>();
    public Transform graphicParent;
    public MeshRenderer Rend => graphicParent.GetComponent<MeshRenderer>();

    [Title("RUNTIME")]
    public List<TileObjSituation> tileObjSituations;
    public int onCount;
    [Title("DEBUG")]
    public TileObj NowEquippedTileObj => GetTileObjSituation(nowTileType) != null ? GetTileObjSituation(nowTileType).nowTileObj : null;
    public StagePart ParentStagePart => GetComponentInParent<StagePart>();
    public bool IsEdgeHexagon => isStartHexagon || isLastHexagon;

    public TileObjSituation GetTileObjSituation(TileType tileType)
    {
        for (int i = 0; i < tileObjSituations.Count; i++)
        {
            if (tileObjSituations[i].tileType == tileType)
            {
                return tileObjSituations[i];
            }
        }
        return null;
    }
    public TileType initialTileType;

    public void InitializeHexagon(List<TileSituation> tileSituations)
    {
        gameObject.SetLayerRecursively(LayerManager.Instance.layerMask_hexagon);

        if (shiningHexagonParticle == null)
        {
            shiningHexagonParticle = Instantiate(ParticleManager.Instance.shiningHexagonEffect.gameObject, transform).GetComponent<ParticleSystem>();
            shiningHexagonParticle.transform.localPosition = Vector3.up * 0f;
            shiningHexagonParticle.transform.localRotation = Quaternion.Euler(new Vector3(-180f, 0f, 0f));
        }
        if (IsEdgeHexagon)
        {
            if (edgeDisplayer)
            {
                GameObject.Destroy(edgeDisplayer);
            }
            edgeDisplayer = GameObject.Instantiate(ResourceManager.Instance.endPointDisplayerPrefab.gameObject, ParentStagePart.transform);
            edgeDisplayer.transform.name = "fisrtHexagonDisplayer";
            edgeDisplayer.transform.localScale = Vector3.one;
            edgeDisplayer.transform.localRotation = Quaternion.Euler(Vector3.up * 30f);
            edgeDisplayer.transform.position = transform.position.ModifiedY(edgeDisplayerHeight);
            edgeDisplayer.GetComponent<MeshRenderer>().material.color = ResourceManager.Instance.GetTilePlan(initialTileType).tileColorGradient.Evaluate(0f);
        }

        RegisterDestinyTileObj(tileSituations);
    }

    public void RegisterDestinyTileObj(List<TileSituation> tileSituations)
    {
        tileObjSituations = new List<TileObjSituation>();
        for (int i = 0; i < tileSituations.Count; i++)
        {
            TileType tileType = tileSituations[i].tileType;
            if(tileType == TileType.None)
            {
                continue;
            }
            TilePlan tilePlan = ResourceManager.Instance.GetTilePlan(tileType);
            TileObj randTileObj = IsEdgeHexagon ? ResourceManager.Instance.GetTilePlan(TileType.None).tileObjs[0] : tilePlan.RandomTileObj;
            TileObj inst_randTileObj = Instantiate(randTileObj.gameObject, transform).GetComponent<TileObj>();
            inst_randTileObj.InitializeTileObj();
            inst_randTileObj.transform.localScale = Vector3.zero;
            Quaternion localRotWhenInstall = Quaternion.Euler(new Vector3(0, 0, 180)) * (randTileObj.isRotateAvailable ? Quaternion.Euler(Vector3.up * (30 + 60 * (int)Random.Range(0, 6))) : Quaternion.identity);
            TileObjSituation tileObjSituationToAdd = new TileObjSituation(tileType, inst_randTileObj, localRotWhenInstall);
            tileObjSituations.Add(tileObjSituationToAdd);
        }
    }
    public void SetActiveOnlyTileObj(TileType tileType, float delaySec)
    {
        for (int i = 0; i < tileObjSituations.Count; i++)
        {
            TileObjSituation tileObjSituation = tileObjSituations[i];
            TileObj tileObj = tileObjSituations[i].nowTileObj;
            bool isProper =  (tileObjSituations[i].tileType == tileType);
            if(isProper)
            {
                if(tileType != TileType.None)
                {
                    tileObj.transform.ArokaTr().SetLocalRot(tileObjSituation.localRotWhenInstall, 0.27f, CurvName.OverEaseOut, delaySec);
                    tileObj.transform.ArokaTr().SetLocalPos(Vector3.zero, 0.3f, CurvName.EaseIn, delaySec);
                    tileObj.transform.ArokaTr().SetLocalScale(Vector3.one, 0.27f, CurvName.OverEaseOut, delaySec);
                }
            }
            else
            {
                tileObj.transform.ArokaTr().SetLocalScale(Vector3.zero, 0.27f, CurvName.OverEaseOut, delaySec);
            }
        }
    }
    [Button("SETSTATUS TEST")]
    public void SetStatusTest(TileType tileType)
    {
        SetStatus(tileType, true);
    }
    [Button("SetTileObjOnlyTest TEST")]
    public void SetTileObjOnlyTest(TileType tileType)
    {
        SetActiveOnlyTileObj(tileType, 0f);
    }
    public void MoveHexagon(TileType tileType, TileType fromTileType)
    {
        TileSituation targetTileSituation = ParentStagePart.GetTileSituation(tileType);
        TileSituation originalTIleSituation = ParentStagePart.GetTileSituation(fromTileType);
        targetTileSituation.nowHexagons.Add(this);
        originalTIleSituation.nowHexagons.Remove(this);
    }
    float edgeDisplayerHeight = 1.2f;
    public void SetStatus(TileType tileType, bool usePerformance = true)
    {
        if(nowTileType == tileType)
        {
            return;
        }
        MoveHexagon(tileType, nowTileType);
        Rend.material = ResourceManager.Instance.GetTilePlan(tileType).tileMat;
        ColObj.transform.localScale = tileType != TileType.None ? Vector3.one : (Vector3.one * .7f).ModifiedY(1f);
        Quaternion targetRot;
       
        if (edgeDisplayer)
        {
            edgeDisplayer.transform.ArokaTr().SetLocalPos(transform.localPosition.ModifiedY(edgeDisplayerHeight), 0f);
        }
      
        if(IsEdgeHexagon)
        {
            targetRot = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            targetRot = (tileType == TileType.None) ? Quaternion.Euler(new Vector3(0, 0, 0)) : Quaternion.Euler(new Vector3(180, 0, 0));
        }

        nowTileType = tileType;
        transform.ArokaTr().SetLocalRot(targetRot, usePerformance ? .5f : 0f);
        onCount++;
    }

    public List<TileType> extendableForDebug;

    public ParticleSystem shiningHexagonParticle;


    public void PlayShiningHexagonParticle(bool b)
    {
        /*
        if(isRecentHexagon == b && !isFirstRefresh)
        {
            return;
        }
        isFirstRefresh = true;
        isRecentHexagon = b;*/
        if (b)
        {
            Rend.material.SetInt("_UseNotice", 1);
            ParticleSystem ps = shiningHexagonParticle;
            ParticleSystem.MainModule mainModule = ps.main;
            mainModule.startColor = ResourceManager.Instance.GetTilePlan(nowTileType).tileColorGradient.Evaluate(0);
            if(!shiningHexagonParticle.isPlaying)
                shiningHexagonParticle.Play();
        }
        else
        {
            Rend.material.SetInt("_UseNotice", 0);
            if (shiningHexagonParticle != null)
            {
                shiningHexagonParticle.Stop();
            }
        }
    }
    /*
    public List<Hexagon> GetExtendableHexagons(TileType tileType)
    {
        List<Hexagon> tmpList = new List<Hexagon>();
        List<Hexagon> extendableHexagons = ExtendableFromHexagons;
        for(int i = 0; i < extendableHexagons.Count; i++)
        {
            if(extendableHexagons[i].nowTileType == tileType)
            {
                tmpList.Add(extendableHexagons[i]);
            }
        }
        return tmpList;
    }

    */

    public Color GetProperColor(int index, int count)
    {
        if(nowTileType == TileType.None)
        {
            return ResourceManager.Instance.GetTilePlan(TileType.None).tileColorGradient.Evaluate(0f);
        }
        else if (IsEdgeHexagon)
        {
            return ResourceManager.Instance.GetTilePlan(TileType.None).tileColorGradient.Evaluate(0f);
        }
        else  if(count == 0)
        {
            return ResourceManager.Instance.GetTilePlan(TileType.None).tileColorGradient.Evaluate(0f);
        }
        else
        {
            float perone = (float)index / count;
            return ResourceManager.Instance.GetTilePlan(nowTileType).tileColorGradient.Evaluate(perone);
        }
    }
    public void InstallBasicTileObjOnHexagon(float delayTime)
    {
        if (NowEquippedTileObj)
            Destroy(NowEquippedTileObj.gameObject, delayTime);


        TileObj tileObj = Instantiate(ResourceManager.Instance.GetTilePlan(nowTileType).tileObjs[0].gameObject, transform).GetComponent<TileObj>();
        tileObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 180));
        tileObj.transform.localScale = Vector3.zero;
        tileObj.transform.ArokaTr().SetLocalScale(Vector3.one, 0.27f, CurvName.OverEaseOut, delayTime);
    }
}