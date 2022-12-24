using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


public class AnimationManager : MonoBehaviour
{
    #region singleTone
    private static AnimationManager _instance = null;
    public static AnimationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(AnimationManager)) as AnimationManager;
                if (_instance == null)
                {
                    Debug.Log("?????? ????????");
                }
            }
            return _instance;
        }
    }
    #endregion

    public AnimationClip[] IdleAnims;
    public AnimationClip[] runAnims;
    public AnimationClip[] sprayWaterAnims;
    public AnimationClip[] cutAxeAnims;
    public AnimationClip[] ironPickAnims;
    public AnimationClip[] ironTakeMagnetAnims;
    public AnimationClip[] fishAnims;
    public AnimationClip[] fishTakeAnims;

}