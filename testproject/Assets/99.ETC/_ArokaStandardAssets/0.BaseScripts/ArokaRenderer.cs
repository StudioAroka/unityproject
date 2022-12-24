using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArokaRenderer : MonoBehaviour
{
    Renderer Rend => GetComponent<Renderer>();

    public class MatData
    {
        public Renderer hostRend;
        public int matIndex;
        public Material Mat => hostRend.materials[matIndex];
        public Material SharedMat => hostRend.sharedMaterials[matIndex];
        public Coroutine nowColorRoutine;

        public MatData(Renderer _hostRend, int _matIndex)
        {
            hostRend = _hostRend;
            matIndex = _matIndex;
        }
    }
    public List<MatData> matDatas;

    public MatData GetMatData(int matIndex)
    {
        for(int i = 0; i < matDatas.Count; i++)
        {
            if(matDatas[i].matIndex == matIndex)
            {
                return matDatas[i];
            }
        }
        return null;
    }

    void Register()
    {
        matDatas = new List<MatData>();
        for(int i = 0; i < Rend.materials.Length; i++)
        {
            MatData matDataToAdd = new MatData(Rend, i);

            matDatas.Add(matDataToAdd);
        }
    }


    #region COLOR

    private void Start()
    {
        Register();
    }
    public void SetSharedColor(Color targetColor, float totalTime = 0f, int matIndex = 0, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        MatData matData = GetMatData(matIndex);
        SetColorStop(matData.matIndex);
        matData.nowColorRoutine = StartCoroutine(SetColorRoutine(matData.SharedMat, targetColor, totalTime, curvName, delayTime));

    }
    public void SetColor(Color targetColor, float totalTime = 0f, int matIndex = 0, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        MatData matData = GetMatData(matIndex);
        SetColorStop(matData.matIndex);
        matData.nowColorRoutine = StartCoroutine(SetColorRoutine(matData.Mat, targetColor, totalTime, curvName, delayTime));
    }
    public void SetColorAll(Color targetColor, float totalTime = 0f, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        for(int i = 0; i < matDatas.Count; i++)
        {
            SetColor(targetColor, totalTime, matDatas[i].matIndex, curvName, delayTime);
        }
    }
    public void SetColorStop(int matIndex)
    {
        MatData matData = GetMatData(matIndex);
        if(matData.nowColorRoutine != null)
        {
            StopCoroutine(matData.nowColorRoutine);
        }
    }
    public void SetColorAllStop()
    {
        for (int i = 0; i < matDatas.Count; i++)
        {
            SetColorStop(matDatas[i].matIndex);
        }
    }
    #endregion

    private IEnumerator SetColorRoutine(Material mat, Color targetColor, float totalTime, CurvName curvName = CurvName.EaseOut, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delayTime);
        AnimationCurve animCurv = CurveManager.Instance.GetCurvPlan(curvName).animCurv;
        Color initialColor = mat.color;
        float accumTime = 0;
        while (transform)
        {
            accumTime += Time.deltaTime;
            float perone = totalTime == 0 ? 1f : Mathf.Clamp01(accumTime / totalTime);
            float curvPerone = animCurv.Evaluate(perone);
            mat.color = Color.Lerp(initialColor, targetColor, curvPerone);
            if (perone >= 1)
            {
                break;
            }
            yield return null;
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SetColor(Random.ColorHSV(), 1f);
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            SetColorAllStop();
        }
    }
}
