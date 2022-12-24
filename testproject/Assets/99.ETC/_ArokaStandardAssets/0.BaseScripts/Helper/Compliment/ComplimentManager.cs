using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public enum ComplimentName
{
    Perfect,
    Excellent,
    Awesome,
    Good,
    Good2
}

public class ComplimentManager : MonoBehaviour
{
    #region singleTone
    private static ComplimentManager _instance = null;
    public static ComplimentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(ComplimentManager)) as ComplimentManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    [System.Serializable]
    public class ComplimentPlan
    {
        public ComplimentName complimentName;
        public Sprite sprite;

        public GameObject InstancePlay(float destroySec = 3f)
        {
            GameObject inst_ment = Instantiate(ComplimentManager.Instance.complimentPrefab, ComplimentManager.Instance.complimentParent);
            inst_ment.GetComponent<Image>().sprite = sprite;
            inst_ment.GetComponent<Image>().SetNativeSize();
            Destroy(inst_ment, destroySec);
            return inst_ment;
        }
    }

    public ComplimentPlan[] complimentPlans;
    public GameObject complimentPrefab;
    public Transform complimentParent;

    [MinMaxSlider(-180, 180, true)]
    public Vector2 randomDegree;

    float RandomDegree => Random.Range(randomDegree.x, randomDegree.y);

    public void PlayComplimentRandom()
    {
        int randInt = Random.Range(0, complimentPlans.Length);
        GetComplimentPlan((ComplimentName)randInt).InstancePlay().transform.localRotation = Quaternion.Euler(new Vector3(0, 0, RandomDegree));
    }

    ComplimentPlan GetComplimentPlan(ComplimentName complimentName)
    {
        for (int i = 0; i < complimentPlans.Length; i++)
        {
            if (complimentName == complimentPlans[i].complimentName)
            {
                return complimentPlans[i];
            }
        }
        Debug.LogWarning("그런 complimentPlan 없어요");
        return null;
    }



}