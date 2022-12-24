using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.EventSystems;

public class SimpleAnim : MonoBehaviour
{
    private Transform tr;
    private Image img;
    private RawImage rawImg;
    private TextMeshProUGUI textMesh;
    private Button ThisButton => GetComponent<Button>();
    private EventTrigger ThisEventTrigger => GetComponent<EventTrigger>();
    private bool isContainTextmesh;
    private bool isContainImage;
    private bool isContainRawImage;
    private bool IsContainButton => GetComponent<Button>() != null;
    private bool IsContainEventTrigger => GetComponent<EventTrigger>() != null;
    bool IsOnlyAfterAnim => IsContainSituationEnum(AnimCurv_SituationEnum.After) && !IsContainSituationEnum(AnimCurv_SituationEnum.On) && !IsContainSituationEnum(AnimCurv_SituationEnum.Off);
    bool IsOnlyOnAnim => IsContainSituationEnum(AnimCurv_SituationEnum.On) && !IsContainSituationEnum(AnimCurv_SituationEnum.Off) && !IsContainSituationEnum(AnimCurv_SituationEnum.After);

    public List<AnimCurvPlan> animCurvPlans = new List<AnimCurvPlan>();
    public IEnumerator nowAnimRoutine;

    [HideInInspector]
    public bool isOn;
    [HideInInspector]
    public bool isAnimChanging;

    public enum AnimCurv_ControlThingEnum
    {
        LocalScale = 0,
        Alpha = 1,
        PositionX = 2,
        PositionY = 3,
        PositionZ = 4
    }
    public enum AnimCurv_SituationEnum
    {
        On,
        Off,
        After
    }
    [System.Serializable]
    public class StandardValue
    {
        public AnimCurv_ControlThingEnum controlThingEnums;
        public float standardValue;
        public StandardValue(AnimCurv_ControlThingEnum _animCurvControlThingEnum, float _standardValue)
        {
            controlThingEnums = _animCurvControlThingEnum;
            standardValue = _standardValue;
        }
    }
    [System.Serializable]
    public class AnimCurvPlan
    {
        public AnimCurv_SituationEnum animCurv_situationEnum;
        public List<AnimCurvInfo> animCurvInfos = new List<AnimCurvInfo>(); // on, off, after

