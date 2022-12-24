using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum AnimStatus
{
    On,
    Off,
    After,
}
[HideDuplicateReferenceBox]
[System.Serializable]
public class ArokaAnimationData
{
    [OnValueChanged("OVC_ArokaAnimationData")]
    public Color color;
    [LabelText("")]
    [OnValueChanged("OVC_ArokaAnimationData")]
    public TransformData trData;

    public static ArokaAnimationData Lerp(ArokaAnimationData a, ArokaAnimationData b, float perone)
    {
        TransformData trData = TransformData.Lerp(a.trData, b.trData, perone);
        Color color = Color.Lerp(a.color, b.color, perone);

        return new ArokaAnimationData(trData, color);
    }
    public ArokaAnimationData(TransformData _trData, Color _color)
    {
        trData = _trData;
        color = _color;
    }
    public ArokaAnimationData(Transform tr, Color _color)
    {
        trData = new TransformData(tr);
        color = _color;
    }
    public void OVC_ArokaAnimationData()
    {
        SetWithThisData();
    }

    public void SetWithThisData()
    {
        trData.SetWithThisData(trData.tr);
        trData.tr.GetComponent<ArokaAnim>().Color = color;
    }

}
[HideDuplicateReferenceBox]
[System.Serializable]
public class ArokaAnimationSetting
{

    [LabelText("Animation Total Seconds")]
    [Range(0, 100)]
    public float animTotalSec;

    [LabelText("Animation Speed")]
    [Range(0, 30)]
    public float playSpeed;

    [LabelText("Animation Delay")]
    [Range(0, 100)]
    public float animDelaySec;

    [LabelText("Use Custom Transition?")]
    [OnValueChanged("OVC_ArokaSetting")]
    public bool useCustomTransitionCurv;

    [HorizontalGroup("TransitionCurv")]
    [VerticalGroup("TransitionCurv/Left")]
    [LabelText("")]
    [OnValueChanged("OVC_ArokaSetting")]
    [HideIf("useCustomTransitionCurv")]
    [EnumPaging()]
    public CurvName transitionCurvName;

    [HorizontalGroup("TransitionCurv")]
    [LabelText("")]
    [VerticalGroup("TransitionCurv/Right")]
    [EnableIf("useCustomTransitionCurv")]
    public AnimationCurve nowTransitionCurv;

    public ArokaAnimationSetting(float _animDelaySec, float _animTotalSec, float _playSpeed, bool _useCustomTransitionCurv, CurvName _transitionCurvName, AnimationCurve _nowTransitionCurv)
    {
        animDelaySec = _animDelaySec;
        animTotalSec = _animTotalSec;
        playSpeed = _playSpeed;
        useCustomTransitionCurv = _useCustomTransitionCurv;
        transitionCurvName = _transitionCurvName;
        nowTransitionCurv = new AnimationCurve(_nowTransitionCurv.keys);
    }
    public static ArokaAnimationSetting ArokAnimationDefaultSetting(AnimStatus animStatus)
    {
        switch(animStatus)
        {
            case AnimStatus.On:
                return new ArokaAnimationSetting(0, 0.4f, 1f, false, CurvName.EaseOut, CurveManager.Instance.GetCurvPlan(CurvName.EaseOut).animCurv);
            case AnimStatus.Off:
                return new ArokaAnimationSetting(0, 0.4f, 1f, false, CurvName.EaseOut, CurveManager.Instance.GetCurvPlan(CurvName.EaseOut).animCurv);
            case AnimStatus.After:
                return new ArokaAnimationSetting(0, 2f, 1f, false, CurvName.Mountain, CurveManager.Instance.GetCurvPlan(CurvName.Mountain).animCurv);
            default:
                return new ArokaAnimationSetting(0, 0.4f, 1f, false, CurvName.EaseOut, CurveManager.Instance.GetCurvPlan(CurvName.EaseOut).animCurv);
        }
    }

    public void OVC_ArokaSetting()
    {
        nowTransitionCurv = new AnimationCurve(CurveManager.Instance.GetCurvPlan(transitionCurvName).animCurv.keys);
    }
}
[System.Serializable]
public class TransformData
{
    [HideInInspector]
    public Transform tr;
    [HideInInspector]
    public Transform parent;
    [OnValueChanged("OVC_TransformData")]
    public Vector3 localPos;
    [OnValueChanged("OVC_TransformData")]
    public Quaternion localRot;
    [OnValueChanged("OVC_TransformData")]
    public Vector3 localScale;

