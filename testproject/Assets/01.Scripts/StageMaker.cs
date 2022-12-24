using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum StageMakeMode
{
    None,
    SelectHexagons,
    Move
}

public class StageMaker : MonoBehaviour
{
    public StageMakeMode stageMakeMode;
    public GameObject hexagonPrefab;
    public GameObject nowField;
    public List<Hexagon> instHexagons;
    public List<Wall> instWalls;
    [BoxGroup("MakeField", centerLabel: true)]
    public Vector3Int fieldSize;
    [BoxGroup("MakeField", centerLabel: true)]
    public float hexagonDistance = 0;
    private Vector3 hexagonDist = new Vector3(0.95f, 1.1f, 1.1f);
    [BoxGroup("MakeField", centerLabel: true)]
    [Button("New Field", ButtonSizes.Large)]
    public void MakeField()
    {
        if (nowField != null)
        {
            DestroyImmediate(nowField.gameObject);
        }
        instHexagons.Clear();
        GameObject stageData = new GameObject("Stage_" + System.DateTime.Now);

        nowField = stageData.gameObject;
        for (int i = 0; i < fieldSize.x; i++)
        {
            for (int j = 0; j < fieldSize.z; j++)
            {
                Hexagon inst_Hexagon = Instantiate(hexagonPrefab, stageData.transform).GetComponent<Hexagon>();
                Vector3 distanceEach = hexagonDist + Vector3.one * hexagonDistance;
                Vector3 targetPos;
                float x = distanceEach.x * (i - ((fieldSize.x - 1) * 0.5f));
                float y = distanceEach.y;
                float z;
                if (i % 2 == 0)
                {
                    z = distanceEach.z * (j - ((fieldSize.z - 1) * 0.5f));
                }
                else
                {
                    z = distanceEach.z * (j - ((fieldSize.z - 1) * 0.5f)) + 1f * distanceEach.z * 0.5f;
                }
                targetPos = new Vector3(x, y, z);

                instHexagons.Add(inst_Hexagon);
                inst_Hexagon.transform.localPosition = targetPos;
                inst_Hexagon.coord = new Vector3Int(i, 0, j);
            }
        }

        for (int i = 0; i < instHexagons.Count; i++)
        {
            instHexagons[i].gameObject.name = "Hexa - ( " + instHexagons[i].coord + " )";

            List<Vector3Int> nearCoords = GetNearCoords(new Vector3Int(instHexagons[i].coord.x, 0, instHexagons[i].coord.z));
            List<Hexagon> nearHexagons = new List<Hexagon>();
            for (int k = 0; k < nearCoords.Count; k++)
            {
                Hexagon hexagon_i = GetHexagon(nearCoords[k]);
                if (hexagon_i != null)
                {
                    nearHexagons.Add(hexagon_i);
                }
            }
            instHexagons[i].nearHexagons = nearHexagons;
        }
        nowField.AddComponent<Stage>();
    }
    public List<Hexagon> nowSelectedHexagons = new List<Hexagon>();
    public Color outlineColor;
    Vector3 initialCameraPos;

    public void Start()
    {
        initialCameraPos = Camera.main.transform.position;
    }

    public List<Hexagon> GetNearHexagon(Hexagon standardHexagon)
    {
        List<Hexagon> nearHexagons = new List<Hexagon>();
        List<Vector3Int> nearCoords = GetNearCoords(standardHexagon.coord);
        for (int i = 0; i < nearCoords.Count; i++)
        {
            Hexagon hexagon_i = GetHexagon(nearCoords[i]);

            if (hexagon_i == null)
            {
                continue;
            }

            if (standardHexagon.GetComponent<Destination>().divisionTargets.Contains(hexagon_i))
            {
                continue;
            }

            if (hexagon_i.GetComponent<Destination>().clusionType == clusionType.Exclusion)
            {
                continue;
            }

            if (hexagon_i.GetComponent<Destination>().stagePartNum != standardHexagon.GetComponent<Destination>().stagePartNum)
            {
                continue;
            }

            nearHexagons.Add(hexagon_i);
        }

        return nearHexagons;
    }


