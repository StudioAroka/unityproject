using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class DataManager : MonoBehaviour
{
    #region singleTone
    private static DataManager _instance = null;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(DataManager)) as DataManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    public static int LOCALNUMBER_MONEY = 1;
    public static int LOCALNUMBER_STAGE = 1;
    
    public int Money_Local
    {
        get
        {
            string paramName = LOCALNUMBER_MONEY + "Money_Local";
            return PlayerPrefs.GetInt(paramName, 0);
        }
        set
        {
            string paramName = LOCALNUMBER_MONEY + "Money_Local";
            PlayerPrefs.SetInt(paramName, value);
        }
    }
    public int StageIndex_Local
    {
        get
        {
            string paramName = LOCALNUMBER_STAGE + "StageIndex_Local";
            return PlayerPrefs.GetInt(paramName, 0);
        }
        set
        {
            string paramName = LOCALNUMBER_STAGE + "StageIndex_Local";
            PlayerPrefs.SetInt(paramName, value);
        }
    }
    public int StageIndexForUser
    {
        get
        {
            return StageIndex_Local + 1;
        }
    }

    [Button("AddMoney")]
    public void AddMoneyTest()
    {
        AddMoney(10000, true);
    }

    public bool Purchase(int price, bool useRefresh = true)
    {
        if(IsPurchasable(price))
        {
            AddMoney(-price, useRefresh);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddMoney(int moneyAmount, bool useRefresh)
    {
        Money_Local += moneyAmount;
        if (useRefresh)
        {
            UIManager.Instance.RefreshAll();
        }
    }
    public void SetMoney(int moneyAmount, bool useRefresh)
    {
        Money_Local = moneyAmount;
        if (useRefresh)
        {
            UIManager.Instance.RefreshAll();
        }
    }

    public bool IsPurchasable(int price)
    {
        return Money_Local >= price;
    }

}
