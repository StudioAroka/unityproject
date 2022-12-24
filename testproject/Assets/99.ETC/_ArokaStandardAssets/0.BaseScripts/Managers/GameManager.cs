using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ByteBrewSDK;
using com.adjust.sdk;

public enum SceneName
{
    Initialize = 0,
    StageSelect = 1,
    Main = 2,
    InGame = 3,
    LevelClear = 4,
    LevelClearAfter = 5,
    LevelFail = 6,
    LevelFailAfter = 7
}

[System.Serializable]
public class PlanProperty
{
    public bool UseDynamicLight => GameManager.Instance.useDynamicLight;
    public CameraPlan cameraPlan;
    public UIPlan uiPlan;
    [ShowIf("UseDynamicLight")]
    public LightPlan lightPlan;
}
[System.Serializable]
public class SceneData
{
    public bool UseDynamicLight => GameManager.Instance.useDynamicLight;
    [PropertySpace(SpaceBefore = 30, SpaceAfter = 10)]
    public SceneName sceneName;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 30)]
    public PlanProperty planProperty;

    public void InitializeSceneData()
    {
        planProperty.cameraPlan.InitializeCameraPlan();
    }
}
[System.Serializable]
public class StepData
{
    public bool UseDynamicLight => GameManager.Instance.useDynamicLight;
    [PropertySpace(SpaceBefore = 30, SpaceAfter = 10)]
    public StepName stepName;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 30)]
    public PlanProperty planProperty;

    public GameObject stepObjPrefab;

    public GameObject nowStepObj;

    public void InitializeStepData()
    {
        planProperty.cameraPlan.InitializeCameraPlan();

        if(nowStepObj != null)
        {
            GameObject.Destroy(nowStepObj.gameObject);
        }
        if(stepObjPrefab != null)
        {
            nowStepObj = GameObject.Instantiate(stepObjPrefab);
            nowStepObj.transform.SetParent(StageManager.Instance.stageEnv);
        }
    }
}
public class GameManager : MonoBehaviour
{
    #region singleTone
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    [Title("PRESET")]

    public Light mainLight;
    public bool useDynamicLight;
    public SceneData[] sceneDatas;

    [Title("RUNTIME")]


    public Camera mainCamera;
    private Transform cameraPlanParent, uiPlanParent, lightPlanParent;
    public SceneData nowSceneData;
    public SceneName NowSceneName => nowSceneData.sceneName;
    private Coroutine nowCameraRoutine;
    private Coroutine nowLightRoutine;


    public SceneData GetSceneData(SceneName sceneName)
    {
        for(int i = 0; i < sceneDatas.Length; i++)
        {
            if(sceneDatas[i].sceneName == sceneName)
            {
                return sceneDatas[i];
            }
        }
        return null;
    }