    public static TransformData Lerp(TransformData trData1, TransformData trData2, float perone)
    {
        Transform tr = trData1.tr;
        Transform parent = trData1.parent;
        Vector3 localPos = Vector3.LerpUnclamped(trData1.localPos, trData2.localPos, perone);
        Quaternion localRot = Quaternion.LerpUnclamped(trData1.localRot, trData2.localRot, perone);
        Vector3 localScale = Vector3.LerpUnclamped(trData1.localScale, trData2.localScale, perone);
        return new TransformData(tr, parent, localPos, localRot, localScale);
    }
    public void ChangeTrOnly(Transform _tr)
    {
        tr = _tr;
        parent = _tr.parent;
    }
    public TransformData(Transform _tr, Transform _parent, Vector3 _localPos, Quaternion _localRot, Vector3 _localScale)
    {
        tr = _tr;
        parent = _parent;
        localPos = _localPos;
        localRot = _localRot;
        localScale = _localScale;
    }
    public TransformData(Transform _tr)
    {
        if (_tr == null)
        {
            Debug.LogWarning("tr is null");
            return;
        }
        tr = _tr;
        parent = _tr.parent;

        if(_tr.GetComponent<ArokaAnim>() != null)
        {
            switch (_tr.GetComponent<ArokaAnim>().trType)
            {
                case TransformType.TextMeshProUGUI:
                    localPos = _tr.GetComponent<TextMeshProUGUI>().rectTransform.anchoredPosition;
                    break;
                case TransformType.Button:
                    localPos = _tr.GetComponent<Button>().image.rectTransform.anchoredPosition;
                    break;
                case TransformType.ETC:
                    localPos = _tr.localPosition;
                    break;
                case TransformType.EventTrigger:
                    localPos = _tr.GetComponent<Image>().rectTransform.anchoredPosition;
                    break;
                case TransformType.Image:
                    localPos = _tr.GetComponent<Image>().rectTransform.anchoredPosition;
                    break;
                case TransformType.RawImg:
                    localPos = _tr.GetComponent<RawImage>().rectTransform.anchoredPosition;
                    break;
                case TransformType.TextMeshPro:
                    localPos = _tr.GetComponent<TextMeshPro>().rectTransform.anchoredPosition;
                    break;
                case TransformType.SpriteRend:
                    localPos = _tr.localPosition;
                    break;
                case TransformType.MeshRend:
                    localPos = _tr.localPosition;
                    break;
                default:
                    localPos = _tr.localPosition;
                    break;
            }
        }
        else
        {
            localPos = _tr.localPosition;
        }
        localRot = _tr.localRotation;
        localScale = _tr.localScale;
    }

    public void SetWithThisData(Transform _tr)
    {
        if (_tr == null)
        {
            Debug.LogWarning("tr is null");
            return;
        }
        tr = _tr;
        _tr.SetParent(parent);
        if (_tr.GetComponent<ArokaAnim>() != null)
        {
            switch (_tr.GetComponent<ArokaAnim>().trType)
            {
                case TransformType.TextMeshProUGUI:
                    _tr.GetComponent<TextMeshProUGUI>().rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.Button:
                    _tr.GetComponent<Button>().image.rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.ETC:
                    _tr.localPosition = localPos;
                    break;
                case TransformType.EventTrigger:
                    _tr.GetComponent<Image>().rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.Image:
                    _tr.GetComponent<Image>().rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.RawImg:
                    _tr.GetComponent<RawImage>().rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.TextMeshPro:
                    _tr.GetComponent<TextMeshPro>().rectTransform.anchoredPosition = localPos;
                    break;
                case TransformType.SpriteRend:
                    _tr.localPosition = localPos;
                    break;
                case TransformType.MeshRend:
                    _tr.localPosition = localPos;
                    break;
                default:
                    _tr.localPosition = localPos;
                    break;
            }
        }
        else
        {
            _tr.localPosition = localPos;
        }

        _tr.localRotation = localRot;
        _tr.localScale = localScale;
    }

