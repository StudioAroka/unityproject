using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class ArokaShineUI : MonoBehaviour
{

    [ReadOnly]
    public Image shinerImg;

    [OnValueChanged("OVC_Setting")]
    [Range(0,180)]
    public float rotation = 30;
    [OnValueChanged("OVC_Setting")]
    [MinMaxSlider(-1000, 1000)]
    public Vector2 rangeX;
    [OnValueChanged("OVC_Setting")]
    public float offsetY;
    [OnValueChanged("OVC_Setting")]
    public CurvName curvName = CurvName.Linear;
    [OnValueChanged("OVC_Setting")]
    public float alpha = .5f;

    public float performTime = 1.5f;

    public float delayTime = 2f;

    public void OVC_Setting()
    {
        Refresh();
    }

    private void Start()
    {
        gameObject.GetComponent<Mask>().enabled = true;

        StartCoroutine(ShineEffectRoutine());
    }

    IEnumerator ShineEffectRoutine()
    {
        yield return null;
        Vector3 initialPos = Vector3.right * rangeX.x + Vector3.up * offsetY;
        Vector3 targetPos = Vector3.right * rangeX.y + Vector3.up * offsetY;
        while (gameObject)
        {
            shinerImg.transform.localPosition = initialPos;
            shinerImg.transform.ArokaTr().SetLocalPos(targetPos, performTime, curvName, delayTime);
            yield return new WaitForSeconds(performTime + delayTime);
        }
    }

    [Button("Refresh", ButtonSizes.Large)]
    public void Refresh()
    {
        if (GetComponent<Mask>() == null)
        {
            gameObject.AddComponent<Mask>();
        }
        gameObject.GetComponent<Mask>().showMaskGraphic = true;
        gameObject.GetComponent<Mask>().enabled = false;
        if (!shinerImg)
        {
            shinerImg = new GameObject("Shiner").AddComponent<Image>();
            shinerImg.transform.SetParent(transform);
            shinerImg.gameObject.AddComponent<ArokaTransform>();

            shinerImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            shinerImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);
        }
        shinerImg.transform.localPosition = Vector3.Lerp(Vector3.right * rangeX.x , Vector3.right * rangeX.y, .5f) + Vector3.up * offsetY;
        shinerImg.transform.localRotation = Quaternion.Euler(Vector3.back * rotation);
        shinerImg.color = shinerImg.color.ModifiedAlpha(alpha);

    }




}
