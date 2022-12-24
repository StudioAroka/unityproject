using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum CameraDataMode
{
    Static,
    PlayerFocus_Plus_Static,
    PlayerFocus_Plus_LookRotation,
    SpecialFocus_Plus_Static,
    SpecialFocus_Plus_LookRotation
}
public class CameraData : MonoBehaviour
{
    public CameraDataMode cameraDataMode = CameraDataMode.PlayerFocus_Plus_Static;

    public bool useLerpFollowPos = true;
    [ShowIf("useLerpFollowPos")]
    [Range(0, 20)]
    public float cameraPosSpeed = 2.5f;
    public bool useLerpFollowRot = true;
    [ShowIf("useLerpFollowRot")]
    [Range(0, 20)]
    public float cameraRotSpeed = 2.5f;

    public bool useDynamicFov;
    [ShowIf("useDynamicFov")]
    [Range(0, 200)]
    public float targetFov = 0;
    [ShowIf("useDynamicFov")]
    [Range(0, 20)]
    public float fovSpeed = 2.5f;

    public bool UseSpecialFocusTarget => (cameraDataMode == CameraDataMode.SpecialFocus_Plus_LookRotation) || (cameraDataMode == CameraDataMode.SpecialFocus_Plus_Static);

    [ShowIf("UseSpecialFocusTarget")]
    public GameObject specialFocusTarget;

    public Vector3 SpecialTargetPos
    {
        get
        {
            if(specialFocusTarget == null)
            {
                return Vector3.zero;
            }
            else
            {
                return specialFocusTarget.transform.position;
            }
        }
    }
}
