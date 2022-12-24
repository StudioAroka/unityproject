using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum StepName
{
    StagePartSelect,
    Puzzle,
    Deco
}
public class StageManager : MonoBehaviour
{
    #region singleTone
    private static StageManager _instance = null;
    public static StageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(StageManager)) as StageManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion


    [Title("PACKAGE")]
    public Transform notImprotantEnv;
    public Transform stageEnv;
    private bool isFirstMakeStepRoutineActivated;
    [HideInInspector]
    public Transform cameraPlanParent, uiPlanParent, lightPlanParent;
    [HideInInspector]
    public StepData nowStepData;
    public StepName NowStepName => nowStepData.stepName;
    public StepData[] stepDatas;

    [Title("STAGE")]
    public GameObject cloudPlane;

    [Title("PRESET")]
    public Stage[] stagePrefabs;
    public bool useTestStage;
    [ShowIf("useTestStage")]
    public int testStageIndex;

    [Title("RUNTIME")]
    public Stage nowStage;

    private void Awake()
    {
    }

    public void ConsistMap()
    {
        if (nowStage != null)
        {
            Destroy(nowStage.gameObject);
        }
#if !UNITY_EDITOR
        useTestStage = false;
#endif
        int stageIndex = useTestStage ? testStageIndex : (DataManager.Instance.StageIndex_Local % stagePrefabs.Length);
        nowStage = Instantiate(stagePrefabs[stageIndex].gameObject, stageEnv).GetComponent<Stage>();
        nowStage.InitializeStage();
        cloudPlane.transform.localPosition = Vector3.zero;


    }

    public IEnumerator LevelClearPerformanceRoutine()
    {
        yield return new WaitForSeconds(2f);
    }
    public void MakeStep(StepName stepName, bool forceExcutive = true)
    {
        if (!forceExcutive && (isFirstMakeStepRoutineActivated && NowStepName == stepName))
        {
            Debug.Log("같은 씬으로 이동하려는 시도");
            return;
        }
        isFirstMakeStepRoutineActivated = true;
        StepData targetStepData = GetStepData(stepName);
        nowStepData = targetStepData;
        GameManager.Instance.SetCamera(targetStepData);
        SetUI(stepName);
        GameManager.Instance.SetLight(targetStepData.planProperty.lightPlan);
    }
#region Static routines

    /*
    public IEnumerator GetInGameRoutine(StepName stepName)
    {
        switch (stepName)
        {
            default:
                return null;
        }
    }
    */
    public void SetUI(StepName stepName)
    {
        //REGISTER ALL SCENE OBJECTS
        List<GameObject> allSceneObjects = new List<GameObject>();
        for (int i = 0; i < stepDatas.Length; i++)
        {
            List<GameObject> sceneObjects = stepDatas[i].planProperty.uiPlan.sceneObjects;

            for (int j = 0; j < sceneObjects.Count; j++)
            {
                if (!allSceneObjects.Contains(sceneObjects[j]))
                {
                    allSceneObjects.Add(sceneObjects[j]);
                }
            }
        }
        StepData stepData = GetStepData(stepName);
        List<GameObject> targetGameObjects = stepData.planProperty.uiPlan.sceneObjects;
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            GameObject stepObject = allSceneObjects[i];
            bool isProper = targetGameObjects != null && targetGameObjects.Contains(stepObject);
            allSceneObjects[i].CustomSetActive(isProper);
        }
    }

    public StepData GetStepData(StepName stepName)
    {
        for (int i = 0; i < stepDatas.Length; i++)
        {
            if (stepDatas[i].stepName == stepName)
            {
                return stepDatas[i];
            }
        }
        return null;
    }