    public void OVC_TransformData()
    {
        SetWithThisData(tr);
    }
}
public enum TransformType
{
    ETC,
    MeshRend,
    SpriteRend,
    TextMeshPro,
    TextMeshProUGUI,
    EventTrigger,
    Button,
    RawImg,
    Image,
}
public class ArokaAnim : MonoBehaviour
{
    private ArokaAnimPlayer arokaAnimPlayer;
    [ReadOnly]
    public AnimStatus nowAnimStatus;
    [ReadOnly]
    public bool isRuntime = false;
    public bool IsPlaying => nowRoutine != null;
    [ReadOnly]
    public TransformType trType;

    public bool IsContainImg => trType == TransformType.Image || trType == TransformType.Button || trType == TransformType.EventTrigger || trType == TransformType.TextMeshProUGUI || trType == TransformType.RawImg;

    public Coroutine nowRoutine;
    [LabelText("Allow Call During Animation Playing")]
    public bool allowCallDuringPlay;
    [LabelText("Use After Anim?")]
    public bool useAfterAnim;
    [LabelText("Play On Awake?")]
    public bool playOnAwake;

    public TransformType GetTransformType(Transform tr)
    {
        if (tr.GetComponent<MeshRenderer>() != null)
        {
            return TransformType.MeshRend;
        }
        else if (tr.GetComponent<SpriteRenderer>() != null)
        {
            return TransformType.SpriteRend;
        }
        else if (tr.GetComponent<TextMeshPro>() != null)
        {
            return TransformType.TextMeshPro;
        }
        else if (tr.GetComponent<TextMeshProUGUI>() != null)
        {
            return TransformType.TextMeshProUGUI;
        }
        else if (tr.GetComponent<EventTrigger>() != null)
        {
            return TransformType.EventTrigger;
        }
        else if (tr.GetComponent<Button>() != null)
        {
            return TransformType.Button;
        }
        else if (tr.GetComponent<RawImage>() != null)
        {
            return TransformType.RawImg;
        }
        else if (tr.GetComponent<Image>() != null)
        {
            return TransformType.Image;
        }
        else
        {
            return TransformType.ETC;
        }
    }
    public Color Color
    {
        get
        {
            switch (trType)
            {
                case TransformType.MeshRend:
                    return isRuntime ? transform.GetComponent<MeshRenderer>().material.color : transform.GetComponent<MeshRenderer>().sharedMaterial.color;
                case TransformType.SpriteRend:
                    return transform.GetComponent<SpriteRenderer>().color;
                case TransformType.TextMeshPro:
                    return transform.GetComponent<TextMeshPro>().color;
                case TransformType.TextMeshProUGUI:
                    return transform.GetComponent<TextMeshProUGUI>().color;
                case TransformType.EventTrigger:
                    return transform.GetComponent<Image>().color;
                case TransformType.Button:
                    return transform.GetComponent<Image>().color;
                case TransformType.RawImg:
                    return transform.GetComponent<RawImage>().color;
                case TransformType.Image:
                    return transform.GetComponent<Image>().color;
                case TransformType.ETC:
                    return default;
                default:
                    return default;
            }
        }
        set
        {
            switch (trType)
            {
                case TransformType.MeshRend:
                    if(isRuntime)
                    {
                        transform.GetComponent<MeshRenderer>().material.color = value;
                    }
                    else
                    {
                        transform.GetComponent<MeshRenderer>().sharedMaterial.color = value;
                    }
                    break;
                case TransformType.SpriteRend:
                    transform.GetComponent<SpriteRenderer>().color = value;
                    break;
                case TransformType.TextMeshPro:
                    transform.GetComponent<TextMeshPro>().color = value;
                    break;
                case TransformType.TextMeshProUGUI:
                    transform.GetComponent<TextMeshProUGUI>().color = value;
                    break;
                case TransformType.EventTrigger:
                    transform.GetComponent<Image>().color = value;
                    break;
                case TransformType.Button:
                    transform.GetComponent<Image>().color = value;
                    break;
                case TransformType.RawImg:
                    transform.GetComponent<RawImage>().color = value;
                    break;
                case TransformType.Image:
                    transform.GetComponent<Image>().color = value;
                    break;
                case TransformType.ETC:
                    break;
                default:
                    break;
            }

        }
    }

    //이 컴포넌트가 달린 물체가 어떤다른 컴포넌트들을 가지고있는지 파악후 갱신

