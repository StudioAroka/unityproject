using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class ItemBtn : MonoBehaviour
{
    public enum BtnStatus
    {
        NotSelected,
        Selected,
        Locked,
    }
    [Title("PRESET")]
    public Image checkImg;
    public Image lockImg;
    public Image previewImg;
    [Title("RUNTIME")]
    public bool interactable;
    public ItemScrollView parentItemScrollView;
    public ItemData itemData;
    public ItemType ItemType => parentItemScrollView.itemType;
    public BtnStatus nowBtnStatus;

    public void InitializeItemBtn(ItemScrollView _parentItemScrollView, ItemData _itemData)
    {
        SetInteractable(true);

        parentItemScrollView = _parentItemScrollView;
        itemData = _itemData;

        switch (_itemData.itemType)
        {
            default:
                break;
        }

        SetBtnStatus(BtnStatus.NotSelected);
    }
    public void OnClickedItemBtn()
    {
        Debug.Log("눌림");
        if (!interactable)
        {
            return;
        }
        if (nowBtnStatus != BtnStatus.NotSelected)
        {
            return;
        }
        parentItemScrollView.SetSelectedBtn(this);
    }
    public void Refresh()
    {
        checkImg.gameObject.CustomSetActive(nowBtnStatus == BtnStatus.Selected);
        lockImg.gameObject.CustomSetActive(nowBtnStatus == BtnStatus.Locked);
    }
    public void SetBtnStatus(BtnStatus _btnStatus)
    {
        nowBtnStatus = _btnStatus;
        Refresh();
    }

    public void SetInteractable(bool b)
    {
        interactable = b;

        //GetComponent<Image>().color = GetComponent<Image>().color.ModifiedAlpha(b ? 1f : .5f);
        previewImg.color = previewImg.color.ModifiedAlpha(b ? 1f : .5f);

    }
}