    bool isFirstMakeSceneRoutineActivated;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Start()
    {
        Application.targetFrameRate = 300;


       ByteBrew.InitializeByteBrew();
       MaxSdk.SetSdkKey("olpVA6G1ZapDqK5ZuvOiN_PZOlPfi9xRPBq9ak9-BSaXM1modzs5l3Zh20s1_ZmVxWXf9NJGZJkapi2AyiLe_v");
       MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
       MaxSdk.SetVerboseLogging(true);
       MaxSdk.InitializeSdk(); 
       MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
           // Show Mediation Debugger
           //  MaxSdk.ShowMediationDebugger();
       };
#if UNITY_IOS
       InitAdjust("5ctjtqxtfugw");
#elif UNITY_ANDROID
        InitAdjust("5ctjtqxtfugw");
#endif
        MakeScene(SceneName.Initialize);

    }

    [Button("MakeScene Test", ButtonSizes.Large)]
    public void MakeSceneTest(SceneName sceneName)
    {
        MakeScene(sceneName);
    }


    public void MakeScene(SceneName sceneName)
    {
        if(isFirstMakeSceneRoutineActivated && NowSceneName == sceneName)
        {
            Debug.Log("같은 씬으로 이동하려는 시도");
            return;
        }
        InitializeSceneDatas();
        Debug.Log("씬으로 진입하였습니다 : " + sceneName);
        isFirstMakeSceneRoutineActivated = true;
        nowSceneData = GetSceneData(sceneName);
        SetCamera(nowSceneData);
        SetUI(sceneName);
        SetLight(nowSceneData.planProperty.lightPlan);
        UIManager.Instance.RefreshAll();
        StartCoroutine(MakeSceneRoutine(sceneName));
    }

    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename
        // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters
        //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
        //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
        //});
        Adjust.start(adjustConfig);
    }
    IEnumerator MakeSceneRoutine(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.Initialize:
                yield return StartCoroutine(UIManager.Instance.BlackPanelRoutine(1f, 1f));
                StageManager.Instance.stageEnv.DestroyAllChildren();
                StageManager.Instance.notImprotantEnv.DestroyAllChildren();
                UIManager.Instance.stagePartSelectBtnParent.DestroyAllChildren();
                System.GC.Collect();
                MakeScene(SceneName.StageSelect);
                break;
            case SceneName.StageSelect:
                ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Started, "Level_Start", "Level" + DataManager.Instance.StageIndexForUser);
                StageManager.Instance.ConsistMap();
                MakeScene(SceneName.Main);
                break;
            case SceneName.Main:
                MakeScene(SceneName.InGame);
                break;
            case SceneName.InGame:
                yield return StartCoroutine(StageManager.Instance.nowStage.InGameRoutine());
                MakeScene(SceneName.LevelClear);
                break;
            case SceneName.LevelClear:
                ByteBrew.NewProgressionEvent(ByteBrewProgressionTypes.Completed, "Level_Completed", "Level" + DataManager.Instance.StageIndexForUser);
                DataManager.Instance.StageIndex_Local++;
                break;
            case SceneName.LevelClearAfter:
                yield return StartCoroutine(UIManager.Instance.BlackPanelRoutine(1f, 1f));
                MakeScene(SceneName.Initialize);
                break;
            case SceneName.LevelFail:
                break;
            case SceneName.LevelFailAfter:
                break;
            default:
                break;
        }
        yield return null;
    }
    private void SetCamera(SceneData sceneData)
    {
        if(nowCameraRoutine != null)
        {
            StopCoroutine(nowCameraRoutine);
         }
        nowCameraRoutine = StartCoroutine(SceneCameraRoutine(sceneData));
    }
    public void SetCamera(StepData stepData)
    {
        if (nowCameraRoutine != null)
        {
            StopCoroutine(nowCameraRoutine);
        }
        nowCameraRoutine = StartCoroutine(StepCameraRoutine(stepData));
    }
    public void SetLight(LightPlan lightPlan)
    {
        if(!useDynamicLight)
        {
            return;
        }
        if(nowLightRoutine != null)
        {
            StopCoroutine(nowLightRoutine);
        }
        nowLightRoutine = StartCoroutine(LightRoutine(lightPlan));
    }
    IEnumerator LightRoutine(LightPlan lightPlan)
    {
        if(lightPlan.useUpdateFollow)
        {
            while (true)
            {
                float deltime = Time.deltaTime;
                Vector3 targetPos = lightPlan.transform.position;
                Quaternion targetRot = lightPlan.transform.rotation;
                mainLight.transform.SetPositionAndRotation(Vector3.Lerp(mainLight.transform.position, targetPos, lightPlan.lightPosSpeed * deltime), Quaternion.Lerp(mainLight.transform.rotation, targetRot, lightPlan.lightRotSpeed * deltime));
                yield return null;
            }
        }
        else
        {
            mainLight.transform.ArokaTr().SetTransform(new TransformData(lightPlan.transform), lightPlan.totalTime, lightPlan.curvName);
        }
    }
    IEnumerator SceneCameraRoutine(SceneData sceneData)
    {
        SceneName sceneName = sceneData.sceneName;

        while (true)
        {
            float deltime = Time.deltaTime;
            Vector3 finalPos;
            Quaternion finalRot;

            CameraData cameraDataToUse = sceneData.planProperty.cameraPlan.NowCameraData;
            switch (sceneName)
            {
                default:
                    CameraData cameraData = StageManager.Instance.GetStepData(StepName.StagePartSelect).planProperty.cameraPlan.NowCameraData;
                    Vector3 targetPos_s2 = cameraData.transform.position;
                    Quaternion targetRot_s2 = cameraData.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos_s2, cameraData.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot_s2, cameraData.cameraRotSpeed * deltime);
                    break;
                    /*
                default:
                    Vector3 targetPos_s = cameraDataToUse.transform.position;
                    Quaternion targetRot_s = cameraDataToUse.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos_s, cameraDataToUse.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot_s, cameraDataToUse.cameraRotSpeed * deltime);
                    break;
                    */
            }
            mainCamera.transform.SetPositionAndRotation(finalPos, finalRot);
            if (cameraDataToUse.useDynamicFov)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraDataToUse.targetFov, cameraDataToUse.fovSpeed * deltime);
            yield return null;
        }
    }
    IEnumerator StepCameraRoutine(StepData stepData)
    {
        StepName stepName = stepData.stepName;

        StagePart stagePart = StageManager.Instance.nowStage.nowSelectedStagePart;
        while (true)
        {
            CameraData cameraDataToUse = stepData.planProperty.cameraPlan.NowCameraData;
            float deltime = Time.deltaTime;
            Vector3 finalPos;
            Quaternion finalRot;

            switch(stepName)
            {
                case StepName.StagePartSelect:
                    Vector3 targetPos_i = cameraDataToUse.transform.position;
                    Quaternion targetRot_i = cameraDataToUse.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos_i, cameraDataToUse.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot_i, cameraDataToUse.cameraRotSpeed * deltime);
                    break;
                case StepName.Puzzle:
                    Vector3 targetPos_p = cameraDataToUse.transform.position;
                    Quaternion targetRot_p =cameraDataToUse.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos_p, cameraDataToUse.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot_p, cameraDataToUse.cameraRotSpeed * deltime);
                    break;
                case StepName.Deco:
                    Vector3 targetPos_d = cameraDataToUse.transform.position;
                    Quaternion targetRot_d = cameraDataToUse.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos_d, cameraDataToUse.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot_d, cameraDataToUse.cameraRotSpeed * deltime);
                    break;
                default:
                    Vector3 targetPos = cameraDataToUse.transform.position;
                    Quaternion targetRot = cameraDataToUse.transform.rotation;
                    finalPos = Vector3.Lerp(mainCamera.transform.position, targetPos, cameraDataToUse.cameraPosSpeed * deltime) + cameraShakeOffset;
                    finalRot = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, cameraDataToUse.cameraRotSpeed * deltime);
                    break;
            }
            mainCamera.transform.SetPositionAndRotation(finalPos, finalRot);
            if(cameraDataToUse.useDynamicFov)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, cameraDataToUse.targetFov, cameraDataToUse.fovSpeed * deltime);
            yield return null;
        }
    }
    public void SetUI(SceneName sceneName)
    {
        //REGISTER ALL SCENE OBJECTS
        List<GameObject> allSceneObjects = new List<GameObject>();
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            List<GameObject> sceneObjects = sceneDatas[i].planProperty.uiPlan.sceneObjects;

            for (int j = 0; j < sceneObjects.Count; j++)
            {
                if (!allSceneObjects.Contains(sceneObjects[j]))
                {
                    allSceneObjects.Add(sceneObjects[j]);
                }
            }
        }
        SceneData sceneData = GetSceneData(sceneName);
        List<GameObject> targetGameObjects = sceneData.planProperty.uiPlan.sceneObjects;
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            GameObject stepObject = allSceneObjects[i];
            bool isProper = targetGameObjects != null && targetGameObjects.Contains(stepObject);
            allSceneObjects[i].CustomSetActive(isProper);
        }
    }
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

        for (int i = 0; i < sceneDatas.Length; i++)
        {
            if (sceneDatas[i].planProperty.cameraPlan == null)
            {
                CameraPlan cameraPlanToAdd = new GameObject("카메라 - " + sceneDatas[i].sceneName).AddComponent<CameraPlan>();
                cameraPlanToAdd.transform.SetParent(cameraPlanParent);
                cameraPlanToAdd.InitializeCameraPlan();
                cameraPlanToAdd.AddCameraData();
                sceneDatas[i].planProperty.cameraPlan = cameraPlanToAdd;
            }
            else
            {
                sceneDatas[i].planProperty.cameraPlan.gameObject.name = "카메라 - " + sceneDatas[i].sceneName;
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

        for (int i = 0; i < sceneDatas.Length; i++)
        {
            if (sceneDatas[i].planProperty.lightPlan == null)
            {
                LightPlan lightPlanToAdd = new GameObject("Light - " + sceneDatas[i].sceneName).AddComponent<LightPlan>();
                lightPlanToAdd.transform.SetParent(lightPlanParent);
                lightPlanToAdd.InitializeLightPlan();
                sceneDatas[i].planProperty.lightPlan = lightPlanToAdd;
            }
            else
            {
                sceneDatas[i].planProperty.lightPlan.gameObject.name = "Light - " + sceneDatas[i].sceneName;
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

        for (int i = 0; i < sceneDatas.Length; i++)
        {
            if (sceneDatas[i].planProperty.uiPlan == null)
            {
                UIPlan uiPlanToAdd = new GameObject("UI - " + sceneDatas[i].sceneName).AddComponent<UIPlan>();
                uiPlanToAdd.transform.SetParent(uiPlanParent);
                uiPlanToAdd.InitializeUIPlan();
                sceneDatas[i].planProperty.uiPlan = uiPlanToAdd;
            }
            else
            {
                sceneDatas[i].planProperty.uiPlan.gameObject.name = "UI - " + sceneDatas[i].sceneName;
            }
        }
    }

    void InitializeSceneDatas()
    {
        for (int i = 0; i < sceneDatas.Length; i++)
        {
            sceneDatas[i].InitializeSceneData();
        }
    }
    #region AWAIT ROUTINE
    public IEnumerator AwaitMouseButtonDownRoutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }
            yield return null;
        }
    }
    public IEnumerator AwaitSpaceKeyRoutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }
            yield return null;
        }
        yield return null;
    }
    public IEnumerator AwaitGetKeyDownRoutine(KeyCode keycode)
    {
        while (true)
        {
            if (Input.GetKeyDown(keycode))
            {
                break;
            }
            yield return null;
        }
    }
    #endregion
    #region CAMERA SHAKING ROUTINE
    public Coroutine nowCameraShakingRoutine;
    public Vector3 cameraShakeOffset;
    public void CameraShake(float shakeRadius = .1f, float shakeSec = .15f)
    {
        if (nowCameraShakingRoutine != null)
        {
            StopCoroutine(nowCameraShakingRoutine);
        }
        nowCameraShakingRoutine = StartCoroutine(CameraShakeRoutine(shakeRadius, shakeSec));
    }
    IEnumerator CameraShakeRoutine(float shakeRadius, float shakeSec)
    {
        float deltime = 0;
        while (true)
        {
            deltime += Time.deltaTime;
            float perone = deltime / shakeSec;
            float strengthCoeff = Mathf.SmoothStep(1, 0, perone);
            cameraShakeOffset = shakeRadius * Random.onUnitSphere;
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
        float restoreTotalTime = .1f;
        deltime = 0;
        while (true)
        {
            deltime += Time.deltaTime;
            float perone = deltime / restoreTotalTime;
            cameraShakeOffset = Vector3.Lerp(cameraShakeOffset, Vector3.zero, perone);
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
        cameraShakeOffset = Vector3.zero;
    }
    #endregion
}
