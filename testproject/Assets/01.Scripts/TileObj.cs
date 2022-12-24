using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class TileObj : MonoBehaviour
{
    [Title("PRESET")]
    public int possibilityInt = 1;
    public bool decoInstallAvailable = true;
    public bool isRotateAvailable = true;
    [Title("RUNTIME")]
    public bool nowDecoInstalled;


    public void InitializeTileObj()
    {
        nowDecoInstalled = false;
    }

    /*
    private void Reset()
    {
        gameObject.AddComponent<ArokaGraphicHelper>();
        gameObject.GetComponent<ArokaGraphicHelper>().Modify();
    }
    
    [Button("DAF")]
    public void Test()
    {
        transform.GetChild(0).transform.localPosition = Vector3.zero;
        transform.GetChild(0).transform.localScale = Vector3.one;
        transform.GetChild(0).transform.localRotation = Quaternion.identity;
    }
    */
}
