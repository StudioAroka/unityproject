using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ByteBrewSDK;
public class Stage : MonoBehaviour
{
    StageManager stageManager;
    [Title("PRESET")]


    [Title("AFTERBAKE")]
    public List<StagePart> stageParts;
    public Transform decoParent;
    [Title("RUNTIME")]
    public StagePart nowSelectedStagePart;

    public StagePart SmallestIndexUnclearedStagePart
    {
        get
        {
            for(int i = 0; i <stageParts.Count; i++)
            {
                if(stageParts[i].nowStagePartStatus == StagePartStatus.NotClearedYet)
                {
                    return stageParts[i];
                }
            }
            return null;
        }
    }

    public bool IsAllStagesAreCleared
    {
        get
        {
            for(int i = 0; i <stageParts.Count; i++)
            {
                if(stageParts[i].nowStagePartStatus != StagePartStatus.Cleared)
                {
                    return false;
                }
            }
            return true;
        }
    }
    private void Awake()
    {
        stageManager = StageManager.Instance;
    }
    public void InitializeStage()
    {
        CloudAlpha(false, 0f);
        ParticleManager.Instance.SetShineParticle(false);
        StageManager.Instance.SetRotateLight(false, Vector3.zero, 1f);
        for (int i = 0; i < stageParts.Count; i++)
        {
            stageParts[i].InitializeStagePart();
        }
        GiveGroupColor();

    }

    public void GiveGroupColor()
    {
        int randomMatIndex = Random.Range(0, ResourceManager.Instance.groupColors.Count);

        for (int i = 0; i < stageParts.Count; i++)
        {
            Debug.LogWarning(stageParts[i].totalHexagons.Count);
            int colorIndex = (randomMatIndex % ResourceManager.Instance.groupColors.Count);
            Color randColor = ResourceManager.Instance.groupColors[colorIndex];
            if (stageParts[i].nowStagePartStatus == StagePartStatus.NotClearedYet)
            {
                for (int j = 0; j < stageParts[i].totalHexagons.Count; j++)
                {
                    stageParts[i].totalHexagons[j].Rend.material.color = randColor;
                }
            }
            randomMatIndex++;
        }

    }
    private IEnumerator AwaitStagePartSelectRoutine()
    {
     //   Debug.LogWarning("AwaitStagePartSelectRoutine");
        for(int i = 0; i < stageParts.Count;i++)
        {
            bool uiShow = (stageParts[i] == SmallestIndexUnclearedStagePart);
            stageParts[i].nowStagePartSelectBtn.transform.ArokaTr().SetLocalScale(uiShow ? Vector3.one : Vector3.zero);
        }
        Camera mainCamera = GameManager.Instance.mainCamera;
        nowSelectedStagePart = null;
        while (true)
        {
            if(IsAllStagesAreCleared)
            {
                break;
            }
            if(nowSelectedStagePart != null)
            {
                break;
            }
            yield return null;
        }
        for (int i = 0; i < stageParts.Count; i++)
        {
            bool uiShow = false;
            stageParts[i].nowStagePartSelectBtn.transform.ArokaTr().SetLocalScale(uiShow ? Vector3.one : Vector3.zero);
        }
    }
    public IEnumerator InGameRoutine()
    {
        Debug.Log("InGameRoutine");
        UIManager.Instance.stagePartGuageBar.InitiailizeGuageBar(this);
        int fillCount = 0;
        UIManager.Instance.stagePartGuageBar.SetFill(fillCount);
        while (true)
        {
            if (IsAllStagesAreCleared)
            {
                Debug.Log("All Stage Parts are cleared");
                break;
            }
            string strForAnalytics = DataManager.Instance.StageIndexForUser + "_" + (fillCount + 1);
            ByteBrew.NewCustomEvent("Step_Started", strForAnalytics);

            ParticleManager.Instance.SetShineParticle(false);
            stageManager.MakeStep(StepName.StagePartSelect, false);
            stageManager.cloudPlane.transform.ArokaTr().SetLocalPos(Vector3.up * 1f, 1f);
            Debug.Log("StagePartSelect중입니다.");
            SetCameraForStageSelect();

            CloudAlpha(false, 1f);
            yield return StartCoroutine(UIManager.Instance.BlackPanelRoutine(0f, 1f));
            yield return StartCoroutine(AwaitStagePartSelectRoutine());
            nowSelectedStagePart.SetCameraForStagePart();
            Debug.Log("선퇵된 퍼즐로 Puzzle을 시작합니다.");
            StageManager.Instance.MakeStep(StepName.Puzzle, false);
            stageManager.cloudPlane.transform.ArokaTr().SetLocalPos(Vector3.up * 3.12f, 1f);
            CloudAlpha(true, 1f);

          
            yield return StartCoroutine(nowSelectedStagePart.StagePartRoutine()); 
            
            ByteBrew.NewCustomEvent("Step_Completed", strForAnalytics);

            yield return StartCoroutine(nowSelectedStagePart.InstallDecoObjsRoutine());
            yield return new WaitForSeconds(1.5f);
            ParticleManager.Instance.SetShineParticle(true);
            stageManager.SetRotateLight(true, nowSelectedStagePart.CenterTopPos.ModifiedY(5f), 1f);
            ComplimentManager.Instance.PlayComplimentRandom();
            fillCount++;
            UIManager.Instance.stagePartGuageBar.SetFill(fillCount);
            yield return StartCoroutine(nowSelectedStagePart.StagePartEndingRoutine());
        }
        ParticleManager.Instance.SetShineParticle(true);
        StageManager.Instance.SetRotateLight(true, transform.position, 1f);
        CloudAlpha(false, 1f);
    }

