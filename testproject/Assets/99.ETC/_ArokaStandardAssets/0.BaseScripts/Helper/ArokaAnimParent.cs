using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class ArokaAnimParent : MonoBehaviour
{
    public List<ArokaAnim> arokaAnimsToExclude;
    private List<ArokaAnim> nowArokaAnims = new List<ArokaAnim>();

    public void InitializeArokaAnimParent()
    {
        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().raycastTarget = false;
        }
        nowArokaAnims = new List<ArokaAnim>();
        nowArokaAnims.AddRange(GetComponentsInChildren<ArokaAnim>());
        for (int i = 0; i < arokaAnimsToExclude.Count; i++)
        {
            if (nowArokaAnims.Contains(arokaAnimsToExclude[i]))
            {
                nowArokaAnims.Remove(arokaAnimsToExclude[i]);
            }
        }
    }
    public void SetAnimAllChildren(bool b)
    {
        InitializeArokaAnimParent();
        for (int i = 0; i < nowArokaAnims.Count; i++)
        {
            nowArokaAnims[i].SetAnim(b);
        }
    }
    [Button("자식 전체 처음상태 보기")]
    public void SetStartStatusAllChildren()
    {
        InitializeArokaAnimParent();
        for (int i = 0; i < nowArokaAnims.Count; i++)
        {
            nowArokaAnims[i].SetAnim(false, false);
        }
    }
    [Button("자식 전체 미리보기완성 상태 보기")]
    public void BakeAllChildren()
    {
        InitializeArokaAnimParent();
        for (int i = 0; i < nowArokaAnims.Count; i++)
        {
            nowArokaAnims[i].SetAnim(true, false);
        }
    }
}
