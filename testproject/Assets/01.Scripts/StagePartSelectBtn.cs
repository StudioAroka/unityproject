using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class StagePartSelectBtn : MonoBehaviour
{
    [Title("PRESET")]
    [Title("RUNTIME")]
    public StagePart linkedStagePart;
    public ArokaTrackingUI ArokaTrackingUI => transform.GetComponent<ArokaTrackingUI>();
    public void InitializeStagePartSelectBtn(StagePart stagePart)
    {
        linkedStagePart = stagePart;
        ArokaTrackingUI.InitializeArokaTrakingUI(stagePart.gameObject, stagePart.CenterTopPos - stagePart.transform.localPosition);
    }
    public void OnClickedStagePartSelectBtn()
    {
        StageManager.Instance.nowStage.nowSelectedStagePart = linkedStagePart;
    }


}
