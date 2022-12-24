using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum StagePartStatus
{
    NotClearedYet,
    Playing,
    Cleared,
}

public enum HexagonDirStatusName
{
    NotDefined,
    Forward,
    Inverse
}
[System.Serializable]
public class TileSituation
{
    public TileType tileType;
    [Title("PRESET")]
    public Hexagon startHexagon;
    public Hexagon endHexagon;

    [Title("RUNTIME")]
    public bool endingDecoInstalled;
    public bool isTileObjInstalled;

    public List<Hexagon> nowHexagons = new List<Hexagon>();

    public void SetOnDisplayers(bool b, float totalTime)
    {
        if(tileType == TileType.None)
        {
            return;
        }
        if (startHexagon.edgeDisplayer)
        startHexagon.edgeDisplayer.transform.ArokaTr().SetLocalScale(b? Vector3.one : Vector3.zero, totalTime);
        if (endHexagon.edgeDisplayer)
        endHexagon.edgeDisplayer.transform.ArokaTr().SetLocalScale(b ? Vector3.one : Vector3.zero, totalTime);
    }
    public void Reset(bool usePerformance)
    {
        for(int i = nowHexagons.Count - 1; i >= 0; i--)
        {
            nowHexagons[i].SetStatus(TileType.None, usePerformance);
        }
    }
    public bool isConnectedLine;
    public HexagonDirStatusName HexagonDirStatus
    {
        get
        {
            if (nowHexagons.Count > 0)
            {
                return nowHexagons[0].isStartHexagon ? HexagonDirStatusName.Forward : HexagonDirStatusName.Inverse;
            }
            else
            {
                return HexagonDirStatusName.NotDefined;
            }
        }
    }
    public void RefreshLine()
    {
        if(nowHexagons.Count > 1 && (tileType != TileType.None))
        {
            bool isProper1 = (nowHexagons[0] == startHexagon) && (nowHexagons[nowHexagons.Count - 1] == endHexagon);
            bool isProper2 = ((nowHexagons[0] == endHexagon) && (nowHexagons[nowHexagons.Count - 1] == startHexagon));
            isConnectedLine =   (isProper1 || isProper2);
        }
        else
        {
            isConnectedLine = false;
        }
        if (puzzleLineRenderer)
            puzzleLineRenderer.RefreshLineRenderer(this);
    }

    public TileSituation(TileType _tileType, Hexagon _startHexagon, Hexagon _endHexagon)
    {
        tileType = _tileType;
        startHexagon = _startHexagon;
        endHexagon = _endHexagon;
    }


    public PuzzleLineRenderer puzzleLineRenderer;
    public void MakeLineRenderers(StagePart stagePart)
    {
        if (puzzleLineRenderer)
        {
            GameObject.Destroy(puzzleLineRenderer.gameObject);
        }
        puzzleLineRenderer = GameObject.Instantiate(ResourceManager.Instance.lineRendererPrefab.gameObject).GetComponent<PuzzleLineRenderer>();
        puzzleLineRenderer.transform.SetParent(stagePart.transform);
        puzzleLineRenderer.transform.position = Vector3.zero;
    }
}

public class StagePart : MonoBehaviour
{
    public Vector3 CenterTopPos
    {
        get
        {
            List<Renderer> renderers = new List<Renderer>();
            for (int i = 0; i < totalHexagons.Count; i++)
            {
                Renderer renderer = totalHexagons[i].Rend;
                renderers.Add(renderer);
            }
            RenderersData renderersData = renderers.ToArray().GetRenderersData();
            Vector3 stagePartCenterPos = renderersData.center.ModifiedY(renderersData.maxY.y);
            return stagePartCenterPos;
        }
    }
    [Title("PRESET")]
    public List<Hexagon> totalHexagons;

    [Title("AFTERBAKE")]
    public Animator tutorialFingerTrAnimator;
    public Vector3 raiseUpDegree;
    public Transform wallsParent;
    public Transform decoHexagonsParent;
    public List<LineData> lineDatas = new List<LineData>();


