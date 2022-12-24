using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using ByteBrewSDK;
public class UIManager : MonoBehaviour
{
    #region singleTone
    private static UIManager _instance = null;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
                if (_instance == null)
                {
                    Debug.Log("싱글톤 예외발생");
                }
            }
            return _instance;
        }
    }
    #endregion

    StageManager stageManager;
    ResourceManager resourceManager;
    [Title("PRESET")]
    public Transform stagePartSelectBtnParent;
    public EventSystem eventSystem;
    public Canvas mainCanvas;
    public Image blackPanel;
    public Camera instagramCamera;
    public Button instagramNextBtn;
    public Image[] starImgs;
    public TextMeshProUGUI stageText;
    public StagePartGuageBar stagePartGuageBar;

    public StagePartSelectBtn stagePartSelectBtnPrefab;


    private void Awake()
    {
        stageManager = StageManager.Instance;
        resourceManager = ResourceManager.Instance;
        blackPanel.gameObject.SetActive(true);
    }
    public IEnumerator BlackPanelRoutine(float targetAlpha, float totalTime)
    {
        blackPanel.gameObject.SetActive(true);
        if (totalTime == 0f)
        {
            blackPanel.color = blackPanel.color.ModifiedAlpha(targetAlpha);
            yield break;
        }
        else
        {
            float initialAlpha = blackPanel.color.a;
            float accumTime = 0f;
            while (true)
            {
                accumTime += Time.deltaTime;
                float perone = accumTime / totalTime;
                float tmpAlpha = Mathf.Lerp(initialAlpha, targetAlpha, accumTime / totalTime);
                blackPanel.color = blackPanel.color.ModifiedAlpha(tmpAlpha);
                if (perone >= 1)
                {
                    break;
                }
                yield return null;
            }
        }
    }
    public void OnClickedResetBtn()
    {
        bool isProperScene = GameManager.Instance.NowSceneName == SceneName.InGame && StageManager.Instance.NowStepName == StepName.Puzzle;
        if (!isProperScene)
        {
            return;
        }
        Stage stage = StageManager.Instance.nowStage;
        stage.nowSelectedStagePart.ResetStagePart();
        VibrationManager.Instance.Vibrate();
        ByteBrew.NewCustomEvent("ResetBtnClicked", "ResetBtnClicked");
    }

    public void RefreshAll()
    {
        stageText.SetText("WORLD " + DataManager.Instance.StageIndexForUser);
    }
    public void OnClickedNextWorldBtn()
    {
        GameManager.Instance.MakeScene(SceneName.LevelClearAfter);
        VibrationManager.Instance.Vibrate();
    }
}