        public float LongestAnimCurvTime
        {
            get
            {
                float longestTime = animCurvInfos[0].TotalAnimTime;
                for (int i = 0; i < animCurvInfos.Count; i++)
                {
                    if (animCurvInfos[i].TotalAnimTime > longestTime)
                    {
                        longestTime = animCurvInfos[i].TotalAnimTime;
                    }
                }
                return longestTime;
            }
        }
        public bool IsAllAnimCurvsCompleted
        {
            get
            {
                for (int i = 0; i < animCurvInfos.Count; i++)
                {
                    if (!animCurvInfos[i].AnimCurvCompleted)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [HideInInspector]
        public List<StandardValue> standardValues_start = new List<StandardValue>();
        [HideInInspector]
        public List<StandardValue> standardValues_end = new List<StandardValue>();
        public void MakeAllAnimCurvDeltimePerone(float f)
        {
            for (int i = 0; i < animCurvInfos.Count; i++)
            {
                animCurvInfos[i].deltimePerone = f;
            }
        }

        public AnimCurvInfo GetAnimCurvInfo(AnimCurv_ControlThingEnum _animCurv_controlThing)
        {
            for (int i = 0; i < animCurvInfos.Count; i++)
            {
                if (animCurvInfos[i].animCurv_controlThing == _animCurv_controlThing)
                {
                    return animCurvInfos[i];
                }
            }
            return null;
        }

        public bool ContainControlThing(AnimCurv_ControlThingEnum _animCurv_ControlThingEnum)
        {

            for (int i = 0; i < animCurvInfos.Count; i++)
            {
                if (animCurvInfos[i].animCurv_controlThing == _animCurv_ControlThingEnum)
                {
                    return true;
                }
            }
            return false;
        }

        public AnimCurvInfo GetLongestAnimCurvInfo()
        {
            AnimCurvInfo longestAnimCurvInfo = animCurvInfos[0];
            for (int i = 0; i < animCurvInfos.Count; i++)
            {
                if (animCurvInfos[i].TotalAnimTime > longestAnimCurvInfo.TotalAnimTime)
                {
                    longestAnimCurvInfo = animCurvInfos[i];
                }
            }
            return longestAnimCurvInfo;
        }

        public float LongestTotalAnimTimeOfPlan => GetLongestAnimCurvInfo().TotalAnimTime;


        private bool IsOnAnimCurvAnim => animCurv_situationEnum == AnimCurv_SituationEnum.On;
        private bool IsOffAnimCurvAnim => animCurv_situationEnum == AnimCurv_SituationEnum.Off;
        private bool IsAfterAnimCurvPlan => animCurv_situationEnum == AnimCurv_SituationEnum.After;
        private bool IsShowLoopingCount => IsAfterAnimCurvPlan && !useInfiniteLoop;
        [ShowIf("IsOnAnimCurvAnim")]
        [Range(0, 10)]
        public int onDelaySec;
        [ShowIf("IsOffAnimCurvAnim")]
        [Range(0, 10)]
        public int offDelaySec;
        [ShowIf("IsAfterAnimCurvPlan")]
        public bool useInfiniteLoop;
        [ShowIf("IsShowLoopingCount")]
        public int loopingCount;

    }
    public AnimCurvPlan GetAnimCurvPlan(AnimCurv_SituationEnum _animCurv_SituationEnum)
    {
        for (int i = 0; i < animCurvPlans.Count; i++)
        {
            if (animCurvPlans[i].animCurv_situationEnum == _animCurv_SituationEnum)
            {
                return animCurvPlans[i];
            }
        }
        return null;
    }


    [System.Serializable]
    public class AnimCurvInfo
    {
        public AnimCurv_ControlThingEnum animCurv_controlThing;
        public AnimationCurve animCurv;
        public float deltimePerone = 0;
        public float playSpeedPerone;
        public float TotalAnimTime => 1;
        [HideInInspector]
        public bool AnimCurvCompleted => deltimePerone >= 1;

        public AnimCurvInfo(AnimCurv_ControlThingEnum _animCurv_controlThing, AnimationCurve _animCurv)
        {
            animCurv_controlThing = _animCurv_controlThing;
            animCurv = _animCurv;
        }
        /*
        */
        public AnimationCurve CopiedAnimCurv(bool isReversedCopy)
        {
            AnimationCurve newAnimCurv = new AnimationCurve();
            int animCurvFrom_keyCount = animCurv.length;
            for (int i = newAnimCurv.length - 1; i >= 0; i--)
            {
                newAnimCurv.RemoveKey(i);
            }
            for (int i = 0; i < animCurvFrom_keyCount; ++i)
            {
                Keyframe keyFrame;
                if (isReversedCopy)
                {
                    keyFrame = new Keyframe(animCurv.keys[i].time, animCurv.keys[animCurvFrom_keyCount - 1 - i].value);
                }
                else
                {
                    keyFrame = animCurv.keys[i];
                }
                newAnimCurv.AddKey(keyFrame);
            }
            for (int i = 0; i < newAnimCurv.keys.Length; ++i)
            {
                newAnimCurv.SmoothTangents(i, 0); //zero weight means average
            }
            return newAnimCurv;
        }
    }
    //
    private void Awake()
    {
        SetEndStatusAndBake();
        SetStartStatus();
    }
    private void Start()
    {
        if (IsOnlyAfterAnim)
        {
            SetAnim(true);
        }
    }
    private void OnDestroy()
    {
        SetAnim(false);
        if (nowAnimRoutine != null)
            StopCoroutine(nowAnimRoutine);
    }
    void GetThisUIDiskProperties()
    {
        tr = transform;

        img = tr.GetComponent<Image>();
        rawImg = tr.GetComponent<RawImage>();
        textMesh = tr.GetComponent<TextMeshProUGUI>();

        isContainImage = img != null;
        isContainRawImage = rawImg != null;
        isContainTextmesh = textMesh != null;
    }
    public void SetAnim(bool _isOn, bool useSmooth = false)
    {
        GetThisUIDiskProperties();
        if (IsOnlyOnAnim)
        {
            if (nowAnimRoutine != null)
            {
                StopCoroutine(nowAnimRoutine);
            }
            if (gameObject.activeInHierarchy)
            {
                IEnumerator routineToPlay = PlayUIDiskAnimRoutine(_isOn, useSmooth);
                nowAnimRoutine = routineToPlay;
                StartCoroutine(routineToPlay);
            }
        }
        else
        {
            if (_isOn != isOn)
            {
                if (nowAnimRoutine != null)
                {
                    StopCoroutine(nowAnimRoutine);
                }
                if (gameObject.activeInHierarchy)
                {
                    IEnumerator routineToPlay = PlayUIDiskAnimRoutine(_isOn, useSmooth);
                    nowAnimRoutine = routineToPlay;
                    StartCoroutine(routineToPlay);
                }
            }
        }
    }
    private IEnumerator PlayUIDiskAnimRoutine(bool _isOn, bool useSmooth)
    {
        isOn = _isOn;
        isAnimChanging = true;
        SetButtonAndEventTriggerRaycastActive(false);

        AnimCurv_SituationEnum properSituationEnum = _isOn ? AnimCurv_SituationEnum.On : AnimCurv_SituationEnum.Off;
        AnimCurvPlan properAnimCurvPlan = GetAnimCurvPlan(properSituationEnum);


        if (properAnimCurvPlan != null)
        {
            //On, Off Curve
            properAnimCurvPlan.MakeAllAnimCurvDeltimePerone(0);

            if (useSmooth)
                yield return StartCoroutine(MakeSmoothAnimChangingPerformance(properAnimCurvPlan.standardValues_start, .15f));

            //On 혹은 Off 시작 초기화
            InitializeTrByStandardValue(properAnimCurvPlan.standardValues_start);

            yield return new WaitForSeconds(_isOn ? properAnimCurvPlan.onDelaySec : properAnimCurvPlan.offDelaySec);
            while (true)
            {
                for (int i = 0; i < properAnimCurvPlan.animCurvInfos.Count; i++)
                {
                    AnimCurvInfo animCurvInfo = properAnimCurvPlan.animCurvInfos[i];
                    animCurvInfo.deltimePerone += Time.smoothDeltaTime * animCurvInfo.playSpeedPerone;
                    UpdateThisAnimCurv(animCurvInfo, animCurvInfo.deltimePerone);

                    if (animCurvInfo.AnimCurvCompleted)
                    {
                        continue;
                    }
                }
                if (properAnimCurvPlan.IsAllAnimCurvsCompleted)
                {
                    break;
                }
                yield return null;
            }
        }
        isAnimChanging = false;
        SetButtonAndEventTriggerRaycastActive(_isOn);
        if (IsOnlyOnAnim)
        {
            isOn = false;
        }

        AnimCurvPlan afterCurvPlan = GetAnimCurvPlan(AnimCurv_SituationEnum.After);
        if (isOn && afterCurvPlan != null)
        {
            if (afterCurvPlan.useInfiniteLoop)
            {
                while (true)
                    yield return StartCoroutine(AfterRoutine());
            }
            else
            {
                for (int i = 0; i < afterCurvPlan.loopingCount; i++)
                    yield return StartCoroutine(AfterRoutine());
            }
        }
    }
    private IEnumerator AfterRoutine()
    {
        AnimCurvPlan properAnimCurvPlan = GetAnimCurvPlan(AnimCurv_SituationEnum.After);
        if (properAnimCurvPlan != null)
        {
            //On, Off Curve
            properAnimCurvPlan.MakeAllAnimCurvDeltimePerone(0);

            //    yield return StartCoroutine(MakeSmoothAnimChangingPerformance(properAnimCurvPlan.standardValues_start, .15f));

            //On 혹은 Off 시작 초기화
            //    InitializeTrByStandardValue(properAnimCurvPlan.standardValues_start);

            while (true)
            {
                for (int i = 0; i < properAnimCurvPlan.animCurvInfos.Count; i++)
                {
                    AnimCurvInfo animCurvInfo = properAnimCurvPlan.animCurvInfos[i];
                    animCurvInfo.deltimePerone += Time.smoothDeltaTime * animCurvInfo.playSpeedPerone;
                    UpdateThisAnimCurv(animCurvInfo, animCurvInfo.deltimePerone);

                    if (animCurvInfo.AnimCurvCompleted)
                    {
                        animCurvInfo.deltimePerone = 0;
                        continue;
                    }
                }
                if (!isOn)
                {
                    // Debug.Log("애프터 루틴 탈출");
                    break;
                }
                yield return null;
            }
        }
    }
    IEnumerator MakeSmoothAnimChangingPerformance(List<StandardValue> standardValuesTo, float totalTime)
    {
        int enumCount = System.Enum.GetValues(typeof(AnimCurv_ControlThingEnum)).Length;
        float delTime = 0;
        List<float> initialStandardValueList = new List<float>();
        for (int i = 0; i < enumCount; i++)
        {
            AnimCurv_ControlThingEnum thisControlThing = (AnimCurv_ControlThingEnum)i;
            initialStandardValueList.Add(GetNowStandardValue(thisControlThing));
        }
        while (true)
        {
            //  Debug.Log("on 의 끝과 after의 시작을 "+ delTime + " 보정중입니다");
            delTime += Time.smoothDeltaTime;
            for (int i = 0; i < enumCount; i++)
            {
                AnimCurv_ControlThingEnum thisControlThing = (AnimCurv_ControlThingEnum)i;
                float tmpValue = Mathf.Lerp(initialStandardValueList[i], standardValuesTo[i].standardValue, delTime / totalTime);
                MakeAnimCurvTr(thisControlThing, tmpValue);
            }

            if (delTime > totalTime)
            {
                break;
            }
            yield return null;
        }
    }
    [Button("반전복사", ButtonSizes.Large)]
    public void CopyAnimCurvPlanReversed()
    {
        ReplaceAnimPlan(AnimCurv_SituationEnum.On, AnimCurv_SituationEnum.Off, true);
    }

    private void ReplaceAnimPlan(AnimCurv_SituationEnum _animCurv_Situation_from, AnimCurv_SituationEnum _animCurv_Situation_to, bool isReversedCopy)
    {
        AnimCurvPlan animCurvPlanFrom = GetAnimCurvPlan(_animCurv_Situation_from);
        AnimCurvPlan animCurvPlanTo = GetAnimCurvPlan(_animCurv_Situation_to);

        animCurvPlanTo.animCurvInfos.Clear();
        for (int i = 0; i < animCurvPlanFrom.animCurvInfos.Count; i++)
        {
            AnimCurvInfo animCurvInfoToAdd =
                new AnimCurvInfo
                (animCurvPlanFrom.animCurvInfos[i].animCurv_controlThing, // localScale, alpha, rotation, position, etc...
                animCurvPlanFrom.animCurvInfos[i].CopiedAnimCurv(isReversedCopy)); // animCurv

            animCurvPlanTo.animCurvInfos.Add(animCurvInfoToAdd);
        }
    }

    public void UpdateAnimCurvPlan(AnimCurvPlan animCurvPlan, float delTime)
    {
        for (int i = 0; i < animCurvPlan.animCurvInfos.Count; i++)
        {
            UpdateThisAnimCurv(animCurvPlan.animCurvInfos[i], delTime);
        }
    }
    public void UpdateThisAnimCurv(AnimCurvInfo animCurvInfo, float delTime)
    {
        for (int i = 0; i < typeof(AnimCurv_ControlThingEnum).EnumCount(); i++)
        {
            if (animCurvInfo.animCurv_controlThing == (AnimCurv_ControlThingEnum)i)
                MakeAnimCurvTr((AnimCurv_ControlThingEnum)i, animCurvInfo.animCurv.Evaluate(delTime));
        }
    }
    void MakeAnimCurvTr(AnimCurv_ControlThingEnum animCurv_ControlThingEnum, float targetValue)
    {
        switch (animCurv_ControlThingEnum)
        {
            case AnimCurv_ControlThingEnum.LocalScale:
                tr.localScale = (Vector3.one * targetValue).ModifiedZ(1f);
                break;
            case AnimCurv_ControlThingEnum.Alpha:
                if (isContainTextmesh)
                {
                    textMesh.color = textMesh.color.ModifiedAlpha(targetValue);
                }
                else if (isContainImage)
                {
                    img.color = img.color.ModifiedAlpha(targetValue);
                }
                else if (isContainRawImage)
                {
                    rawImg.color = rawImg.color.ModifiedAlpha(targetValue);
                }
                else
                {
                }
                break;
            case AnimCurv_ControlThingEnum.PositionX:
                float evalPosX = targetValue;
                if (isContainTextmesh)
                {
                    textMesh.rectTransform.anchoredPosition = textMesh.rectTransform.anchoredPosition.ModifiedX(evalPosX);
                }
                else if (isContainImage)
                {
                    img.rectTransform.anchoredPosition = img.rectTransform.anchoredPosition.ModifiedX(evalPosX);
                }
                else if (isContainRawImage)
                {
                    rawImg.rectTransform.anchoredPosition = rawImg.rectTransform.anchoredPosition.ModifiedX(evalPosX);
                }
                else
                {
                    transform.localPosition = transform.localPosition.ModifiedY(evalPosX);
                }
                break;
            case AnimCurv_ControlThingEnum.PositionY:
                float evalPosY = targetValue;
                if (isContainTextmesh)
                {
                    textMesh.rectTransform.anchoredPosition = textMesh.rectTransform.anchoredPosition.ModifiedY(evalPosY);
                }
                else if (isContainImage)
                {
                    img.rectTransform.anchoredPosition = img.rectTransform.anchoredPosition.ModifiedY(evalPosY);
                }
                else if (isContainRawImage)
                {
                    rawImg.rectTransform.anchoredPosition = rawImg.rectTransform.anchoredPosition.ModifiedY(evalPosY);
                }
                else
                {
                    transform.localPosition = transform.localPosition.ModifiedY(evalPosY);
                }
                break;
            case AnimCurv_ControlThingEnum.PositionZ:
                float evalPosZ = targetValue;
                if (isContainTextmesh)
                {
                    textMesh.rectTransform.anchoredPosition = textMesh.rectTransform.anchoredPosition.ModifiedZ(evalPosZ);
                }
                else if (isContainImage)
                {
                    img.rectTransform.anchoredPosition = img.rectTransform.anchoredPosition.ModifiedZ(evalPosZ);
                }
                else if (isContainRawImage)
                {
                    rawImg.rectTransform.anchoredPosition = rawImg.rectTransform.anchoredPosition.ModifiedZ(evalPosZ);
                }
                else
                {
                    transform.localPosition = transform.localPosition.ModifiedZ(evalPosZ);
                }
                break;
        }
    }

    public float GetNowStandardValue(AnimCurv_ControlThingEnum _animCurvControlThingEnum)
    {
        switch (_animCurvControlThingEnum)
        {
            case AnimCurv_ControlThingEnum.LocalScale:
                return transform.localScale.x;
            case AnimCurv_ControlThingEnum.Alpha:
                if (isContainTextmesh)
                    return textMesh.color.a;
                else if (isContainImage)
                    return img.color.a;
                else if (isContainRawImage)
                {
                    return rawImg.color.a;
                }
                else
                    return 1;
            case AnimCurv_ControlThingEnum.PositionX:
                if (isContainTextmesh)
                    return textMesh.rectTransform.anchoredPosition.x;
                else if (isContainImage)
                    return img.rectTransform.anchoredPosition.x;
                else if (isContainRawImage)
                {
                    return rawImg.rectTransform.anchoredPosition.x;
                }
                else
                    return transform.position.x;
            case AnimCurv_ControlThingEnum.PositionY:
                if (isContainTextmesh)
                    return textMesh.rectTransform.anchoredPosition.y;
                else if (isContainImage)
                    return img.rectTransform.anchoredPosition.y;
                else if (isContainRawImage)
                {
                    return rawImg.rectTransform.anchoredPosition.y;
                }
                else
                    return transform.position.y;
            case AnimCurv_ControlThingEnum.PositionZ:
                if (isContainTextmesh)
                    return 0;
                else if (isContainImage)
                    return 0;
                else if (isContainRawImage)
                    return 0;
                else
                    return transform.position.z;
            default:
                return 0;
        }
    }

    public void InitializeTrByStandardValue(List<StandardValue> standardValues)
    {
        for (int i = 0; i < standardValues.Count; i++)
        {
            MakeAnimCurvTr(standardValues[i].controlThingEnums, standardValues[i].standardValue);
        }
    }
    [Button("중간 모습 미리보기", ButtonSizes.Large)]
    public void SetMiddletatus()
    {
        GetThisUIDiskProperties();

        if (IsOnlyOnAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.On, 0.5f);
        }
        else if (IsOnlyAfterAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.After, 0.5f);
        }
        else
        {
            PreviewAnimStart(AnimCurv_SituationEnum.Off, 0.5f);
        }
    }
    [Button("처음 모습으로 세팅", ButtonSizes.Large)]
    public void SetStartStatus()
    {
        GetThisUIDiskProperties();
        if (IsOnlyOnAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.On, false);
        }
        else if (IsOnlyAfterAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.After, false);
        }
        else
        {
            PreviewAnimStart(AnimCurv_SituationEnum.Off, true);
        }
        SetButtonAndEventTriggerRaycastActive(false);
    }
    [Button("완성 모습으로 세팅", ButtonSizes.Large)]
    public void SetEndStatus()
    {
        GetThisUIDiskProperties();
        if (IsOnlyOnAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.On, true);
        }
        else if (IsOnlyAfterAnim)
        {
            PreviewAnimStart(AnimCurv_SituationEnum.After, true);
        }
        else
        {
            PreviewAnimStart(AnimCurv_SituationEnum.Off, false);
        }
        SetButtonAndEventTriggerRaycastActive(true);
    }
    void SetButtonAndEventTriggerRaycastActive(bool b)
    {
        if (IsContainButton || IsContainEventTrigger)
            img.raycastTarget = b;
    }
    public void PreviewAnimStart(AnimCurv_SituationEnum animCurv_SituationEnum, bool useEndValue)
    {
        PreviewAnimStart(animCurv_SituationEnum, useEndValue ? 1 : 0);
    }
    public void PreviewAnimStart(AnimCurv_SituationEnum animCurv_SituationEnum, float timeValue)
    {
        AnimCurvPlan properAnimCurvPlan = GetAnimCurvPlan(animCurv_SituationEnum);
        if (properAnimCurvPlan == null)
        {
            Debug.Log(transform + "이 이상");
        }
        for (int i = 0; i < properAnimCurvPlan.animCurvInfos.Count; i++)
        {
            UpdateThisAnimCurv(properAnimCurvPlan.animCurvInfos[i], timeValue);
        }
    }

    [Button("완성모습으로 베이크", ButtonSizes.Gigantic)]
    public void SetEndStatusAndBake()
    {
        GetThisUIDiskProperties();
        SetEndStatus();
        for (int k = 0; k < 2; k++)
        {
            for (int i = 0; i < animCurvPlans.Count; i++)
            {
                bool isStartOfAnim = k == 0;
                int enumCount = System.Enum.GetValues(typeof(AnimCurv_ControlThingEnum)).Length;
                List<StandardValue> properStandardValues = isStartOfAnim ? animCurvPlans[i].standardValues_start : animCurvPlans[i].standardValues_end;
                properStandardValues.Clear();
                for (int j = 0; j < enumCount; j++)
                {
                    AnimCurv_ControlThingEnum controlThing = (AnimCurv_ControlThingEnum)j;
                    float properStandardValue;
                    if (animCurvPlans[i].GetAnimCurvInfo(controlThing) != null)
                    {
                        AnimCurvInfo animCurvInfo = animCurvPlans[i].GetAnimCurvInfo(controlThing);
                        properStandardValue = animCurvInfo.animCurv.Evaluate(isStartOfAnim ? 0 : animCurvInfo.TotalAnimTime);
                        if (animCurvInfo.playSpeedPerone == 0)
                            animCurvInfo.playSpeedPerone = 3;
                    }
                    else
                    {
                        switch (controlThing)
                        {
                            case AnimCurv_ControlThingEnum.Alpha:
                                if (isContainTextmesh)
                                    properStandardValue = textMesh.color.a;
                                else if (isContainImage)
                                    properStandardValue = img.color.a;
                                else if (isContainRawImage)
                                    properStandardValue = rawImg.color.a;
                                else
                                    properStandardValue = 1f;
                                break;
                            case AnimCurv_ControlThingEnum.LocalScale:
                                properStandardValue = transform.localScale.x;
                                break;
                            case AnimCurv_ControlThingEnum.PositionX:
                                if (isContainTextmesh)
                                    properStandardValue = textMesh.rectTransform.anchoredPosition.x;
                                else if (isContainImage)
                                    properStandardValue = img.rectTransform.anchoredPosition.x;
                                else if (isContainRawImage)
                                    properStandardValue = rawImg.rectTransform.anchoredPosition.x;
                                else
                                    properStandardValue = transform.localPosition.x;
                                break;
                            case AnimCurv_ControlThingEnum.PositionY:
                                if (isContainTextmesh)
                                    properStandardValue = textMesh.rectTransform.anchoredPosition.y;
                                else if (isContainImage)
                                    properStandardValue = img.rectTransform.anchoredPosition.y;
                                else if (isContainRawImage)
                                    properStandardValue = rawImg.rectTransform.anchoredPosition.y;
                                else
                                    properStandardValue = transform.localPosition.y;
                                break;
                            case AnimCurv_ControlThingEnum.PositionZ:
                                if (isContainTextmesh)
                                    properStandardValue = 0;
                                else if (isContainImage)
                                    properStandardValue = 0;
                                else if (isContainRawImage)
                                    properStandardValue = 0;
                                else
                                    properStandardValue = transform.localPosition.z;
                                break;
                            default:
                                properStandardValue = 0;
                                break;
                        }
                    }
                    properStandardValues.Add(new StandardValue(controlThing, properStandardValue));
                }
            }
        }
    }


    bool IsContainSituationEnum(AnimCurv_SituationEnum _situationEnum)
    {
        for (int i = 0; i < animCurvPlans.Count; i++)
        {
            if (animCurvPlans[i].animCurv_situationEnum == _situationEnum)
            {
                return true;
            }
        }
        return false;
    }
}
