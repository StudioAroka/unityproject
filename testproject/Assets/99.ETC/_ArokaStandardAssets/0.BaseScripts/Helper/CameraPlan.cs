using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraPlan : MonoBehaviour
{
    public int cameraIndex;
    [ReadOnly]
    public List<CameraData> cameraDatas = new List<CameraData>();
    public CameraData NowCameraData => cameraDatas[cameraIndex];

    public void InitializeCameraPlan()
    {
        cameraIndex = 0;
    }

    [Button("Add Camera", ButtonSizes.Large)]
    public void AddCameraData()
    {
        CameraData cameraData = new GameObject("CameraData " + transform.childCount).AddComponent<CameraData>();
        cameraData.transform.SetParent(transform);
        cameraData.transform.localPosition = Camera.main.transform.localPosition;
        cameraData.transform.localRotation = Camera.main.transform.localRotation;
        cameraDatas.Add(cameraData);
    }

    [Button("Remove Camera", ButtonSizes.Large)]
    public void RemoveCameraData()
    {
        DestroyImmediate(cameraDatas[cameraDatas.Count - 1].gameObject);
        cameraDatas.RemoveAt(cameraDatas.Count - 1);
    }
}
