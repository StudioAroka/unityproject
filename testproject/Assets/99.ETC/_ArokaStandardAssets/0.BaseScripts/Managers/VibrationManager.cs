using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    #region singleTone
    private static VibrationManager _instance = null;
    public static VibrationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(VibrationManager)) as VibrationManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    public void Vibrate(float delayTime = 0f)
    {
        StartCoroutine(VibrateRoutine(delayTime));
    }
    
    IEnumerator VibrateRoutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Taptic.Light();
    }

    public IEnumerator ClearVibrationRoutine()
    {
        Vibrate();
        yield return new WaitForSeconds(.3f);
        Vibrate();
        yield return new WaitForSeconds(.3f);
        Vibrate();
        yield return new WaitForSeconds(.3f);
        Vibrate();
        yield return new WaitForSeconds(.3f);
        Vibrate();
        yield return new WaitForSeconds(.3f);
    }
}
