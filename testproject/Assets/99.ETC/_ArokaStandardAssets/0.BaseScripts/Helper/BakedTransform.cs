using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class BakedTransform : MonoBehaviour
{
    public enum BakedTrStatusName
    {
        WhenStageBaked,
        A,
        B,
        C,
        D,
    }
    [System.Serializable]
    public class BakedTrData
    {
        public BakedTrStatusName bakedTrStatusName;
        public TransformData transformData;
        public BakedTrData(BakedTrStatusName _bakedTrStatusName, TransformData _transformData)
        {
            bakedTrStatusName = _bakedTrStatusName;
            transformData = _transformData;
        }
        public BakedTrData(BakedTrStatusName _bakedTrStatusName, Transform _tr)
        {
            bakedTrStatusName = _bakedTrStatusName;
            transformData = new TransformData(_tr);
        }
    }
    public BakedTrData GetBakedTrData(BakedTrStatusName bakedTrStatusName)
    {
        for (int i = 0; i < bakedTrDatas.Count; i++)
        {
            if (bakedTrDatas[i].bakedTrStatusName == bakedTrStatusName)
            {
                return bakedTrDatas[i];
            }
        }
        return null;
    }

    [TitleGroup("====== Bake System ======")]
    public BakedTrStatusName bakeStatus;
    [TitleGroup("====== Bake System ======")]
    [GUIColor(0, 1, 0)]
    [Button("BAKE", ButtonSizes.Medium)]
    public void BakeButton()
    {
        BakeTransform(bakeStatus);
    }
    public void BakeTransform(BakedTrStatusName bakedTrStatusName)
    {
        BakedTrData bakedTrDataFound = GetBakedTrData(bakedTrStatusName);
        if (bakedTrDataFound != null)
        {
            bakedTrDatas.Remove(bakedTrDataFound);
        }
        BakedTrData bakedTrDataToAdd = new BakedTrData(bakedTrStatusName, transform);
        bakedTrDatas.Add(bakedTrDataToAdd);
    }

    [TitleGroup("====== PREVIEW =======")]
    public BakedTrStatusName previewStatus;
    [TitleGroup("====== PREVIEW =======")]
    [GUIColor(0, 1, 0)]
    [Button("SHOW PREVIEW", ButtonSizes.Medium)]
    public void PreviewButton()
    {
        SetTransform(previewStatus);
    }

    public void SetTransform(BakedTrStatusName bakedTrStatusName)
    {
        BakedTrData bakedTrData = GetBakedTrData(bakedTrStatusName);
        if (bakedTrData == null)
        {
            Debug.LogWarning("해당 bakedTr 존재하지 않습니다 ^^ ");
        }
        bakedTrData.transformData.SetWithThisData(transform);
    }


    [ReadOnly]
    [TitleGroup("====== Baked Datas ======")]
    public List<BakedTrData> bakedTrDatas = new List<BakedTrData>();
    [GUIColor(0, 1, 0)]
    [TitleGroup("====== Baked Datas ======")]
    [Button("CLEAR", ButtonSizes.Medium)]
    public void Clear()
    {
        bakedTrDatas.Clear();
    }


}