    public List<Vector3Int> GetNearCoords(Vector3Int standardV3int)
    {
        List<Vector3Int> nearHexagonCoords = new List<Vector3Int>();
        nearHexagonCoords.Add(standardV3int + new Vector3Int(1, 0, 0));
        nearHexagonCoords.Add(standardV3int + new Vector3Int(-1, 0, 0));
        nearHexagonCoords.Add(standardV3int + new Vector3Int(0, 0, -1));
        nearHexagonCoords.Add(standardV3int + new Vector3Int(0, 0, 1));
        if (standardV3int.x % 2 == 0)
        {
            if (standardV3int.z % 2 == 0)
            {
                nearHexagonCoords.Add(standardV3int + new Vector3Int(-1, 0, -1));
                nearHexagonCoords.Add(standardV3int + new Vector3Int(1, 0, -1));
            }
            else
            {
                nearHexagonCoords.Add(standardV3int + new Vector3Int(-1, 0, -1));
                nearHexagonCoords.Add(standardV3int + new Vector3Int(1, 0, -1));
            }
        }
        else
        {
            if (standardV3int.z % 2 == 0)
            {
                nearHexagonCoords.Add(standardV3int + new Vector3Int(1, 0, 1));
                nearHexagonCoords.Add(standardV3int + new Vector3Int(-1, 0, 1));
            }
            else
            {
                nearHexagonCoords.Add(standardV3int + new Vector3Int(-1, 0, 1));
                nearHexagonCoords.Add(standardV3int + new Vector3Int(1, 0, 1));
            }
        }
        return nearHexagonCoords;
    }

    public Hexagon GetHexagon(Vector3Int coord)
    {
        for (int i = 0; i < instHexagons.Count; i++)
        {
            if (instHexagons[i].coord == coord)
            {
                return instHexagons[i];
            }
        }
        return null;
    }