    [System.Serializable]
    public class AnimPlan
    {
        [BoxGroup("Preview", CenterLabel = true)]
        [InfoBox("$animStatus")]
        [OnValueChanged("OVC_accumSec_preview")]
        [LabelText("Preview Player")]
        [ProgressBar(0, "AnimTotalSec", Height = 60, ColorGetter = "PreviewPlayerColor", CustomValueStringGetter = "$PreviewPlayerText")]
        public float accumSec_preview;

        [HideInInspector]
        public ArokaAnim parentArokaAnim;
        [HideInInspector]
        public AnimStatus animStatus;


        [Space]
        [HideDuplicateReferenceBox]
        [BoxGroup("Animation", CenterLabel = true)]
        [LabelText("")]
        public ArokaAnimationData animationData;
        [Space]
        [HideDuplicateReferenceBox]
        [BoxGroup("Setting", CenterLabel = true)]
        [LabelText("")]
        public ArokaAnimationSetting animationSetting;

        public float AnimTotalSec => animationSetting.animTotalSec;

        [PropertySpace(20, 100)]
        public AnimPlan OtherAnimPlan => animStatus == AnimStatus.On ? parentArokaAnim.animPlan_off : parentArokaAnim.animPlan_on;

        public AnimPlan (ArokaAnim _parentArokaAnim, AnimStatus _animStatus, ArokaAnimationData _animationData, ArokaAnimationSetting _animationSetting)
        {
            parentArokaAnim = _parentArokaAnim;
            animStatus = _animStatus;

            animationData = _animationData;
            animationSetting = _animationSetting;
        }

        public void OVC_accumSec_preview()
        {
            parentArokaAnim.arokaAnimPlayer = new ArokaAnimPlayer(parentArokaAnim, OtherAnimPlan.animationData, animationData, animationSetting);
            parentArokaAnim.arokaAnimPlayer.SetTransformByAccumSecond(accumSec_preview);
        }

        private Color PreviewPlayerColor
        {
            get
            {
                switch(animStatus)
                {
                    default:
                        return Color.gray;
                    case AnimStatus.On:
                        return new Color(0.3735025f, 1f, 0.3735025f, 1f);
                    case AnimStatus.Off:
                        return new Color(0.7830f, 0.1957f, 0.2359f, 1f);
                    case AnimStatus.After:
                        return new Color(0f, 0.1958159f, 0.3301887f, 1f);
                }
            }
        }
        private string PreviewPlayerText
        {
            get
            {
                float AnimSecondsPerone = accumSec_preview / animationSetting.animTotalSec;
                float accumSec_previewPlayer_forUser = (int)(accumSec_preview * 1000) / 1000f;
                float animTotalPerone_forUser = (int)(AnimSecondsPerone * 1000) / 1000f * 100f;
                return "(Preview) Progressing Seconds : " + accumSec_previewPlayer_forUser + "\nTotal Seconds : " + animationSetting.animTotalSec + "\n(" + animTotalPerone_forUser + "%)";
            }
        }

    }

    [EnumToggleButtons()]
    [LabelText("Editor")]
    public AnimStatus animStatus_editor;
    public bool Is_animStatus_editor_on => animStatus_editor == AnimStatus.On;
    public bool Is_animStatus_editor_off => animStatus_editor == AnimStatus.Off;
    public bool Is_animStatus_editor_after => animStatus_editor == AnimStatus.After;
    [ShowIf("Is_animStatus_editor_on")]
    public AnimPlan animPlan_on;
    [ShowIf("Is_animStatus_editor_off")]
    public AnimPlan animPlan_off;
    [ShowIf("Is_animStatus_editor_after")]
    [EnableIf("useAfterAnim")]
    public AnimPlan animPlan_after;

    private void Reset()
    {
        InitializeArokaAnim();
        Debug.Log("Reset");
    }

