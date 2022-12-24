using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ToolInfo : MonoBehaviour
{
    [Title("PRESET")]
    public ToolName toolName;

    [Header("Scale")]
    [Range(0, 5f)]
    public float targetScaleFactor = 1f;

    [Header("Speed")]
    [Range(0, 10f)]
    public float speed_show = 1f;
    [Range(0, 10f)]
    public float speed_hide = 1f;

    [Header("ETC")]
    public GameObject[] specialObjs;

    [Title("RUNTIME")]
    public bool isActive;
    private Coroutine nowScaleRoutine;


    public void SetOn(bool b, bool usePerformance)
    {
        isActive = b;
        SetScale(b, usePerformance);
        for(int i = 0; i <specialObjs.Length; i++)
        {
            specialObjs[i].gameObject.SetActive(b);
        }
    }

    void SetScale(bool b, bool usePerformance)
    {
        if(nowScaleRoutine != null)
        {
            StopCoroutine(nowScaleRoutine);
        }
        float _targetScaleFactor = b ? targetScaleFactor : 0f;
        float _speed = b ? speed_show : speed_hide;
        if(usePerformance)
            nowScaleRoutine = StartCoroutine(SetScaleRoutine(_targetScaleFactor, _speed));
        else
        {
            transform.localScale = _targetScaleFactor * Vector3.one;
        }
    }

    IEnumerator SetScaleRoutine(float _targetScaleFactor, float speed)
    {
        float accumTime = 0;
        float totalTime = 1f;
        float initialScaleFactor = transform.localScale.x;
        while(true)
        {
            accumTime += Time.deltaTime * speed;
            float perone = accumTime / totalTime;

            float properScaleFactor = Mathf.Lerp(initialScaleFactor, _targetScaleFactor, perone);
            transform.localScale = Vector3.one * properScaleFactor;
            if(perone >= 1f)
            {
                break;
            }
            yield return null;
        }
    }

}
