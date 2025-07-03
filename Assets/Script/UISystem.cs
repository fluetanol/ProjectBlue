using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface UISystemData
{
    public ISkillTimeData _skillTimeData
    {  set; }
    public IBasicData _basicData
    {  set;   }
}



public class UISystem : MonoBehaviour, UISystemData
{
    public static UISystem Instance;

    public Canvas Canvas;
    public Image ESkillCoolImg;
    public Image QSkillCoolImg;
    public Image HeatlthIndicatorImg;

    public TMP_Text ESkillCoolText;
    public TMP_Text QSkillCoolText;
    public TMP_Text HealthIndicatorText;

    private GameObject _UIOwnerPlayer;

    public Image EnemyHealthBarImg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ISkillTimeData _skillTimeData
    {
        private get;
        set;
    }

    public IBasicData _basicData
    {
        private get;
        set;
    }

    public void LateUpdate()
    {
        CoolTimeUpdate();
        HealthUpdate();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void CoolTimeUpdate()
    {
        ESkillCoolImg.fillAmount =
        1 - _skillTimeData.ECoolTimeElapsed / _skillTimeData.ECoolTime;

        QSkillCoolImg.fillAmount =
        1 - _skillTimeData.QCoolTimeElapsed / _skillTimeData.QCoolTime;


        ESkillCoolText.text =
        _skillTimeData.ECoolTimeElapsed == 0f ? "E" :
        _skillTimeData.ECoolTimeElapsed.ToString("F1") + "s";

        QSkillCoolText.text =
        _skillTimeData.QCoolTimeElapsed == 0f ? "Q" :
        _skillTimeData.QCoolTimeElapsed.ToString("F1") + "s";
    }

    private void HealthUpdate()
    {
        HeatlthIndicatorImg.fillAmount = Mathf.Max(0, _basicData.currentHP / _basicData.maxHP);

        HealthIndicatorText.text =
        Mathf.Max(0, _basicData.currentHP).ToString("F0") + " / " +
        _basicData.maxHP.ToString("F0") + " HP";
    }


    private void EnemyHealthUpdate()
    {

    }

    public void OutOfRangeEnemyHealthBar()
    {
        if (EnemyHealthBarImg != null)
        {
            EnemyHealthBarImg.gameObject.SetActive(false);
        }
    }

    public void UpdateEnemyHealthBar(Vector3 screenPosition)
    {
        if (!EnemyHealthBarImg.IsActive())
        {
            EnemyHealthBarImg.gameObject.SetActive(true);
        }

        RectTransform canvasRect = Canvas.GetComponent<RectTransform>();
        RectTransform enemyHealthBarRect = EnemyHealthBarImg.GetComponent<RectTransform>();

        // screen point를 RectTransform의 로컬 포인트로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)Canvas.transform,
            screenPosition,
            Canvas.worldCamera,
            out Vector2 localPoint))
        {
            enemyHealthBarRect.anchoredPosition = localPoint;
        }
    }
    
}