    [Title("RUNTIME")]
    VibrationManager vibrationManager;
    StageManager stageManager;
    public StagePartSelectBtn nowStagePartSelectBtn;
    public StagePartStatus nowStagePartStatus;
    public List<TileSituation> nowTileSituations;
    public BakedTransform bakedTr;
    private void Awake()
    {
        vibrationManager = VibrationManager.Instance;
           stageManager = StageManager.Instance;
    }
    public bool IsAllHexagonsAreFilledCorrectly
    {
        get
        {
            for (int i = 0; i < totalHexagons.Count; i++)
            {
                if (totalHexagons[i].nowTileType == TileType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public Hexagon GetHexagon(Vector3Int coord)
    {
        for (int i = 0; i < totalHexagons.Count; i++)
        {
            if (totalHexagons[i].coord == coord)
            {
                return totalHexagons[i];
            }
        }
        return null;
    }

    public TileSituation GetTileSituation(TileType tileType)
    {
        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            if (nowTileSituations[i].tileType == tileType)
            {
                return nowTileSituations[i];
            }
        }
        return null;
    }
    void LineDatasToTileSituations()
    {
        totalHexagons = new List<Hexagon>();
        totalHexagons.AddRange(GetComponentsInChildren<Hexagon>());
        for (int i = totalHexagons.Count - 1; i >= 0; i--)
        {
            if (totalHexagons[i].isDecorationHexagon)
            {
                totalHexagons.RemoveAt(i);
            }
        }

        List<Hexagon> hexgonListToNone = new List<Hexagon>();
        for (int i = 0; i < lineDatas.Count; i++)
        {
            List<Hexagon> originalHexagons = lineDatas[i].hexagons;
            Hexagon firstHexagon = originalHexagons[0];
            Hexagon lastHexagon = originalHexagons[^1];
            TileSituation tileSituationToAdd = new TileSituation(lineDatas[i].tileType, originalHexagons[0], originalHexagons[^1]);
            tileSituationToAdd.nowHexagons = new List<Hexagon>() {  };
            nowTileSituations.Add(tileSituationToAdd);

            firstHexagon.isStartHexagon = true;
            firstHexagon.isLastHexagon = false;
            firstHexagon.initialTileType = firstHexagon.nowTileType;

            lastHexagon.isStartHexagon = false;
            lastHexagon.isLastHexagon = true;
            lastHexagon.initialTileType = lastHexagon.nowTileType;

            hexgonListToNone.AddRange(originalHexagons);
            hexgonListToNone.Remove(firstHexagon);
            hexgonListToNone.Remove(lastHexagon);
        }
        TileSituation tileSituationNone = new TileSituation(TileType.None, hexgonListToNone[0], hexgonListToNone[^1]);
        tileSituationNone.nowHexagons = hexgonListToNone;
        nowTileSituations.Add(tileSituationNone);

        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            if (nowTileSituations[i].tileType != TileType.None)
            {
                nowTileSituations[i].MakeLineRenderers(this);
                nowTileSituations[i].Reset(false);
            }
        }
        for(int i = 0; i < totalHexagons.Count; i ++)
        {
            totalHexagons[i].SetStatus(TileType.None, false);
        }

    }
    public void InitializeStagePart()
    {
        LineDatasToTileSituations();
        bakedTr = gameObject.AddComponent<BakedTransform>();
        bakedTr.BakeTransform(BakedTransform.BakedTrStatusName.WhenStageBaked);

        nowStagePartSelectBtn = Instantiate(UIManager.Instance.stagePartSelectBtnPrefab.gameObject, UIManager.Instance.stagePartSelectBtnParent).GetComponent<StagePartSelectBtn>();
        nowStagePartSelectBtn.InitializeStagePartSelectBtn(this);
        nowStagePartSelectBtn.transform.localScale = Vector3.zero;

        for (int i = 0; i < totalHexagons.Count; i++)
        {
            totalHexagons[i].InitializeHexagon(nowTileSituations);
        }
        SetStagePartStatus(StagePartStatus.NotClearedYet);
    }
    public IEnumerator StagePartEndingRoutine()
    {
        stageManager.SetRotateLight(true, stageManager.nowStage.nowSelectedStagePart.CenterTopPos.ModifiedY(5f), 0.5f);
        SetStagePartStatus(StagePartStatus.Cleared);
        //클리어씬 카메라뷰 n초 유지
        yield return new WaitForSeconds(2f);
        stageManager.SetRotateLight(false, stageManager.nowStage.nowSelectedStagePart.CenterTopPos.ModifiedY(5f), 0.5f);

        TransformData trData = bakedTr.GetBakedTrData(BakedTransform.BakedTrStatusName.WhenStageBaked).transformData;
        transform.SetParent(trData.parent);
        transform.ArokaTr().SetTransform(trData, 1f, CurvName.EaseOut);
        yield return new WaitForSeconds(1f);
    }
    enum HexagonActStatus
    {
        None,
        Modify,
        Fill
    }
    public TileSituation recentTileSituationToControl;
    IEnumerator StagePartFloatingRoutine()
    {
        Transform tmpParent = new GameObject().transform;
        tmpParent.SetParent(StageManager.Instance.nowStage.transform);
        tmpParent.position = CenterTopPos;
        transform.SetParent(tmpParent);

        float floatingTime = 1f;
        tmpParent.ArokaTr().SetLocalPos(tmpParent.position + Vector3.up * 5f, floatingTime);
        tmpParent.ArokaTr().SetLocalRotDegree(raiseUpDegree, floatingTime, CurvName.EaseOut);
        yield return new WaitForSeconds(floatingTime);
        transform.SetParent(StageManager.Instance.nowStage.transform);
        Destroy(tmpParent.gameObject);
        yield return new WaitForSeconds(1f);
    }

    Coroutine nowHexagonRoutine;
    public IEnumerator StagePartRoutine()
    {
      
        SetStagePartStatus(StagePartStatus.Playing);
        SetActiveLinerRenderersOnly(TileType.None);
        RefreshLines();
        yield return StartCoroutine(StagePartFloatingRoutine());
        if (tutorialFingerTrAnimator != null)
        {
            TutorialManager.Instance.SetActiveTutorial(TutorialMent.TutorialMent, true);
            TutorialManager.Instance.SetActiveTutorial(TutorialMent.TutorialFinger, true);
            ArokaTrackingUI trackingUIFinger = TutorialManager.Instance.GetTutorialPlan(TutorialMent.TutorialFinger).tutorialAnimObj.GetComponent<ArokaTrackingUI>();
            trackingUIFinger.InitializeArokaTrakingUI(tutorialFingerTrAnimator.gameObject, Vector3.zero, Vector3.right * 25f, .35f);
        }
        Camera mainCamera = GameManager.Instance.mainCamera;
        while (true)
        {
            if (IsAllHexagonsAreFilledCorrectly)
            {
                break;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(StaticMethod.MousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerManager.Instance.layerMask_hexagon))
                {
                    if (hit.transform != null)
                    {
                        if (nowHexagonRoutine == null)
                        {
                            Hexagon detectedHexagon = hit.transform.GetComponentInParent<Hexagon>();
                            if(detectedHexagon)
                            {
                                nowHexagonRoutine = StartCoroutine(HexagonFillRoutine(detectedHexagon));
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        SetActiveLinerRenderersOnly( TileType.None);
        if (tutorialFingerTrAnimator != null)
        {
            TutorialManager.Instance.SetActiveTutorial(TutorialMent.TutorialMent, false);
            TutorialManager.Instance.SetActiveTutorial(TutorialMent.TutorialFinger, false);
        }
    }

    public void SetActiveShiningEffectOnly(Hexagon hexagon)
    {
        for(int i = 0; i < totalHexagons.Count; i++)
        {
            totalHexagons[i].PlayShiningHexagonParticle(totalHexagons[i] == hexagon);
        }
    }

    IEnumerator HexagonFillRoutine(Hexagon initialHexagon)
    {
        Debug.Log("NEW HexagonFillRoutine");
        Camera mainCamera = GameManager.Instance.mainCamera;
        TileType tileTypeToOpen = initialHexagon.IsEdgeHexagon ? initialHexagon.initialTileType : initialHexagon.nowTileType;
        SetActiveLinerRenderersOnly(tileTypeToOpen);
        TileSituation tileSituation = GetTileSituation(tileTypeToOpen);
        recentTileSituationToControl = tileSituation;
        Hexagon previousHexagon = initialHexagon;

        if (initialHexagon.IsEdgeHexagon)
        {
            Debug.Log("edge 헥사곤을 등록하며 시작");
            tileSituation.Reset(true);
            initialHexagon.SetStatus(tileTypeToOpen);
        }
        else
        {
            Debug.Log("수정하며 시작");
            UnlinkTill(tileSituation, tileSituation.nowHexagons.IndexOf(initialHexagon), true);
        }
        previousHexagon = initialHexagon;
        RefreshLines();
        vibrationManager.Vibrate();


        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
           //     Debug.Log("마우스 클릭 Up 감지");
                break;
            }
            else if (Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(StaticMethod.MousePosition);
                if(tileSituation.isConnectedLine)
                {
                    break;
                }
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerManager.Instance.layerMask_hexagon))
                {
                    if (hit.transform != null)
                    {
                        Hexagon detectedHexagon = hit.transform.GetComponentInParent<Hexagon>();
                        if(detectedHexagon != null && (detectedHexagon.ParentStagePart ==this))
                        {
                            if(previousHexagon != detectedHexagon )
                            {
                                bool isNearDetect = detectedHexagon.nearHexagons.Contains(previousHexagon);
                                if (!isNearDetect)
                                {
                              //      Debug.Log("최근헥사곤" + detectedHexagon + "이 previous 헥사곤인 : " + previousHexagon + "랑 멀다");
                                    if (detectedHexagon.nowTileType == TileType.None)
                                    {
                                     
                                    }
                                    else
                                    {
                                        if (detectedHexagon.nowTileType == tileTypeToOpen)
                                        {
                                        //    Debug.Log("이 타입으로 이미 칠해진 땅임");
                                       //     Debug.Log("자기자신 라인 감지" + detectedHexagon.transform.name);
                                            UnlinkTill(tileSituation, tileSituation.nowHexagons.IndexOf(detectedHexagon), true);
                                            previousHexagon = detectedHexagon;
                                            SetActiveShiningEffectOnly(detectedHexagon);
                                            RefreshLines();
                                            vibrationManager.Vibrate();
                                        }
                                    }
                                }
                                else
                                {
                                    if (detectedHexagon.nowTileType != TileType.None && detectedHexagon.nowTileType != tileTypeToOpen)
                                    {
                                  //      Debug.Log("다른타입으로 이미 칠해진 땅임");
                                    }
                                    else if (detectedHexagon.IsEdgeHexagon && (detectedHexagon.initialTileType == tileTypeToOpen) && (detectedHexagon != initialHexagon))
                                    {
                                    //    Debug.Log("같은 라인의 다른 엣지헥사곤에 마무리 일격");
                                        detectedHexagon.SetStatus(tileTypeToOpen);
                                        previousHexagon = detectedHexagon;
                                        SetActiveShiningEffectOnly(detectedHexagon);
                                        RefreshLines();
                                        vibrationManager.Vibrate();
                                    }
                                    else
                                    {
                                        if (detectedHexagon.nowTileType == TileType.None)
                                        {
                                          //  Debug.Log("비어있는땅이어서 칠한다");
                                            detectedHexagon.SetStatus(tileTypeToOpen);
                                            previousHexagon = detectedHexagon;
                                            SetActiveShiningEffectOnly(detectedHexagon);
                                            RefreshLines();
                                            vibrationManager.Vibrate();
                                        }
                                        else
                                        {
                                            if (detectedHexagon.nowTileType == tileTypeToOpen)
                                            {
                                           //     Debug.Log("이 타입으로 이미 칠해진 땅임");
                                       //         Debug.Log("자기자신 라인 감지" + detectedHexagon.transform.name);
                                                UnlinkTill(tileSituation, tileSituation.nowHexagons.IndexOf(detectedHexagon), true);
                                                previousHexagon = detectedHexagon;
                                                SetActiveShiningEffectOnly(detectedHexagon);
                                                RefreshLines();
                                                vibrationManager.Vibrate();
                                            }
                                            else
                                            {
                                          //      Debug.Log("다른 타입으로 이미 칠해진 땅임");
                                            }
                                        }

                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        RefreshLines();
        SetActiveShiningEffectOnly(null);
        nowHexagonRoutine = null;
    }

    public void UnlinkTill(TileSituation tileSituation, int index, bool usePerformance)
    {
        if (tileSituation.nowHexagons.Count == 0)
        {
            return;
        }
        for (int i = tileSituation.nowHexagons.Count - 1; i > index; i--)
        {
            tileSituation.nowHexagons[i].SetStatus(TileType.None, usePerformance);
        }
    }
    /*
    public void UnlinkHexagon(Hexagon hexagonToUnlink, Hexagon hexgonLinkedFrom, bool usePerformance)
    {
        if(hexagonToUnlink.isStartHexagon || hexagonToUnlink.isLastHexagon)
        {
            Debug.Log("Unlinkable Hexagon");
            return;
        }
        TileSituation tileSituationToUnlink = GetTileSituation(hexagonToUnlink);
        hexagonToUnlink.nowLinkedFromHexagon = null;
        tileSituationToUnlink.allHexagons.Remove(hexagonToUnlink);
        hexgonLinkedFrom.isRootHexagon = true;
        hexagonToUnlink.isRootHexagon = false;
        hexagonToUnlink.SetStatus(TileType.None, usePerformance);
    }
    */
    
    public void SetActiveLinerRenderersOnly(TileType tileType)
    {
        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            TileSituation tileSituation = nowTileSituations[i];
            if (tileSituation.puzzleLineRenderer)
            {
                tileSituation.puzzleLineRenderer.SetVisible(tileSituation.tileType == tileType);
            }
        }
    }

    public IEnumerator InstallDecoObjsRoutine()
    {
        StageManager.Instance.MakeStep(StepName.Deco);
        SetCameraForDeco();
        yield return new WaitForSeconds(1f);
        ParticleManager.Instance.SetShineParticle(true);
        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            TileSituation tileSituation = nowTileSituations[i];
            if(tileSituation.tileType != TileType.None)
            {
               tileSituation.startHexagon.InstallBasicTileObjOnHexagon(0f);
                tileSituation.startHexagon.transform.ArokaTr().SetLocalRot(Quaternion.Euler(Vector3.right * 180f));
                tileSituation.startHexagon.edgeDisplayer.transform.ArokaTr().SetLocalScale(Vector3.zero, .5f);
                tileSituation.endHexagon.InstallBasicTileObjOnHexagon(0f);
                tileSituation.endHexagon.transform.ArokaTr().SetLocalRot(Quaternion.Euler(Vector3.right * 180f));
                tileSituation.endHexagon.edgeDisplayer.transform.ArokaTr().SetLocalScale(Vector3.zero, .5f);
            }
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            TileSituation tileSituation = nowTileSituations[i];
            if (tileSituation.tileType == TileType.None)
            {
                continue;
            }
            List<Hexagon> allHexagons = tileSituation.nowHexagons;
            List<Hexagon> mixedHexagons = allHexagons.MixUpHexagonList();
            for (int j = 0; j < mixedHexagons.Count; j++)
            {
                Hexagon hexagon = mixedHexagons[j];
                TileType tileType = hexagon.nowTileType;
                TilePlan tilePlan = ResourceManager.Instance.GetTilePlan(tileType);
                DecoObj decoObjToInstall = tilePlan.decoObjs.Count > 0 ? tilePlan.decoObjs[Random.Range(0, tilePlan.decoObjs.Count)] : null;
                if (decoObjToInstall == null)
                {
                    continue;
                }
                DecoObj inst_decoObj = Instantiate(decoObjToInstall.gameObject, transform).GetComponent<DecoObj>();
                inst_decoObj.InitializeDecoObj();
                inst_decoObj.transform.position = hexagon.transform.position + Vector3.up * 8f;
                inst_decoObj.graphicParent.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360f));

                List<Hexagon> rayHexagons = inst_decoObj.RayHexagons;
                if (!tileSituation.endingDecoInstalled  && inst_decoObj.InstallAvailable(tileType, rayHexagons))
                {
                    Debug.Log("설치성공");
                    inst_decoObj.InstallAtRayHexagons(true, rayHexagons);
                    inst_decoObj.transform.SetParent(transform);
                    inst_decoObj.transform.localScale = Vector3.zero;
                    inst_decoObj.transform.position = hexagon.transform.position + Vector3.up * 0.3f;

                    inst_decoObj.transform.ArokaTr().SetLocalScale(Vector3.one, 0.3f, CurvName.OverEaseOut);
                    ParticleManager.Instance.GetParticle(ParticleManager.ParticleTypeName.Paw).ParticleInstantiatePlay(inst_decoObj.transform.position);
                    tileSituation.endingDecoInstalled = true;
                    break;
                }
                else
                {
                    Debug.Log("설치실패");
                    inst_decoObj.gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(.5f);
        }
    }


    public void SetStagePartStatus(StagePartStatus stagePartStatus)
    {
        nowStagePartStatus = stagePartStatus;
        switch (stagePartStatus)
        {
            case StagePartStatus.Cleared:
                //전체 헥사곤 색깔 뿌옇게.
                break;
            case StagePartStatus.NotClearedYet:
                for (int i = 0; i < nowTileSituations.Count; i++)
                {
                    nowTileSituations[i].SetOnDisplayers(false, 0f);
                }
                //전체 헥사곤 색깔을 비활성화 색깔로
                break;
            case StagePartStatus.Playing:
                for (int i = 0; i < nowTileSituations.Count; i++)
                {
                    nowTileSituations[i].SetOnDisplayers(true, .5f); ;
                }
                Debug.LogWarning("발동");
                break;
        }
    }

    public void RefreshLines()
    {
        for(int i = 0; i < nowTileSituations.Count; i++)
        {
            nowTileSituations[i].RefreshLine();
        }

        for(int i = 0; i< totalHexagons.Count; i++)
        {
            Hexagon hexagon = totalHexagons[i];
            TileSituation tileSituation = GetTileSituation(totalHexagons[i].nowTileType);
            int index = tileSituation.nowHexagons.IndexOf(hexagon);
            int count = tileSituation.nowHexagons.Count;
            hexagon.graphicParent.ArokaTr().SetMeshColor(hexagon.GetProperColor(index, count), .2f);
            if (tileSituation.isConnectedLine)
            {
                if (!hexagon.IsEdgeHexagon)
                {
                    tileSituation.isTileObjInstalled = true;
                    Debug.Log("여기작동1" + hexagon);
                    hexagon.SetActiveOnlyTileObj(totalHexagons[i].nowTileType, Mathf.Lerp(0.1f, 1f, (float)index / tileSituation.nowHexagons.Count));
                }
                else
                {
                    tileSituation.isTileObjInstalled = false;
                    hexagon.SetActiveOnlyTileObj(TileType.None,0f);
                }
            }
            else
            {
                tileSituation.isTileObjInstalled = false;
                hexagon.SetActiveOnlyTileObj(TileType.None, 0f);
            }
        }
    }
    public void ResetStagePart()
    {
        for (int i = 0; i < nowTileSituations.Count; i++)
        {
            TileSituation tileSituation = nowTileSituations[i];
            tileSituation.Reset(true);
        }
        RefreshLines();
     //   recentDetectedHexagon = null;
    }
    public void SetCameraForStagePart()
    {
        List<Renderer> hexaRenderers = new List<Renderer>();
        for (int i = 0; i < totalHexagons.Count; i++)
        {
            Renderer renderer_i = totalHexagons[i].Rend;
            hexaRenderers.Add(renderer_i);
        }
        RenderersData renderersData = hexaRenderers.ToArray().GetRenderersData();
        float stagePartSizeX = renderersData.distX;
        float stagePartSizeY = renderersData.distY;
        float stagePartSizeZ = renderersData.distZ;
        Vector3 hexagonsCenter = renderersData.center;
        Vector3 cameraInitialPos = hexagonsCenter;
        Quaternion lookRotation = Quaternion.Euler(Vector3.right * 90);
        Debug.DrawRay(hexagonsCenter, Vector3.up * 500f, Color.red, 30f);
        Transform ingameCameraTr = StageManager.Instance.GetStepData(StepName.Puzzle).planProperty.cameraPlan.cameraDatas[0].transform;
        ingameCameraTr.transform.position = cameraInitialPos;
        ingameCameraTr.transform.rotation = lookRotation;
        Vector3 cameraDirection = Vector3.down.normalized;

        float distanceValue = Mathf.Clamp(stagePartSizeX * 7.5f, 80f, 500f);
        Debug.LogWarning(distanceValue);
        Vector3 topPos = renderersData.center.ModifiedY((-cameraDirection * (distanceValue)).y);

        ingameCameraTr.transform.position = topPos + Vector3.back * 20f;
        ingameCameraTr.transform.rotation = Quaternion.LookRotation(hexagonsCenter - ingameCameraTr.transform.position);
    }

    public void SetCameraForDeco()
    {
        List<Renderer> hexaRenderers = new List<Renderer>();
        for (int i = 0; i < totalHexagons.Count; i++)
        {
            Renderer renderer_i = totalHexagons[i].Rend;
            hexaRenderers.Add(renderer_i);
        }
        RenderersData renderersData = hexaRenderers.ToArray().GetRenderersData();
        float stagePartSizeX = renderersData.distX;
        float stagePartSizeY = renderersData.distY;
        float stagePartSizeZ = renderersData.distZ;
        Vector3 hexagonsCenter = renderersData.center;
        Vector3 cameraInitialPos = hexagonsCenter;
        Quaternion lookRotation = Quaternion.Euler(Vector3.right * 90);
        Debug.DrawRay(hexagonsCenter, Vector3.up * 500f, Color.red, 30f);
        Transform ingameCameraTr = StageManager.Instance.GetStepData(StepName.Deco).planProperty.cameraPlan.cameraDatas[0].transform;
        ingameCameraTr.transform.position = cameraInitialPos;
        ingameCameraTr.transform.rotation = lookRotation;
        Vector3 cameraDirection = Vector3.down.normalized;

        float distanceValue = Mathf.Clamp(stagePartSizeX * 2.5f, 20f, 500f);
        Debug.LogWarning(distanceValue);
        Vector3 topPos = renderersData.center.ModifiedY((-cameraDirection * (distanceValue)).y);

        ingameCameraTr.transform.position = topPos + Vector3.back * 20f + Vector3.up * 1.5f + Vector3.right * 10f;
        ingameCameraTr.transform.rotation = Quaternion.LookRotation(hexagonsCenter - ingameCameraTr.transform.position);
    }
}
