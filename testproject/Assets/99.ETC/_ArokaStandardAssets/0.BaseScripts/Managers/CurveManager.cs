using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurvName
{
    None,
    Linear,
    EaseOut,
    EaseIn,
    EaseInOut,
    Mountain,
    Valley,
    StaticOne,
    StaticZero,
    Mountain_WoodChanger,
    OverEaseOut,
    MountinRight,
    MountinLeft,
    Sin,
    HalfSin
}
public class CurveManager : MonoBehaviour
{
    #region singleTone
    private static CurveManager _instance = null;
    public static CurveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(CurveManager)) as CurveManager;
                if (_instance == null)
                {
                    Debug.Log("?????? ????????");
                }
            }
            return _instance;
        }
    }
    #endregion

    [System.Serializable]
    public class CurvPlan
    {
        public CurvName curvName;
        public AnimationCurve animCurv;
    }
    public CurvPlan[] curvPlans;

    public CurvPlan GetCurvPlan(CurvName curvName)
    {
        for(int i = 0; i < curvPlans.Length; i++)
        {
            if(curvPlans[i].curvName == curvName)
            return curvPlans[i];
        }
        return null;
    }
}