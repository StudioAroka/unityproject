using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ArokaProgressBar : MonoBehaviour
{
    public enum IconStatusName
    {
        Ready,
        Activating,
        Completed,
    }

    [System.Serializable]
    public class IconProperty
    {
        public bool IsParentProgressBarNull => parentProgressBar == null;
        [ShowIf("IsParentProgressBarNull")]
        ArokaProgressBar parentProgressBar;
        [ReadOnly]
        public IconStatusName iconStatusName;

        public bool UseStandardSpr => parentProgressBar ? parentProgressBar.useStandardSpr : false;
        [HideIf("UseStandardSpr")]
        public Sprite iconSpr;
        public Color color = Color.white;
        [Range(0,8f)]
        public float size = 1f;

        public IconProperty(IconStatusName _iconStatusName, Color _color, float _size)
        {
            iconStatusName = _iconStatusName;
            color = _color;
            size = _size;
        }

        public void InitializeIconPlan(ArokaProgressBar _parentProgressBar)
        {
            parentProgressBar = _parentProgressBar;
        }
    }

    [Title("PRESET")]
    [Range(0, 1000)]
    public float iconTerm = 500;
    public bool useStandardSpr = true;
    [ShowIf("useStandardSpr")]
    public Sprite standardSpr;
    public Image iconParent;
    public List<IconProperty> iconProperties;
    public IconProperty GetIconProperty(IconStatusName iconStatusName)
    {
        for(int i = 0; i < iconProperties.Count; i++)
        {
            if(iconProperties[i].iconStatusName == iconStatusName)
            {
                return iconProperties[i];
            }
        }
        return null;
    }

    private void Reset()
    {
        iconProperties = new List<IconProperty>();
        int iconStatusNameCount = typeof(IconStatusName).EnumCount();
        for(int i = 0; i < iconStatusNameCount; i++)
        {
            iconProperties.Add(new IconProperty((IconStatusName)i, Color.white, 1f));
            iconProperties[i].InitializeIconPlan(this);
        }
    }
    public List<IconPlan> iconPlans;
    [Title("RUNTIME")]

    [System.Serializable]
    public class IconPlan
    {
        public bool IsParentProgressBarNull => parentProgressBar == null;
        [ShowIf("IsParentProgressBarNull")]
        ArokaProgressBar parentProgressBar;
        public int IndexOfThis()
        {
            return parentProgressBar.iconPlans.IndexOf(this);
        }

        [HideInInspector]
        public Image nowIcon;

        public void SetIconStatus(IconStatusName iconStatusName)
        {
            IconProperty iconProperty = parentProgressBar.GetIconProperty(iconStatusName);
            ApplyIconWithIconProperty(iconProperty);
        }
        public void InstantiateIcon()
        {
            nowIcon = new GameObject("Icon " + IndexOfThis()).AddComponent<Image>();
            nowIcon.transform.SetParent(parentProgressBar.iconParent.transform);
        }
        public void InitializeIconPlan(ArokaProgressBar _parentProgressBar)
        {
            parentProgressBar = _parentProgressBar;
        }
        void ApplyIconWithIconProperty(IconProperty iconProperty)
        {
            nowIcon.sprite = parentProgressBar.useStandardSpr ?  parentProgressBar.standardSpr : iconProperty.iconSpr;
            nowIcon.SetNativeSize();
            nowIcon.color = iconProperty.color;
            nowIcon.transform.localScale = (Vector3.one * iconProperty.size).ModifiedZ(1f);
        }
    }


    public void ConsistProgressBar(List<IconPlan> _iconPlans)
    {
        iconPlans = _iconPlans;
        ConsistIcons();
    }
    public void ConsistProgressBar(int iconCount)
    {
        iconPlans = new List<IconPlan>();
        for(int i = 0; i < iconCount; i++)
        {
            iconPlans.Add(new IconPlan());
        }
        ConsistIcons();
    }

    public void SetIconStatus(int i, IconStatusName iconStatusName)
    {
        iconPlans[i].SetIconStatus(iconStatusName);
    }

    [Button]
    public void ConsistIcons()
    {
        iconParent.transform.DestroyImmediateAllChildren();
        for (int i = 0; i < iconPlans.Count; i++)
        {
            iconPlans[i].InitializeIconPlan(this);
            iconPlans[i].InstantiateIcon();
            iconPlans[i].SetIconStatus(IconStatusName.Ready);
            Vector3 localPos_min = Vector3.left * iconTerm;
            Vector3 localPos_max = Vector3.right * iconTerm;
            iconPlans[i].nowIcon.transform.localPosition = Vector3.Lerp(localPos_min, localPos_max, (float)i / (iconPlans.Count - 1));
        }
    }

    public void SetTargetIndex(int targetIndex)
    {
        for (int i = 0; i < targetIndex; i++)
        {
            SetIconStatus(i, IconStatusName.Completed);
        }
        SetIconStatus(targetIndex, IconStatusName.Activating);
        for (int i = targetIndex + 1; i < iconPlans.Count; i++)
        {
            SetIconStatus(i, IconStatusName.Ready);
        }
    }

}