    public Hexagon GetHitHexagon()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform != null)
            {
                Hexagon hexagon = hit.transform.GetComponentInParent<Hexagon>();
                if (hexagon != null)
                {
                    return hexagon;
                }
            }
        }
        return null;
    }

    public void SetOutLine(Hexagon hexagon, bool b)
    {
        if (b)
        {
            Outline outline = hexagon.Rend.gameObject.GetComponent<Outline>();

            if (outline == null)
            {
                outline = hexagon.Rend.gameObject.AddComponent<Outline>();
            }
            outline.enabled = true;
            outline.OutlineColor = outlineColor.ModifiedAlpha(1f);
            outline.OutlineWidth = 13f;
            outline.OutlineMode = Outline.Mode.OutlineAll;
        }
        else
        {
            Outline outline = hexagon.Rend.gameObject.GetComponent<Outline>();
            outline.enabled = false;
        }
    }

    public void AddSelect(Hexagon hexagon)
    {
        if (!nowSelectedHexagons.Contains(hexagon))
        {
            nowSelectedHexagons.Add(hexagon);
            SetOutLine(hexagon, true);
        }
    }

    public void RemoveSelect(Hexagon hexagon)
    {
        if (nowSelectedHexagons.Contains(hexagon))
        {
            nowSelectedHexagons.Remove(hexagon);
            SetOutLine(hexagon, false);
        }
    }

    public void ClearSelect()
    {
        for (int i = nowSelectedHexagons.Count - 1; i >= 0; i--)
        {
            RemoveSelect(nowSelectedHexagons[i]);
        }
    }

    Vector3 previousMousePos;
    Vector3 offset = Vector3.zero;
    public LineRenderer nowSelectedLine;
    public int nextStagePartIndex;
    public void RefreshLineRenderer()
    {
        List<Vector3> poses = new List<Vector3>();
        for (int i = 0; i < nowSelectedHexagons.Count; i++)
        {
            poses.Add(nowSelectedHexagons[i].transform.position.ModifiedY(0));
        }
        nowSelectedLine.positionCount = nowSelectedHexagons.Count;
        nowSelectedLine.SetPositions(poses.ToArray());
    }

    public void ShowStagePart(int stagePartNum)
    {
        ClearSelect();
        for (int i = 0; i < instHexagons.Count; i++)
        {
            bool enabled = (instHexagons[i].GetComponent<Destination>() != null) && (instHexagons[i].GetComponent<Destination>().stagePartNum == stagePartNum);
            if (enabled)
            {

                instHexagons[i].gameObject.SetActive(!instHexagons[i].gameObject.activeSelf);
            }
        }
    }



    public Wall GetResistWall(Hexagon ha, Hexagon hb)
    {
        for (int i = 0; i < instWalls.Count; i++)
        {
            if (instWalls[i].divideHexagons.Contains(ha))
            {
                if (instWalls[i].divideHexagons.Contains(hb))
                {
                    return instWalls[i];
                }
            }
        }
        return null;
    }

    public List<LineData> lineDatas = new List<LineData>();

    public void AddLineData(TileType tileType)
    {

        if (GetLineData(nowSelectedHexagons) == null)
        {
            LineData lineData = new LineData();
            lineData.tileType = tileType;
            lineData.hexagons.AddRange(nowSelectedHexagons);
            lineDatas.Add(lineData);

        }
        else
        {
            LineData lineData = GetLineData(nowSelectedHexagons);
            lineData.tileType = tileType;
            lineData.hexagons.Clear();
            lineData.hexagons = new List<Hexagon>();
            lineData.hexagons.AddRange(nowSelectedHexagons);
        }
    }

    public LineData GetLineData(List<Hexagon> hexagons)
    {
        for (int i = 0; i < lineDatas.Count; i++)
        {
            for (int j = 0; j < nowSelectedHexagons.Count; j++)
            {
                if (lineDatas[i].hexagons.Contains(nowSelectedHexagons[j]))
                {
                    return lineDatas[i];
                }
            }
        }
        return null;
    }

    public LineData GetLineData(Hexagon hexagon)
    {
        for (int i = 0; i < lineDatas.Count; i++)
        {
            if (lineDatas[i].hexagons.Contains(hexagon))
            {
                return lineDatas[i];
            }
        }
        return null;
    }
    bool isDoubleClick;
    public Coroutine nowDoubleClickRoutine;
    public void DoubleClickTimer()
    {
        if (nowDoubleClickRoutine != null)
        {
            StopCoroutine(nowDoubleClickRoutine);

        }
        nowDoubleClickRoutine = StartCoroutine(DoubleClickTimerRoutine());
    }

    public IEnumerator DoubleClickTimerRoutine()
    {
        float totalTime = 0.2f;
        isDoubleClick = true;
        yield return new WaitForSeconds(totalTime);
        isDoubleClick = false;
    }

    public void SetTileTypeTotal(TileType tileType)
    {
        AddLineData(tileType);
        nowSelectedHexagons.SetTileType(tileType);
        ClearSelect();
    }

    private void Update()
    {
        #region Select Hexagon
        if (Input.GetMouseButton(0))
        {
            Hexagon hitHexagon = GetHitHexagon();
            if (hitHexagon != null)
            {
                AddSelect(hitHexagon);
                if (Input.GetMouseButtonDown(0))
                {
                    if (isDoubleClick)
                    {
                        LineData lineData = GetLineData(hitHexagon);
                        if (lineData != null)
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {

                            }
                            else
                            {
                                ClearSelect();
                            }
                            for (int i = 0; i < lineData.hexagons.Count; i++)
                            {
                                AddSelect(lineData.hexagons[i]);
                            }
                        }
                    }
                    else
                    {
                        DoubleClickTimer();
                    }
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            Hexagon hitHexagon = GetHitHexagon();
            RemoveSelect(hitHexagon);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelect();
        }
        #endregion

        #region Divide
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (nowSelectedHexagons.Count == 2)
            {
                if (GetResistWall(nowSelectedHexagons[0], nowSelectedHexagons[1]) == null)
                {

                    Wall instWall = Instantiate(ResourceManager.Instance.wallPrefab.gameObject, nowField.transform).GetComponent<Wall>();
                    instWall.divideHexagons.Add(nowSelectedHexagons[0]);
                    instWall.divideHexagons.Add(nowSelectedHexagons[1]);
                    instWall.transform.position = Vector3.Lerp(nowSelectedHexagons[0].transform.position, nowSelectedHexagons[1].transform.position, 0.5f);
                    Vector3 direction = (nowSelectedHexagons[0].transform.position - nowSelectedHexagons[1].transform.position);
                    instWall.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(Vector3.up * 90);
                    instWalls.Add(instWall);
                }
                else
                {
                    Wall theWall = GetResistWall(nowSelectedHexagons[0], nowSelectedHexagons[1]);
                    instWalls.Remove(theWall);
                    Destroy(theWall.gameObject);
                }
                ClearSelect();
            }
        }
        #endregion

        #region Delete
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            for (int i = 0; i < nowSelectedHexagons.Count; i++)
            {
                LineData lineData = GetLineData(nowSelectedHexagons[i]);
                if (lineData != null)
                {
                    lineData.hexagons.Remove(nowSelectedHexagons[i]);
                }
            }
            nowSelectedHexagons.SetExclusion();

            ClearSelect();
        }
        #endregion

        #region SetStagePart
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                nowSelectedHexagons.SetStagePart(0);
                ClearSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                nowSelectedHexagons.SetStagePart(1);
                ClearSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                nowSelectedHexagons.SetStagePart(2);
                ClearSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                nowSelectedHexagons.SetStagePart(3);
                ClearSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                nowSelectedHexagons.SetStagePart(4);
                ClearSelect();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowStagePart(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ShowStagePart(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShowStagePart(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShowStagePart(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShowStagePart(4);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                ClearSelect();
                for (int i = 0; i < instHexagons.Count; i++)
                {
                    instHexagons[i].gameObject.SetActive(true);
                }
            }
        }
        #endregion

        #region SetTileType
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetTileTypeTotal(TileType.Sea);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SetTileTypeTotal(TileType.Snow);

        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SetTileTypeTotal(TileType.Village);

        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            SetTileTypeTotal(TileType.Forest);

        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            SetTileTypeTotal(TileType.Desert);

        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            SetTileTypeTotal(TileType.Volcano);
        }
        #endregion

        #region SetDecoration
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                LineData lineData = GetLineData(nowSelectedHexagons[0]);
                lineData.isDecorationLine = true;
                nowSelectedHexagons.SetDecoration();
                ClearSelect();
            }
        }
        #endregion

        #region CameraControll
        float cameraY = Camera.main.transform.position.y;
        cameraY += Input.GetAxis("Mouse ScrollWheel") * 15f;
        Vector3 targetPos = Camera.main.transform.position.ModifiedY(cameraY);
        if (Input.GetMouseButton(2))
        {
            Vector3 nowMousePos = Input.mousePosition;
            Vector3 direction = (previousMousePos - nowMousePos).normalized * 1f;
            offset = new Vector3(direction.x, 0, direction.y);
            previousMousePos = Input.mousePosition;
        }
        else
        {
            offset = Vector3.zero;
            previousMousePos = Input.mousePosition;
        }
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos + offset, Time.deltaTime * 50f);
        #endregion

        #region Save
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ChangeToStage();
            }
        }
        #endregion

        RefreshLineRenderer();

    }

    public bool IsSaveAvailable()
    {
        for (int i = 0; i < instHexagons.Count; i++)
        {
            Hexagon hexagon_i = instHexagons[i];
            Destination destination_i = hexagon_i.gameObject.GetComponent<Destination>();

            if (destination_i == null)
            {
                Debug.LogWarning("Destination 없음");
                Debug.LogWarning(hexagon_i + " 포함 여부의 운명이 정해지지 않음, stagePartNum, TileType, 등");

                return false;
            }

            if (destination_i.clusionType == clusionType.Inclusion)
            {
                if (destination_i.stagePartNum < 0)
                {
                    if (!destination_i.isDecoration)
                    {
                        Debug.LogWarning(hexagon_i + " stagePartNum");
                        return false;
                    }
                }
            }
            else if (destination_i.clusionType == clusionType.None)
            {
                Debug.LogWarning(hexagon_i + " 포함 여부의 운명이 정해지지 않음");
                return false;
            }
        }
        return true;
    }

    public void ChangeToStage()
    {
        bool isReadyToChange = IsSaveAvailable();

        Debug.Log("프리팹화가 될수 있는지? : " + isReadyToChange);

        if (!isReadyToChange)
        {
            Debug.LogWarning("프리팹화 조건 불충족");
            return;
        }

        nowField.name = "Stage_" + System.DateTime.Now;

        for (int i = instHexagons.Count - 1; i >= 0; i--)
        {
            if (instHexagons[i].GetComponent<Destination>().clusionType == clusionType.Exclusion)
            {
                Destroy(instHexagons[i].gameObject);
                instHexagons.RemoveAt(i);
            }
        }




        List<Hexagon> decoHexagons = new List<Hexagon>();
        for (int i = instHexagons.Count - 1; i >= 0; i--)
        {
            if (instHexagons[i].GetComponent<Destination>().clusionType == clusionType.Inclusion)
            {
                if (instHexagons[i].GetComponent<Destination>().isDecoration)
                {
                    instHexagons[i].isDecorationHexagon = true;
                    instHexagons[i].transform.SetParent(nowField.transform);
                    decoHexagons.Add(instHexagons[i]);
                    instHexagons.RemoveAt(i);
                }
            }
        }


        int tmpStagePartNum = 0;
        while (true)
        {
            List<Hexagon> hexagons_is = FindHexgons_StagePartNum(tmpStagePartNum);
            if (hexagons_is.Count == 0)
            {
                break;
            }
            GameObject stagetPartObj = new GameObject("StagePart " + tmpStagePartNum);
            stagetPartObj.transform.SetParent(nowField.transform);
            StagePart stagePart = stagetPartObj.AddComponent<StagePart>();
            nowField.GetComponent<Stage>().stageParts.Add(stagetPartObj.GetComponent<StagePart>());
            for (int i = 0; i < hexagons_is.Count; i++)
            {
                LineData lineData = GetLineData(hexagons_is[i]);
                if (!stagePart.lineDatas.Contains(lineData))
                {
                    stagePart.lineDatas.Add(lineData);
                }
               
                hexagons_is[i].transform.SetParent(stagetPartObj.transform);
            }
            tmpStagePartNum++;
        }

        GameObject decorationParent = new GameObject("DecorationParent -1");
        decorationParent.transform.SetParent(nowField.transform);
        decorationParent.transform.localPosition = Vector3.zero;
        Stage nowStage = nowField.GetComponent<Stage>();
        nowStage.decoParent = decorationParent.transform;
        for (int i = 0; i < decoHexagons.Count; i++)
        {
            Hexagon hexagon = decoHexagons[i];
            Destination destination = decoHexagons[i].GetDestination();
            if(destination.stagePartNum == -1)
            {
                hexagon.transform.SetParent(decorationParent.transform);
                Debug.LogWarning("설정된 스테이지파트가 없는 데코");
            }
            else
            {
                StagePart stagePart = nowStage.stageParts[destination.stagePartNum];
                Transform decorationParent2 = stagePart.decoHexagonsParent;
                if (stagePart.decoHexagonsParent == null)
                {
                    decorationParent2 = new GameObject("DecoHexagonParent " + destination.stagePartNum).transform;
                    decorationParent2.transform.SetParent(stagePart.transform);
                    decorationParent2.transform.localPosition = Vector3.zero;
                    stagePart.decoHexagonsParent = decorationParent2;
                    Debug.LogWarning("새로운 StagePartDecoParent 생성");
                }
                hexagon.transform.SetParent(decorationParent2);
            }
        }

        for (int i = 0; i < instWalls.Count; i++)
        {
            if (instWalls[i].divideHexagons[0].ParentStagePart.wallsParent == null)
            {
                GameObject wallsParent = new GameObject("WallsParent");
                wallsParent.transform.SetParent(instWalls[i].divideHexagons[0].ParentStagePart.transform);
                wallsParent.transform.localPosition = Vector3.up * -2.4f;
                instWalls[i].divideHexagons[0].ParentStagePart.wallsParent = wallsParent.transform;
            }
            instWalls[i].transform.SetParent(instWalls[i].divideHexagons[0].ParentStagePart.wallsParent.transform);
            Division(instWalls[i].divideHexagons[0], instWalls[i].divideHexagons[1]);
            Division(instWalls[i].divideHexagons[1], instWalls[i].divideHexagons[0]);
        }


        for (int i = 0; i < instHexagons.Count; i++)
        {
            instHexagons[i].Rend.sharedMaterial = ResourceManager.Instance.GetTilePlan(instHexagons[i].GetDestination().tileType).tileMat;
            instHexagons[i].nearHexagons = GetNearHexagon(instHexagons[i]);
            instHexagons[i].nowTileType = instHexagons[i].GetDestination().tileType;
        }

        for (int i = 0; i < decoHexagons.Count; i++)
        {
            Destroy(decoHexagons[i].GetComponentInChildren<Outline>());
            Destroy(decoHexagons[i].GetDestination().decorationSymbol);
            Destroy(decoHexagons[i].GetComponent<Destination>());
        }

        for (int i = 0; i < instHexagons.Count; i++)
        {
            Destroy(instHexagons[i].GetComponent<Destination>());
            Destroy(instHexagons[i].GetComponentInChildren<Outline>());
        }


       

        for (int i = 0; i < nowStage.stageParts.Count; i++)
        {
            List<Hexagon> hexagons = new List<Hexagon>();
            hexagons.AddRange(nowStage.stageParts[i].transform.GetComponentsInChildren<Hexagon>());
            
            List<Renderer> renderers = new List<Renderer>();
            for (int j = 0; j < hexagons.Count; j++)
            {
                renderers.Add(hexagons[j].Rend);
            }

            RenderersData renderersData = renderers.ToArray().GetRenderersData();
            float xSize = renderersData.distX;
            float zSize = renderersData.distZ;

            Debug.LogWarning("맵크기 = " + "x = " + xSize + " / " + " z = " + zSize);
            bool isHorizontalMap = xSize >= zSize;
            nowStage.stageParts[i].raiseUpDegree = isHorizontalMap ? Vector3.up * 270 : Vector3.up * 360f;
        }
    }




    public void Division(Hexagon hexagon_a, Hexagon hexagon_b)
    {
        Destination des_a = hexagon_a.GetDestination();
        Destination des_b = hexagon_b.GetDestination();
        if (!des_a.divisionTargets.Contains(hexagon_b))
        {
            des_a.AddDivisionTarget(hexagon_b);
        }
    }

    public List<Hexagon> FindHexgons_StagePartNum(int num)
    {

        List<Hexagon> results = new List<Hexagon>();
        for (int i = 0; i < instHexagons.Count; i++)
        {
            if (instHexagons[i].GetComponent<Destination>().stagePartNum == num)
            {
                results.Add(instHexagons[i]);
            }
        }
        return results;
    }
}

/*    
if (Input.GetMouseButtonDown(0))
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
    {
        if (hit.transform != null)
        {
            Color color = Random.ColorHSV();
            Hexagon hexagonData = hit.transform.GetComponentInParent<Hexagon>();
            hexagonData.GetComponentInChildren<Renderer>().material.color = color;
            for (int i = 0; i < hexagonData.nearHexagons.Count; i++)
            {
                Hexagon nearHexagonData_i = hexagonData.nearHexagons[i];
                if(nearHexagonData_i != null)
                {
                    nearHexagonData_i.GetComponentInChildren<Renderer>().material.color = color;
                }
            }
        }
    }
}*/