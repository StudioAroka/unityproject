using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePartUi : MonoBehaviour
{
    public Image backgroundImg;
    public Image fillImg;

    public void SetFill(bool b)
    {
        fillImg.transform.GetComponent<ArokaAnim>().SetAnim(b);
    }
}
