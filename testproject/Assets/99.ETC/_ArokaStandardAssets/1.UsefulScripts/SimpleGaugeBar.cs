using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class SimpleGaugeBar : MonoBehaviour
{
    public bool playOnAwake;

    public Image background;
    public Image fillRect;

    public bool usingKnob;
    [ShowIf("usingKnob")]
    public Image knob;
    [ShowIf("usingKnob")]
    public Image knobStandardImg;

    public Coroutine nowGaugeRoutine;
    [Range(0, 1)]
    public float targetPerone;
    [Range(0,1)]
    public float lerpSpeed = .125f;
    private void Start()
    {
        if(playOnAwake)
        {
            StartGaugeRoutine();
        }
    }
    public void StartGaugeRoutine()
    {
        Debug.Log("Aroka Gauge Routine Started");
        StopGaugeRoutine();
        nowGaugeRoutine = StartCoroutine(UpdateGaugeBarRoutine());
    }
    public void StopGaugeRoutine()
    {
        Debug.Log("Aroka Gauge Routine Stopped");
        if (nowGaugeRoutine != null)
        {
            StopCoroutine(nowGaugeRoutine);
        }
    }
    IEnumerator UpdateGaugeBarRoutine()
    {
        while (true)
        {
            RefreshGaugeRoutine();
            yield return null;
        }
    }

    void RefreshGaugeRoutine()
    {
        fillRect.fillAmount = Mathf.Lerp(fillRect.fillAmount, targetPerone, lerpSpeed);
        if (usingKnob)
        {
            float sizeDelta = knobStandardImg.rectTransform.sizeDelta.x;
            float properX = Mathf.Lerp(-sizeDelta * .5f, sizeDelta * .5f, targetPerone);
            knob.transform.localPosition = knob.transform.localPosition.ModifiedX(properX);
        }
    }
}
