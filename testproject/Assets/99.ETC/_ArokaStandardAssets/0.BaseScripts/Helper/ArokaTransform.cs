using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ArokaTransform : MonoBehaviour
{
    Coroutine nowPosRoutine;
    Coroutine nowRotRoutine;
    Coroutine nowScaleRoutine;
    Coroutine nowActiveRoutine;

    Coroutine nowColorRoutine;



    #region POSITION
    public void SetPos(Vector3 targetPos, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0)
    {
        SetPosStop();
        if(totalTime == 0f)
        {
            transform.position = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(true, transform.parent, targetPos, totalTime, curvName, Vector3.zero, CurvName.None, delayTime));
        }
    }
    public void SetLocalPos(Vector3 targetPos, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0)
    {
        SetPosStop();
        if (totalTime == 0f)
        {
            transform.localPosition = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(false, transform.parent, targetPos, totalTime, curvName, Vector3.zero, CurvName.None, delayTime));
        }
    }
    public void SetLocalPosWithParent(Vector3 targetPos, Transform parent, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0)
    {
        SetPosStop();
        if (totalTime == 0f)
        {
            transform.SetParent(parent);
            transform.localPosition = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(false, parent, targetPos, totalTime, curvName, Vector3.zero, CurvName.None, delayTime));
        }
    }
    public void SetPosWithCurv(Vector3 targetPos, float totalTime, CurvName curvName, Vector3 offsetMultiplier, CurvName offsetCurv, float delayTime = 0)
    {
        SetPosStop();
        if (totalTime == 0f)
        {
            transform.position = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(true, transform.parent, targetPos, totalTime, curvName, offsetMultiplier, offsetCurv, delayTime));
        }
    }
    public void SetLocalPosWithCurv(Vector3 targetPos, float totalTime, CurvName curvName, Vector3 offsetMultiplier, CurvName offsetCurv, float delayTime = 0)
    {
        SetPosStop();
        if (totalTime == 0f)
        {
            transform.localPosition = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(false, transform.parent, targetPos, totalTime, curvName, offsetMultiplier, offsetCurv, delayTime));
        }
          
    }
    public void SetLocalPosWithCurv_WithParent(Vector3 targetPos, Transform parent, float totalTime, CurvName curvName, Vector3 offsetMultiplier, CurvName offsetCurv, float delayTime = 0)
    {
        SetPosStop();
        if(totalTime == 0f)
        {
            transform.SetParent(parent);
            transform.localPosition = targetPos;
        }
        else
        {
            nowPosRoutine = StartCoroutine(SetPosRoutine(false, parent, targetPos, totalTime, curvName, offsetMultiplier, offsetCurv, delayTime));
        }
    }

    public void SetPosStop()
    {
        if (nowPosRoutine != null)
        {
            StopCoroutine(nowPosRoutine);
        }
    }

    //MOVE 함수
    private IEnumerator SetPosRoutine(bool isWorld, Transform parent, Vector3 targetLocalPos, float totalTime, CurvName curvName = CurvName.EaseOut, Vector3 offsetMultiplier = default, CurvName offsetCurvName = CurvName.None, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        transform.SetParent(parent);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        AnimationCurve offsetCurv = CurveManager.Instance.GetCurvPlan(offsetCurvName).animCurv;

        Vector3 initialLocalPos = isWorld ? transform.position : transform.localPosition;
        float accumTime = 0;
        while (gameObject)
        {
            accumTime += Time.deltaTime;
            float perone = totalTime == 0 ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);
            Vector3 offset = offsetCurv.Evaluate(curvPerone) * offsetMultiplier;

            if (isWorld)
            {
                transform.position = Vector3.Lerp(initialLocalPos, targetLocalPos + offset, curvPerone);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(initialLocalPos, targetLocalPos + offset, curvPerone);
            }
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }
    #endregion

    #region Scale
    public void SetLocalScale(Vector3 targetScale, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetLocalScaleStop();
        if (totalTime == 0f)
        {
            transform.localScale = targetScale;
        }
        else 
        {
            nowScaleRoutine = StartCoroutine(SetScaleRoutine(transform.parent, targetScale, totalTime, curvName, Vector3.zero, CurvName.None, delayTime));
        }
    }
    public void SetLocalScaleWithCurv(Vector3 targetScale, float totalTime, CurvName curvName, Vector3 offsetMultiplier, CurvName offsetCurv, float delayTime)
    {
        SetLocalScaleStop();
        if(totalTime == 0f)
        {
            transform.localScale = targetScale;
        }
        else
        {
            nowScaleRoutine = StartCoroutine(SetScaleRoutine(transform.parent, targetScale, totalTime, curvName, offsetMultiplier, offsetCurv, delayTime));
        }
    }

    public void SetLocalScaleWithParent(Vector3 targetScale, Transform parent, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetLocalScaleStop();
        if(totalTime == 0f)
        {
            transform.SetParent(parent);
            transform.localScale = targetScale;
        }
        else
        {
            nowScaleRoutine = StartCoroutine(SetScaleRoutine(parent, targetScale, totalTime, curvName, Vector3.zero, CurvName.None, delayTime));
        }
    }

    public void SetLocalScaleStop()
    {
        if (nowScaleRoutine != null)
        {
            StopCoroutine(nowScaleRoutine);
        }
    }
    private IEnumerator SetScaleRoutine(Transform parent, Vector3 targetScale, float totalTime, CurvName curvName = CurvName.EaseOut, Vector3 offsetMultiplier = default, CurvName offsetCurvName = CurvName.None, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        transform.SetParent(parent);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        AnimationCurve offsetCurv = CurveManager.Instance.GetCurvPlan(offsetCurvName).animCurv;
        Vector3 initialLocalScale = transform.localScale;
        float accumTime = 0;
        while (transform)
        {
            accumTime += Time.deltaTime;
            float perone = totalTime == 0 ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);
            Vector3 offset = offsetCurv.Evaluate(curvPerone) * offsetMultiplier;

            transform.localScale = Vector3.LerpUnclamped(initialLocalScale, targetScale + offset, curvPerone);
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }
    #endregion

    #region Rotation

    public void SetRot(Quaternion targetRot, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetRotStop();
        if(totalTime == 0f)
        {
            transform.rotation = targetRot;
        }
        else
        {
            nowRotRoutine = StartCoroutine(SetRotRoutine(true, targetRot, totalTime, curvName, delayTime));
        }
    }
    public void SetLocalRot(Quaternion targetRot, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetRotStop();
        if (totalTime == 0f)
        {
            transform.localRotation = targetRot;
        }
        else
        {
            nowRotRoutine = StartCoroutine(SetRotRoutine(false, targetRot, totalTime, curvName, delayTime));
        }
            
    }
    public void SetRotEuler(Vector3 targetEuler, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetRot(Quaternion.Euler(targetEuler), totalTime , curvName , delayTime);
    }
    public void SetLocalRotEuler(Vector3 targetEuler, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        SetLocalRot(Quaternion.Euler(targetEuler), totalTime, curvName, delayTime);
    }

    public void SetRotInfinity(Vector3 dirVec, float speed, Space space = Space.Self)
    {
        SetRotStop();
        nowRotRoutine = StartCoroutine(SetRotInfinityRoutine(dirVec, speed, space));
    }
    public void SetRotStop()
    {
        if (nowRotRoutine != null)
        {
            StopCoroutine(nowRotRoutine);
        }
    }

    private IEnumerator SetRotRoutine(bool isWorld, Quaternion targeRot, float totalTime, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        Quaternion initialRot = isWorld ? transform.rotation : transform.localRotation;
        float accumTime = 0;
        while (transform)
        {
            accumTime += Time.deltaTime;
            float perone = (totalTime == 0f) ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);
            if (isWorld)
            {
                transform.rotation = Quaternion.Lerp(initialRot, targeRot, curvPerone);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(initialRot, targeRot, curvPerone);
            }
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }


    private IEnumerator SetRotInfinityRoutine(Vector3 direction, float speed, Space space = Space.Self, bool isFixedUpdate = false)
    {
        if (isFixedUpdate)
        {
            WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
            while (gameObject)
            {
                transform.Rotate(direction, Time.deltaTime * speed, space);
                yield return fixedUpdate;
            }
        }
        else
        {
            while (gameObject)
            {
                transform.Rotate(direction, Time.deltaTime * speed, space);
                yield return null;
            }
        }
    }

    public void SetLocalRotDegree(Vector3 degreeDir, float totalTime, CurvName curvName)
    {
        SetRotStop();
        nowRotRoutine = StartCoroutine(SetLocalRotDegreeRoutine(degreeDir, totalTime, curvName));
    }

    private IEnumerator SetLocalRotDegreeRoutine(Vector3 degreeDir,float totalTime, CurvName curvName)
    {
        Quaternion initialRot = transform.localRotation;
        AnimationCurve  animationCurve= CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        float accumTime = 0f;
        while (true)
        {
            accumTime += Time.deltaTime;
            float perone = accumTime / totalTime;
            float curvPerone = animationCurve.Evaluate(perone);
            Quaternion tmpRot = initialRot * Quaternion.Euler(Vector3.Lerp(Vector3.zero,degreeDir, curvPerone));
            transform.localRotation = tmpRot;

            if(perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }


    #endregion

    #region Color
    public void SetMeshColor(Color targetColor, float totalTime = 0f, CurvName curvName = CurvName.Linear, float delayTime = 0f)
    {
        List<int> materialIndexList = new List<int>() { 0 };
        SetMeshColor(targetColor, materialIndexList, totalTime, curvName, delayTime);;
    }
    public void SetMeshColor(Color targetColor, List<int> materialIndexList, float totalTime = 0f, CurvName curvName = CurvName.Linear, float delayTime = 0f)
    {
        SetColorStop();
        nowColorRoutine = StartCoroutine(SetMeshColorRoutine(targetColor, materialIndexList, totalTime, curvName, delayTime));
    }
    public void SetImgColor(Color targetColor, float totalTime = 0f, CurvName curvName = CurvName.Linear, float delayTime = 0f)
    {
        SetColorStop();
        nowColorRoutine = StartCoroutine(SetImgColorRoutine(targetColor, totalTime, curvName, delayTime));
    }
    public void SetColorStop()
    {
        if (nowColorRoutine != null)
        {
            StopCoroutine(nowColorRoutine);
        }
    }
    private IEnumerator SetImgColorRoutine(Color targetColor, float totalTime, CurvName curvName = CurvName.Linear, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        Image img = GetComponent<Image>();
        Color initialColor = img.color;
        float accumTime = 0;

        while (transform)
        {
            accumTime += Time.deltaTime;
            float perone = totalTime == 0 ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);

            img.color = Color.LerpUnclamped(initialColor, targetColor, curvPerone);
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }
    private IEnumerator SetMeshColorRoutine(Color targetColor, List<int> materialIndexList, float totalTime, CurvName curvName = CurvName.Linear, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        List<Color> initialColors = new List<Color>();
        MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
        for(int i = 0; i < materialIndexList.Count; i++)
        {
            int materialIndex = materialIndexList[i];
            initialColors.Add(meshRenderer.materials[materialIndex].color);
        }
        float accumTime = 0;
        
        while (transform)
        {
            accumTime += Time.deltaTime;
            float perone = totalTime == 0 ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);

            for(int i = 0; i < materialIndexList.Count; i++)
            {
                meshRenderer.materials[i].color = Color.LerpUnclamped(initialColors[i], targetColor, curvPerone);
            }
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }
    #endregion 

    #region TRANSFORM
    public void SetTransform(TransformData trData, float totalTime, CurvName curvName, float delayTime = 0f)
    {
        SetTransformStop();
        SetPos(trData.localPos, totalTime, curvName, delayTime);
        SetRot(trData.localRot, totalTime, curvName, delayTime);
        SetLocalScale(trData.localScale, totalTime, curvName, delayTime);
    }
    public void SetLocalTransform(TransformData trData, float totalTime, CurvName curvName, float delayTime = 0f)
    {
        SetTransformStop();
        SetLocalPos(trData.localPos, totalTime, curvName, delayTime);
        SetLocalRot(trData.localRot, totalTime, curvName, delayTime);
        SetLocalScale(trData.localScale, totalTime, curvName, delayTime);
    }
    public void SetLocalTransform(Transform parent, Vector3 localPos, Quaternion localRot, Vector3 localScale, float totalTime, CurvName curvName, float delayTime = 0f)
    {
        SetTransformStop();
        transform.SetParent(parent);
        SetLocalPos(localPos, totalTime, curvName, delayTime);
        SetLocalRot(localRot, totalTime, curvName, delayTime);
        SetLocalScale(localScale, totalTime, curvName, delayTime);
    }
    public void SetTransformStop()
    {
        SetPosStop();
        SetLocalScaleStop();
        SetRotStop();
    }
    #endregion


    #region ETC
    public void SetActiveWithDelay(bool b, float delay, Transform afterParent)
    {
        if (nowActiveRoutine != null)
        {
            StopCoroutine(nowActiveRoutine);
        }
        nowActiveRoutine = StartCoroutine(SetActiveWithDelayRoutine(b, delay, afterParent));
    }
    public void SetActiveWithDelay(bool b, float delay)
    {
        if (nowActiveRoutine != null)
        {
            StopCoroutine(nowActiveRoutine);
        }
        nowActiveRoutine = StartCoroutine(SetActiveWithDelayRoutine(b, delay, transform.parent));
    }
    private IEnumerator SetActiveWithDelayRoutine(bool b, float delay, Transform afterParent)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(b);
        transform.SetParent(afterParent);
    }

    public Coroutine nowParentRoutine;
    public void SetParentWithDelay(bool b, Transform targetParent,float delay)
    {
        if (nowParentRoutine != null)
        {
            StopCoroutine(nowParentRoutine);
        }
        nowParentRoutine = StartCoroutine(SetParentWithDelayRoutine(b, targetParent, delay));
    }
    private IEnumerator SetParentWithDelayRoutine(bool b, Transform targetParent, float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.SetParent(targetParent);
    }
    #endregion




}