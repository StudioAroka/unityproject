using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public enum ItemType
{
    None,
    Coin,
    Wood,
}
public class ItemScrollView : MonoBehaviour
{
    StageManager stageManager;
    [Title("PRESET")]
    public ItemType itemType;
    public ItemBtn itemBtnPrefab;
    public ScrollRect scrollRect;
    public Transform btnContent;
    public ArokaAnim nextBtn;
    public ArokaAnim blackPanel;
    [OnValueChanged("OVC_ScrollviewSetting")]
    [Range(0, 1500)]
    public float btnPadding = 200f;
    [OnValueChanged("OVC_ScrollviewSetting")]
    [Range(0, 1500)]
    public float leftMargin = 100f;

    [Title("RUNTIME")]
    public List<ItemBtn> nowItemBtns;
    public bool isNextBtnClicked;
    public ItemBtn nowItemBtn;

    public bool IsAllItemBtnStatus(ItemBtn.BtnStatus _btnStatus)
    {
        for (int i = 0; i < nowItemBtns.Count; i++)
        {
            if (nowItemBtns[i].nowBtnStatus != _btnStatus)
            {
                return false;
            }
        }
        return true;
    }

    private void Awake()
    {
        stageManager = StageManager.Instance;
    }
    public void OVC_ScrollviewSetting()
    {
        ReArrangeBtns(nowItemBtns);
    }
    public void InitializeSimpleScrollView(List<ItemData> itemDatas)
    {
        SetOn(false, false, false);
        isNextBtnClicked = false;
        //버튼 초기화 및 위치잡기
        btnContent.transform.DestroyAllChildren();
        nowItemBtns = new List<ItemBtn>();
        //chul 수정
        btnContent.GetComponent<Image>().rectTransform.anchoredPosition = btnContent.GetComponent<Image>().rectTransform.anchoredPosition.ModifiedX(0);

        for (int i = 0; i < itemDatas.Count; i++)
        {
            ItemBtn inst_btn = Instantiate(itemBtnPrefab.gameObject, btnContent).GetComponent<ItemBtn>();
            nowItemBtns.Add(inst_btn);
            inst_btn.InitializeItemBtn(this, itemDatas[i]);
        }
        ReArrangeBtns(nowItemBtns);
        RefreshAll();
    }
    void ReArrangeBtns(List<ItemBtn> inst_itemBtns)
    {
        float contentSizeWidthPredicted = inst_itemBtns.Count * btnPadding + leftMargin;
        btnContent.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentSizeWidthPredicted);
        for(int i = 0; i < inst_itemBtns.Count; i++)
        {
            float properX = leftMargin + i * btnPadding;
            inst_itemBtns[i].transform.SetParent(btnContent);
            inst_itemBtns[i].transform.localPosition = Vector2.right * properX;
        }
    }
    public void SetSelectedBtn(ItemBtn targetBtn)
    {
        if (nowItemBtn == targetBtn)
        {
            Debug.LogWarning("이미 그 버튼인것으로 예상됨");
            return;
        }
        nowItemBtn = targetBtn;
        SetSelectedOnlyOneBtn(targetBtn);

        if (targetBtn == null)
        {
            return;
        }
        SetOn(true, true);
        switch (itemType)
        {
            default:
                break;
        }
        RefreshAll();
    }

    public void RefreshAll()
    {
        for (int i = 0; i < nowItemBtns.Count; i++)
        {
            nowItemBtns[i].Refresh();
        }
    }

    void SetSelectedOnlyOneBtn(ItemBtn targetBtn)
    {
        for (int i = 0; i < nowItemBtns.Count; i++)
        {
            if (nowItemBtns[i].nowBtnStatus == ItemBtn.BtnStatus.Locked)
            {
                continue;
            }
            else
            {
                bool isProper = nowItemBtns[i] == targetBtn;
                nowItemBtns[i].SetBtnStatus(isProper ? ItemBtn.BtnStatus.Selected : ItemBtn.BtnStatus.NotSelected);
            }
        }
    }

    public void SetOn(bool blackPanelOn, bool nextBtnOn, bool usePerformance = true)
    {
        blackPanel.SetAnim(blackPanelOn, usePerformance);
        nextBtn.SetAnim(nextBtnOn, usePerformance);
    }
    public void OnClickedNextBtn()
    {
        if (nextBtn.nowAnimStatus != AnimStatus.On)
        {
            return;
        }
        isNextBtnClicked = true;
        SetOn(false, false);
    }
    public IEnumerator AwaitNextBtnClickedRoutine()
    {
        while (true)
        {
            if (isNextBtnClicked)
            {
                break;
            }
            yield return null;
        }
    }

}