    void InitializeArokaAnim()
    {
        isRuntime = false;
        allowCallDuringPlay = true;
        nowRoutine = null;
        useAfterAnim = false;

        trType = GetTransformType(transform);

        //On
        ArokaAnimationData arokaAnimationData_on = new ArokaAnimationData(new TransformData(transform), Color);
        ArokaAnimationSetting defaultAnimationSetting_on = ArokaAnimationSetting.ArokAnimationDefaultSetting(AnimStatus.On);
        animPlan_on = new AnimPlan(this, AnimStatus.On, arokaAnimationData_on, defaultAnimationSetting_on);

        //Off
        TransformData trData_off = new TransformData(transform);
        trData_off.localScale = Vector3.zero;

        ArokaAnimationData arokaAnimationData_off = new ArokaAnimationData(trData_off, Color);
        ArokaAnimationSetting defaultAnimationSetting_off = ArokaAnimationSetting.ArokAnimationDefaultSetting(AnimStatus.Off);
        animPlan_off = new AnimPlan(this, AnimStatus.Off, arokaAnimationData_off, defaultAnimationSetting_off);

        //After
        TransformData trData_after = new TransformData(transform);
        trData_after.localScale = Vector3.one * 1.05f;

        ArokaAnimationData arokaAnimationData_after = new ArokaAnimationData(trData_after, Color);
        ArokaAnimationSetting defaultAnimationSetting_after = ArokaAnimationSetting.ArokAnimationDefaultSetting(AnimStatus.After);
         animPlan_after = new AnimPlan(this, AnimStatus.After, arokaAnimationData_after, defaultAnimationSetting_after);

        SetAnim(true, false);
    }

    [System.Serializable]
    public class ArokaAnimPlayer
    {
        [LabelText("Preview Player")]
        [ProgressBar(0, "AnimTotalSec", Height = 60, ColorGetter = "ArokaPlayerColor", CustomValueStringGetter = "$ArokaPlayerText")]

        [OnValueChanged("OVC_accumSec")]
        public float accumSecond;

        [ReadOnly]
        public ArokaAnim arokaAnim;
        public ArokaAnimationData initialAnimData;
        public ArokaAnimationData targetAnimData;
        public ArokaAnimationSetting animationSetting;

        public float AccumPerone => accumSecond / animationSetting.animTotalSec;
        public float AnimTotalSec => animationSetting.animTotalSec;
        public ArokaAnimPlayer(ArokaAnim _arokaAnim, ArokaAnimationData _initialAnimData, ArokaAnimationData _targetAnimData, ArokaAnimationSetting _animationSetting)
        {
            arokaAnim = _arokaAnim;
            initialAnimData = _initialAnimData;
            targetAnimData = _targetAnimData;
            animationSetting = _animationSetting;
        }
        private string ArokaPlayerText
        {
            get
            {
                float AnimSecondsPerone = accumSecond / animationSetting.animTotalSec;
                float accumSec_previewPlayer_forUser = (int)(accumSecond * 1000) / 1000f;
                float animTotalPerone_forUser = (int)(AnimSecondsPerone * 1000) / 1000f * 100f;
                return "Progressing Seconds : " + accumSec_previewPlayer_forUser + "\nTotal Seconds : " + animationSetting.animTotalSec + "\n(" + animTotalPerone_forUser + "%)";
            }
        }
        private Color ArokaPlayerColor
        {
            get
            {
                return Color.white;
            }
        }

        public void SetTransformByAccumSecond(float _accumSecond)
        {
            accumSecond = Mathf.Clamp(_accumSecond, 0, AnimTotalSec);

            float AnimSecondsPerone = accumSecond / animationSetting.animTotalSec;
            float AnimCurvPerone = animationSetting.nowTransitionCurv.Evaluate(AnimSecondsPerone);

            ArokaAnimationData lerpedArokaAnimData = ArokaAnimationData.Lerp(initialAnimData, targetAnimData, AnimCurvPerone);
            arokaAnim.SetArokaAnimationData(lerpedArokaAnimData);


        }
        public void AddTransformByAccumSecond(float secondToAdd)
        {
            accumSecond += secondToAdd;
            SetTransformByAccumSecond(accumSecond);
        }
        public void OVC_accumSec()
        {
            SetTransformByAccumSecond(accumSecond);
        }
    }


    private void Awake()
    {
        isRuntime = true;
        SetAnim(false, false);
        if(playOnAwake)
        {
            SetAnim(true, true);
        }
    }

    void SetArokaAnimationData(ArokaAnimationData _arokaAnimationData)
    {
        _arokaAnimationData.SetWithThisData();
    }

