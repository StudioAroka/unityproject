using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LightPlan : MonoBehaviour
{
    public bool useUpdateFollow = true;
    [ShowIf("useUpdateFollow")]
    [Range(0, 20)]
    public float lightPosSpeed = 3f;
    [ShowIf("useUpdateFollow")]
    [Range(0, 20)]
    public float lightRotSpeed = 3f;

    [HideIf("useUpdateFollow")]
    public float totalTime = 1f;
    [HideIf("useUpdateFollow")]
    public CurvName curvName = CurvName.EaseOut;


    public void InitializeLightPlan()
    {

    }
}
