using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class ArokaTrackingUI : MonoBehaviour
{
    private Camera nowCamera;

    [Title("SETTINGS")]
    public bool useManual;
    [ShowIf("useManual")]
    public GameObject objToTrack;
    [ShowIf("useManual")]
    public Vector3 worldOffset;
    [ShowIf("useManual")]
    public Vector3 uiOffset;
    [ShowIf("useManual")]
    [Range(0,1)]
    public float lerpSpeed_pos;

    public bool localScaleFollow_objToTrackLocalScale;
    [Range(0, 1)]
    public float lerpSpeed_localScale = .125f;

    Coroutine nowTrackRoutine;
    void Start()
    {
        nowCamera = Camera.main;

        if(useManual)
        {
            if(nowTrackRoutine!= null)
            {
                StopCoroutine(nowTrackRoutine);
            }
            nowTrackRoutine = StartCoroutine(TrackRoutine());
        }
    }

    public void InitializeArokaTrakingUI(GameObject _objToTrack, Vector3 _worldOffset = default, Vector3 _uiOffset = default, float _lerpSpeed  = .9f)
    {
        if (useManual)
        {
            Debug.LogWarning("useManual ¿Ãπ«∑Œ .");
            return;
        }
        objToTrack = _objToTrack;
        worldOffset = _worldOffset;
        uiOffset = _uiOffset;
        lerpSpeed_pos = _lerpSpeed;

        if (nowTrackRoutine != null)
        {
            StopCoroutine(nowTrackRoutine);
        }
        nowTrackRoutine = StartCoroutine(TrackRoutine());
    }
    
    IEnumerator TrackRoutine()
    {
        nowCamera = Camera.main;
        while (true)
        {
            if(objToTrack)
            {
                transform.position = nowCamera.WorldToScreenPoint(objToTrack.transform.position + worldOffset) + uiOffset;
                if (localScaleFollow_objToTrackLocalScale)
                    transform.localScale = Vector3.Lerp(transform.localScale, objToTrack.transform.localScale, lerpSpeed_localScale);
            }
            yield return null;
        }
    }

}
