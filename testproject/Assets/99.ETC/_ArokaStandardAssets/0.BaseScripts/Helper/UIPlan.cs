using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIPlan : MonoBehaviour
{
    public List<GameObject> sceneObjects = new List<GameObject>();

    public void InitializeUIPlan()
    {

    }
    [Button("대표 판넬 생성및 추가", ButtonSizes.Large)]
    public void MakeStandardPanel()
    {
        Image panelImg = new GameObject("Panel : " + gameObject.name).AddComponent<Image>();
        panelImg.transform.SetParent(UIManager.Instance.mainCanvas.transform);
        panelImg.gameObject.AddComponent<ArokaAnimParent>();
        panelImg.rectTransform.anchorMin = new Vector2(0, 0);
        panelImg.rectTransform.anchorMax = new Vector2(1, 1);
        panelImg.rectTransform.pivot = new Vector2(.5f, .5f);
        panelImg.rectTransform.SetLeft(0);
        panelImg.rectTransform.SetRight(0);
        panelImg.rectTransform.SetTop(0);
        panelImg.rectTransform.SetBottom(0);
        panelImg.color = Color.white.ModifiedAlpha(0);
        panelImg.transform.localScale = Vector3.one;
        sceneObjects.Add(panelImg.gameObject);

        Refresh();
    }

    [Button("REFRESH", ButtonSizes.Large)]
    void Refresh()
    {
        for(int i = sceneObjects.Count - 1; i >= 0; i--)
        {
            if(sceneObjects[i] == null)
            {
                sceneObjects.RemoveAt(i);
            }
        }
    }

}
