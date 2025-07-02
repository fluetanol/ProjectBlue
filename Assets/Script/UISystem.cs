using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public Image ESkillCoolImg;
    public Image QSkillCoolImg;

    public TMP_Text ESkillCoolText;
    public TMP_Text QSkillCoolText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ISkillTimeData _skillTimeData;


    private void Awake()
    {
        _skillTimeData = GetComponentInParent<ISkillTimeData>();
        if (_skillTimeData == null)
        {
            Debug.LogError("ISkillTimeData component is missing on the GameObject.");
        }
    }

    public void Update()
    {
        ESkillCoolImg.fillAmount =
        1 - _skillTimeData.ECoolTimeElapsed / _skillTimeData.ECoolTime;

        QSkillCoolImg.fillAmount =
        1 - _skillTimeData.QCoolTimeElapsed / _skillTimeData.QCoolTime;


        ESkillCoolText.text =
        _skillTimeData.ECoolTimeElapsed == 0f ? "E":
        _skillTimeData.ECoolTimeElapsed.ToString("F1") + "s";

        QSkillCoolText.text =
        _skillTimeData.QCoolTimeElapsed == 0f ? "Q" :
        _skillTimeData.QCoolTimeElapsed.ToString("F1") + "s";

    }
}
