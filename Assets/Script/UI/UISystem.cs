using System;
using DG.Tweening;
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
    public Image ESkillDurationImg;
    public Image QSkillDurationImg;
    public Image HeatlthIndicatorImg;

    public TMP_Text ESkillCoolText;
    public TMP_Text QSkillCoolText;
    public TMP_Text ESkillDurationText;
    public TMP_Text QSkillDurationText;
    public TMP_Text HealthIndicatorText;

    public Button QuitButton;

    private GameObject _UIOwnerPlayer;

    
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
            Debug.Log("Quit Button Clicked");
        });
    }

    private void Start()
    {
        HealthIndicatorText.text =
        Mathf.Max(0, _basicData.currentHP).ToString("F0") + " / " +
        _basicData.maxHP.ToString("F0") + " HP";
    }



    void LateUpdate()
    {
        // print("elapsed : " + _skillTimeData.ECoolTimeElapsed + " " + _skillTimeData.EDurationElapsedTime);
        CoolTimeUpdate();
        // HealthUpdate();
        DurationTimeUpdate();
    }



    private void CoolTimeUpdate()
    {

        if (_skillTimeData.ECoolTimeElapsed >= 0 && _skillTimeData.EDurationElapsedTime == 0)
        {
            ESkillCoolImg.fillAmount =
            1 - (_skillTimeData.ECoolTimeElapsed / _skillTimeData.ECoolTime);

            ESkillCoolText.text =
            _skillTimeData.ECoolTimeElapsed == 0f ? "E" :
            _skillTimeData.ECoolTimeElapsed.ToString("F1") + "s";
        }

        if (_skillTimeData.QCoolTimeElapsed >= 0 && _skillTimeData.QDurationElapsedTime == 0)
        {
            QSkillCoolImg.fillAmount =
        1 - _skillTimeData.QCoolTimeElapsed / _skillTimeData.QCoolTime;

            QSkillCoolText.text =
            _skillTimeData.QCoolTimeElapsed == 0f ? "Q" :
            _skillTimeData.QCoolTimeElapsed.ToString("F1") + "s";
        }   
    }

    private void DurationTimeUpdate()
    {
        if (_skillTimeData.EDurationElapsedTime > 0)
        {
            ESkillDurationImg.fillAmount =
            1 - (_skillTimeData.EDurationElapsedTime / _skillTimeData.EDuration);

            ESkillDurationText.text =
            (_skillTimeData.EDuration - _skillTimeData.EDurationElapsedTime).ToString("F1") + "s";
        }
        else
        {
            ESkillDurationImg.fillAmount = 0;
            //ESkillDurationText.text = "E";
        }
        if (_skillTimeData.QDurationElapsedTime > 0)
        {

        }

    }

    public void HealthUpdate()
    {
        if (HeatlthIndicatorImg.fillAmount == 0)
        {
            return;
        }

        DOTween.To(() => HeatlthIndicatorImg.fillAmount,
        x => HeatlthIndicatorImg.fillAmount = x,
        Mathf.Max(0, _basicData.currentHP / _basicData.maxHP),
        0.1f).SetEase(Ease.Flash);

        //HeatlthIndicatorImg.fillAmount = Mathf.Max(0, _basicData.currentHP / _basicData.maxHP);

        HealthIndicatorText.text =
        Mathf.Max(0, _basicData.currentHP).ToString("F0") + " / " +
        _basicData.maxHP.ToString("F0") + " HP";
    }
}
