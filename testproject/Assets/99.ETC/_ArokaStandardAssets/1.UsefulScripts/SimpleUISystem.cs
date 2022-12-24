using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SimpleUISystem : MonoBehaviour
{
    [System.Serializable]
    public class PopUpPlan
    {
        public int stepIndex;
        public List<GameObject> sceneObjects;
    }
    [Title("PRESET")]
    public PopUpPlan[] popUpPlans;

    [Title("PREVIEW FUNCTION")]
    [Button("Preview UI", ButtonSizes.Large)]
    public void Preview(int stepIndex)
    {
        RegisterAllStepObjects();
        PopUpPlan stepPlan = GetPopUpPlan(stepIndex);
        List<GameObject> targetGameObjects = stepPlan.sceneObjects;
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            GameObject stepObject = allSceneObjects[i];
            bool isProper = targetGameObjects != null && targetGameObjects.Contains(stepObject);
            allSceneObjects[i].GetComponent<ArokaAnim>().SetAnim(isProper, false);
        }
    }

    [Title("RUNTIME")]
    public PopUpPlan nowPopUpPlan;
    public List<GameObject> allSceneObjects = new List<GameObject>();

    private void Awake()
    {
        RegisterAllStepObjects();
        MakeStepPlan(0);
    }
    public void MakeStepPlan(int stepIndex)
    {
        PopUpPlan stepPlan = GetPopUpPlan(stepIndex);
        nowPopUpPlan = stepPlan;
        SetUI(stepPlan.sceneObjects);
    }

    public PopUpPlan GetPopUpPlan(int stepIndex)
    {
        for (int i = 0; i < popUpPlans.Length; i++)
        {
            if (popUpPlans[i].stepIndex == stepIndex)
            {
                return popUpPlans[i];
            }
        }
        return null;
    }
    private void SetUI(List<GameObject> targetGameObjects)
    {
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            GameObject stepObject = allSceneObjects[i];
            bool isProper = targetGameObjects != null && targetGameObjects.Contains(stepObject);
            allSceneObjects[i].CustomSetActive(isProper);
        }
    }
    private void RegisterAllStepObjects()
    {
        allSceneObjects = new List<GameObject>();
        for (int i = 0; i < popUpPlans.Length; i++)
        {
            for (int j = 0; j < popUpPlans[i].sceneObjects.Count; j++)
            {
                if (!allSceneObjects.Contains(popUpPlans[i].sceneObjects[j]))
                    allSceneObjects.Add(popUpPlans[i].sceneObjects[j]);
            }
        }
    }
}
