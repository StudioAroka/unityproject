using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    #region singleTone
    private static LayerManager _instance = null;
    public static LayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(LayerManager)) as LayerManager;
                if (_instance == null)
                {
                    Debug.Log("?????? ????????");
                }
            }
            return _instance;
        }
    }
    #endregion

    public LayerMask layerMask_hexagon;
}
