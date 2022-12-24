using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public enum TutorialMent
{
    None,
    TutorialFinger,
    TutorialMent,
}
public class TutorialManager : MonoBehaviour
{
    #region singleTone
    private static TutorialManager _instance = null;
    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(TutorialManager)) as TutorialManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion
    [System.Serializable]
    public class TutorialPlan
    {
        public TutorialMent tutorialMent;
        public GameObject tutorialAnimObj;
    }
    public TutorialPlan[] tutorialPlans;


    public TutorialPlan GetTutorialPlan(TutorialMent tutorialMent)
    {
        for (int i = 0; i < tutorialPlans.Length; i++)
        {
            if (tutorialPlans[i].tutorialMent == tutorialMent)
            {
                return tutorialPlans[i];
            }
        }
        return null;
    }
    public void SetActiveTutorial(TutorialMent tutorialMent, bool b)
    {
        if (GetTutorialPlan(tutorialMent).tutorialAnimObj == null)
        {
            return;
        }
        GameObject tutorialAnimObj = GetTutorialPlan(tutorialMent).tutorialAnimObj;
        SetAnimTutorialAnimObj(tutorialAnimObj, b);
    }

    void SetAnimTutorialAnimObj(GameObject tutorialAnimObj, bool b)
    {
        if (tutorialAnimObj.GetComponent<ArokaAnimParent>())
        {
            tutorialAnimObj.GetComponent<ArokaAnimParent>().SetAnimAllChildren(b);
        }
        else if (tutorialAnimObj.GetComponent<ArokaAnim>())
        {
            tutorialAnimObj.GetComponent<ArokaAnim>().SetAnim(b);
        }
    }

    public void SetOnTutorialOnlyOne(TutorialMent tutorialMent)
    {
        for (int i = 0; i < tutorialPlans.Length; i++)
        {
            if (tutorialPlans[i].tutorialAnimObj != null)
            {
                bool isIdentical = tutorialMent == tutorialPlans[i].tutorialMent;
                SetAnimTutorialAnimObj(tutorialPlans[i].tutorialAnimObj, isIdentical);
            }
        }
    }


}