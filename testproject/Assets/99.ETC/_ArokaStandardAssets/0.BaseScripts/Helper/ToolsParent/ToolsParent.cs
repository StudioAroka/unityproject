using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ToolName
{
    None,
    Washer
}
public class ToolsParent : MonoBehaviour
{
    [Title("PRESET")]
    public ToolInfo[] toolInfos;

    [Title("RUNTIME")]
    public ToolInfo nowToolInfo;

    public Vector3 targetPos;
    public Vector3 lerpSpeed = Vector3.one * 16f;

    public void SetTargetPos(Vector3 _pos, bool useLerp)
    {
        if(!useLerp)
            transform.position = _pos;
        targetPos = _pos;

    }

    public void SetLerpSpeed(Vector3 _lerpSpeed)
    {
        lerpSpeed = _lerpSpeed;
    }

    private void Update()
    {
        float lerpedX = Mathf.Lerp(transform.position.x, targetPos.x, lerpSpeed.x * Time.deltaTime);
        float lerpedY = Mathf.Lerp(transform.position.y, targetPos.y, lerpSpeed.y * Time.deltaTime);
        float lerpedZ = Mathf.Lerp(transform.position.z, targetPos.z, lerpSpeed.z * Time.deltaTime);
        transform.position = new Vector3(lerpedX, lerpedY, lerpedZ);
    }

    public ToolInfo GetToolInfo(ToolName toolName)
    {
        for(int i = 0; i < toolInfos.Length; i++)
        {
            if(toolInfos[i].toolName == toolName)
            {
                return toolInfos[i];
            }
        }
        return null;
    }

    public void MakeToolInfo(ToolName toolName, bool usePerformance = true)
    {
        nowToolInfo = GetToolInfo(toolName);

        for(int i = 0; i < toolInfos.Length; i++)
        {
            bool isIdentical = toolInfos[i].toolName == toolName;
            toolInfos[i].SetOn(isIdentical, usePerformance);
        }
    }

    public void InitializeToolsParent()
    {
        MakeToolInfo(ToolName.None, false);
        targetPos = transform.position;
    }
}
