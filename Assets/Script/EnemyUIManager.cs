
using UnityEngine;
using UnityEngine.UI;


public class EnemyUIManager : MonoBehaviour
{
    public Transform followTarget;
    public Vector3 UIOffset = new Vector3(0, 1.5f, 0); // UI 오프셋

    public Image EnemyHealthBarImg; // 적의 체력바 이미지
    public Image EnemyHealthBarEntireImg;

    [SerializeField] private Enemy _enemy;
    private IEnemyData _enemyData;

    void Awake()
    {
        _enemyData = _enemy;

        if (_enemyData == null)
        {
            Debug.LogError("EnemyData not found in parent or children of this object.");
            return;
        }
    }

    void LateUpdate()
    {
        SetHealthBar();
        SetHealthBarTransform();
    }

    private void SetHealthBar()
    {
        EnemyHealthBarImg.fillAmount = _enemyData.RestHealth / _enemyData.MaxHealth;
    }

    private void SetHealthBarTransform()
    {
        Vector3 followPosition = followTarget.position;
        followPosition.y = 0;
        transform.position = followTarget.position + UIOffset;

        Vector3 camDir = Camera.main.transform.forward;
        EnemyHealthBarEntireImg.transform.forward = camDir;
    }
}