    Coroutine nowCloudAlphaRoutine;
    public void CloudAlpha(bool b, float totalTime = 0f)
    {
        if (nowCloudAlphaRoutine != null)
        {
            StopCoroutine(nowCloudAlphaRoutine);
        }

        if (totalTime == 0)
        {
            float targetAlpha = b ? 1 : 0;
            stageManager.cloudPlane.GetComponent<Renderer>().material.SetFloat("_Alpha", targetAlpha);
            return;
        }
       
        nowCloudAlphaRoutine = StartCoroutine(CloudAlphaRoutine(b, totalTime));
    }


    public IEnumerator CloudAlphaRoutine(bool b,float totalTime = 0f)
    {
        float accumTime = 0f;
        float initialAlpha = stageManager.cloudPlane.GetComponent<Renderer>().material.GetFloat("_Alpha");
        float targetAlpha = b ? 1 : 0;
       
        while (true)
        {
            accumTime += Time.deltaTime;
            float perone = accumTime / totalTime;
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, perone);
            stageManager.cloudPlane.GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
            
            if( perone >= 1)
            {
                break;
            }

            yield return null;
        }

    }

    public void SetCameraForStageSelect()
    {
        List<Renderer> hexaRenderers = new List<Renderer>();

        for (int i = 0; i < stageParts.Count; i++)
        {
            for (int j = 0; j < stageParts[i].totalHexagons.Count; j++)
            {
                Renderer renderer_j = stageParts[i].totalHexagons[j].Rend;
                hexaRenderers.Add(renderer_j);
            }
        }

        if (decoParent != null)
        {
            for (int j = 0; j < decoParent.transform.childCount; j++)
            {
                hexaRenderers.Add(decoParent.transform.GetChild(j).GetComponentInChildren<Renderer>());
            }
        }

        RenderersData renderersData = hexaRenderers.ToArray().GetRenderersData();
        float stagePartSizeX = renderersData.distX;
        float stagePartSizeY = renderersData.distY;
        float stagePartSizeZ = renderersData.distZ;
        Vector3 hexagonsCenter = renderersData.center;
        Vector3 cameraInitialPos = hexagonsCenter;
        Quaternion lookRotation = Quaternion.Euler(Vector3.right * 90);
        Debug.DrawRay(hexagonsCenter, Vector3.up * 500f, Color.red, 30f);
        Transform ingameCameraTr = StageManager.Instance.GetStepData(StepName.StagePartSelect).planProperty.cameraPlan.cameraDatas[0].transform;
        ingameCameraTr.transform.position = cameraInitialPos;
        ingameCameraTr.transform.rotation = lookRotation;
        Vector3 cameraDirection = Vector3.down.normalized;

        float distanceValue = Mathf.Clamp(stagePartSizeX * 2.5f, 20f, 500f);
        Debug.LogWarning(distanceValue);
        Vector3 topPos = renderersData.center.ModifiedY((-cameraDirection * (distanceValue)).y);

        ingameCameraTr.transform.position = topPos + Vector3.back * 24f + Vector3.right * 16f;
        ingameCameraTr.transform.rotation = Quaternion.LookRotation(hexagonsCenter - ingameCameraTr.transform.position);
    }
    [Button("WallScale", ButtonSizes.Gigantic)]
    public void SetWallScale()
    {
        for (int i = 0; i < stageParts.Count; i++)
        {

            if (stageParts[i].wallsParent != null)
            {
                for (int j = 0; j < stageParts[i].wallsParent.transform.childCount; j++)
                {
                    Transform tr = stageParts[i].wallsParent.transform.GetChild(j);
                    tr.transform.localScale = new Vector3(0.25f, 0.5f, 1f);
                    tr.transform.localPosition = tr.transform.localPosition.ModifiedY(2.25f);
                }
                stageParts[i].wallsParent.transform.localScale = Vector3.one;
                stageParts[i].wallsParent.transform.localPosition = Vector3.zero.ModifiedY(0f);
            }
        }
    }
    [Button("Rename", ButtonSizes.Gigantic)]
    public void Rename()
    {
        string newName = "Stage_ ";

        int stageValue = 0;
        int wallCount = 0;
        for (int i = 0; i < stageParts.Count; i++)
        {
            int lineCount = stageParts[i].lineDatas.Count;
            Debug.Log(stageParts[i].lineDatas[0].hexagons.Count);
            int stagePartValue = 0;

            for (int j = 0; j < lineCount; j++)
            {
                stagePartValue = stageParts[i].lineDatas[j].hexagons.Count * stageParts[i].lineDatas[j].hexagons.Count * lineCount;
            }
            stageValue += stagePartValue;
            if (stageParts[i].wallsParent != null)
                wallCount += stageParts[i].wallsParent.transform.childCount;
        }
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        float mapXsize = ((int)(renderers.GetRenderersData().distX * 10)) / 10f;
        float mapYsize = ((int)(renderers.GetRenderersData().distY * 10)) / 10f;

        Vector3 minXZ = renderers.GetRenderersData().minX.ModifiedZ(renderers.GetRenderersData().minZ.z).ModifiedY(0);
        Vector3 maxXZ = renderers.GetRenderersData().maxX.ModifiedZ(renderers.GetRenderersData().maxZ.z).ModifiedY(0);

        float massSize = (float)((int)(Vector3.Distance(minXZ, maxXZ)));
        //string path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
        // UnityEditor.AssetDatabase.RenameAsset(path,newName);
        // UnityEditor.AssetDatabase.Refresh();
        string lineString = stageValue >= 10 ? stageValue.ToString() : "0" + stageValue;
        string wallString = wallCount >= 10 ? wallCount.ToString() : "0" + wallCount;
        gameObject.name = newName + lineString + " , " + wallString + " / " + massSize;
    }

}