    public void SetAnim(bool b, bool usePerformance = true)
    {
        if (b)
        {
            if (nowAnimStatus == AnimStatus.On || nowAnimStatus == AnimStatus.After)
            {
                return;
            }
        }
        else
        {
            if (nowAnimStatus == AnimStatus.Off)
            {
                return;
            }
        }
        if (nowRoutine != null)
        {
            StopCoroutine(nowRoutine);
        }
        nowRoutine = StartCoroutine(PlayTotalAnimationRoutine(b, usePerformance));

    }
    public IEnumerator PlayTotalAnimationRoutine(bool b, bool usePerformance)
    {
        if(b)
        {
            yield return StartCoroutine(PlayAnimPlanRoutine(animPlan_on,  usePerformance));
            if(useAfterAnim)
                yield return StartCoroutine(PlayAnimPlanRoutine(animPlan_after, usePerformance));
        }
        else
        {
            yield return StartCoroutine(PlayAnimPlanRoutine(animPlan_off, usePerformance));
        }

    }
    IEnumerator PlayAnimPlanRoutine(AnimPlan targetAnimPlan, bool usePerformance)
    {
        nowAnimStatus = targetAnimPlan.animStatus;

        if (trType == TransformType.Button || trType == TransformType.EventTrigger)
        {
            GetComponent<Image>().raycastTarget = nowAnimStatus != AnimStatus.Off;
        }
        ArokaAnimationSetting targetAnimationSetting = targetAnimPlan.animationSetting;
        AnimPlan presentAnimPlan = new AnimPlan(this, nowAnimStatus, new ArokaAnimationData(new TransformData(transform), Color), targetAnimationSetting);
        arokaAnimPlayer = new ArokaAnimPlayer(this, presentAnimPlan.animationData, targetAnimPlan.animationData, targetAnimationSetting);

        if(usePerformance)
        {
            yield return new WaitForSeconds(targetAnimPlan.animationSetting.animDelaySec);
            float speed = targetAnimationSetting.playSpeed;
            arokaAnimPlayer.SetTransformByAccumSecond(0);
            while (targetAnimPlan.animStatus == nowAnimStatus)
            {
                arokaAnimPlayer.AddTransformByAccumSecond(Time.deltaTime * speed);

                if (arokaAnimPlayer.AccumPerone >= 1)
                {
                    if (targetAnimPlan.animStatus == AnimStatus.After)
                    {
                        arokaAnimPlayer.accumSecond -= arokaAnimPlayer.AnimTotalSec;
                    }
                    else
                    {
                        break;
                    }
                }
                yield return null;
            }
        }
        else
        {
            arokaAnimPlayer.SetTransformByAccumSecond(targetAnimationSetting.animTotalSec);
        }
    }

    [PropertySpace(30, 30)]
    [ButtonGroup("1")]
    [Button("On 미리보기", ButtonSizes.Large)]
    public void PreviewOnStatus()
    {
        SetArokaAnimationData(animPlan_on.animationData);
    }
    [PropertySpace(30, 30)]
    [ButtonGroup("1")]
    [Button("이 상태를 On으로 베이크", ButtonSizes.Large)]
    public void BakeAsOn()
    {
        ArokaAnimationData arokaAnimationData_on = new ArokaAnimationData(new TransformData(transform), Color);
        animPlan_on.animationData = arokaAnimationData_on;
    }
    [PropertySpace(30, 30)]
    [ButtonGroup("2")]
    [Button("Off 미리보기", ButtonSizes.Large)]
    public void PreviewOffStatus()
    {
        SetArokaAnimationData(animPlan_off.animationData);
    }
    [PropertySpace(30, 30)]
    [ButtonGroup("2")]
    [Button("이 상태를 Off로 베이크", ButtonSizes.Large)]
    public void BakeAsOff()
    {
        ArokaAnimationData arokaAnimationData_off = new ArokaAnimationData(new TransformData(transform), Color);
        animPlan_off.animationData = arokaAnimationData_off;
    }
    [PropertySpace(30, 30)]
    [ButtonGroup("3")]
    [Button("After 미리보기", ButtonSizes.Large)]
    public void PreviewAfterStatus()
    {
        SetArokaAnimationData(animPlan_after.animationData);
    }
    [PropertySpace(30, 30)]
    [ButtonGroup("3")]
    [Button("이 상태를 After로 베이크", ButtonSizes.Large)]
    public void BakeAsAfter()
    {
        ArokaAnimationData arokaAnimationData_after = new ArokaAnimationData(new TransformData(transform), Color);
        animPlan_after.animationData = arokaAnimationData_after;
    }



}
