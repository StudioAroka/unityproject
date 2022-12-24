using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StagePartGuageBar : MonoBehaviour
{
    public StagePartUi stagePartUiPrefab;
    public List<Image> backgrounds = new List<Image>();
    public List<Image> fills = new List<Image>();
    public List<StagePartUi> instStagePartUis = new List<StagePartUi>();

    public void InitiailizeGuageBar(Stage stage)
    {
        for (int i = instStagePartUis.Count - 1; i >= 0; i--)
        {
            if(instStagePartUis[i] != null)
                Destroy(instStagePartUis[i].gameObject);
        }
        instStagePartUis = new List<StagePartUi>();

        for (int i = 0; i < stage.stageParts.Count; i++)
        {
            StagePartUi stagePartUi = Instantiate(stagePartUiPrefab, transform).GetComponent<StagePartUi>();
            stagePartUi.transform.localPosition = (i - ((stage.stageParts.Count - 1) * 0.5f)) * Vector3.right * 150;
            stagePartUi.SetFill(false);
            instStagePartUis.Add(stagePartUi);
        }
    }

    public void SetFill(int fillCount)
    {
        for (int i = 0; i < instStagePartUis.Count; i++)
        {
            bool isFill = i < fillCount;
            instStagePartUis[i].SetFill(isFill);
        }
    }
}