#endregion

    [Button("Refresh", ButtonSizes.Large)]
    public void Refresh()
    {
        AddCameras();
        AddUIs();
        AddLights();
    }
    void AddCameras()
    {
        if (cameraPlanParent == null)
        {
            cameraPlanParent = new GameObject("       카메라 플랜       ").transform;
        }
        cameraPlanParent.transform.SetParent(transform);
        cameraPlanParent.transform.localPosition = Vector3.zero;
        cameraPlanParent.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < stepDatas.Length; i++)
        {
            if (stepDatas[i].planProperty.cameraPlan == null)
            {
                CameraPlan cameraPlanToAdd = new GameObject("카메라 - " + stepDatas[i].stepName).AddComponent<CameraPlan>();
                cameraPlanToAdd.transform.SetParent(cameraPlanParent);
                cameraPlanToAdd.InitializeCameraPlan();
                cameraPlanToAdd.AddCameraData();
                stepDatas[i].planProperty.cameraPlan = cameraPlanToAdd;
            }
            else
            {
                stepDatas[i].planProperty.cameraPlan.gameObject.name = "카메라 - " + stepDatas[i].stepName;
            }
        }
    }

    void AddLights()
    {
        if (lightPlanParent == null)
        {
            lightPlanParent = new GameObject("       라이트 플랜      ").transform;
        }
        lightPlanParent.transform.SetParent(transform);
        lightPlanParent.transform.localPosition = Vector3.zero;
        lightPlanParent.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < stepDatas.Length; i++)
        {
            if (stepDatas[i].planProperty.lightPlan == null)
            {
                LightPlan lightPlanToAdd = new GameObject("Light - " + stepDatas[i].stepName).AddComponent<LightPlan>();
                lightPlanToAdd.transform.SetParent(lightPlanParent);
                lightPlanToAdd.InitializeLightPlan();
                stepDatas[i].planProperty.lightPlan = lightPlanToAdd;
            }
            else
            {
                stepDatas[i].planProperty.lightPlan.gameObject.name = "Light - " + stepDatas[i].stepName;
            }
        }
    }
    void AddUIs()
    {
        if (uiPlanParent == null)
        {
            uiPlanParent = new GameObject("       UI 플랜       ").transform;
        }
        uiPlanParent.transform.SetParent(transform);
        uiPlanParent.transform.localPosition = Vector3.zero;
        uiPlanParent.transform.localRotation = Quaternion.identity;

        for (int i = 0; i < stepDatas.Length; i++)
        {
            if (stepDatas[i].planProperty.uiPlan == null)
            {
                UIPlan uiPlanToAdd = new GameObject("UI - " + stepDatas[i].stepName).AddComponent<UIPlan>();
                uiPlanToAdd.transform.SetParent(uiPlanParent);
                uiPlanToAdd.InitializeUIPlan();
                stepDatas[i].planProperty.uiPlan = uiPlanToAdd;
            }
            else
            {
                stepDatas[i].planProperty.uiPlan.gameObject.name = "UI - " + stepDatas[i].stepName;
            }
        }
    }
    public void InitializeStepDatas()
    {
        for (int i = 0; i < stepDatas.Length; i++)
        {
            stepDatas[i].InitializeStepData();
        }
    }
    public Transform rotateLight;
    public void SetRotateLight(bool b, Vector3 worldPos, float totalTime)
    {
        rotateLight.transform.position = worldPos;
        RoateLight(b, totalTime);
    }

    Coroutine nowRoateLightRoutine;
    public void RoateLight(bool b, float totalTime)
    {
        if (nowRoateLightRoutine != null)
        {
            StopCoroutine(nowRoateLightRoutine);
        }
        nowRoateLightRoutine = StartCoroutine(RoateLightRoutine(b ? 1 : 0, totalTime));
    }
    public IEnumerator RoateLightRoutine(float f, float totalTime)
    {
        float accumTime = 0f;
        float initialAlpha = rotateLight.GetComponent<Renderer>().material.GetFloat("_Alpha");
        while (true)
        {
            accumTime += Time.deltaTime;

            float perone = accumTime / totalTime;
            float alpha = Mathf.Lerp(initialAlpha, f, perone);
            rotateLight.GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);

            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }
}