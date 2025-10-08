using TMPro;
using UnityEngine;

public class DebugUiSystem : MonoBehaviour
{
    public static DebugUiSystem Instance;

    [Header("FPS Display")]
    [SerializeField] private TMP_Text _fpsText;
    public float updateInterval = 0.5f;
    private float accum = 0; // FPS 계산을 위한 누적 시간
    private int frames = 0; // FPS 계산을 위한 누적 프레임 수
    private float timeleft; // 시간 간격

    [Header("Monster")]
    [SerializeField] private TMP_Text _monsterCountText;
    [SerializeField] private IPoolDebugData _poolDebugData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _poolDebugData = EnemyPoolManager.Instance;

        if (_fpsText == null)
        {
            Debug.LogWarning("FPS Text UI is not assigned.");
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {
        CalculateFps();
        DebugMonsterCount();
    }


    void CalculateFps()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // 일정 시간 간격마다 FPS 계산 및 표시
        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            if (_fpsText != null)
            {
                _fpsText.text = format; // Text UI에 표시
            }
            else
            {
                Debug.Log(format); // 콘솔에 표시
            }

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }


    void DebugMonsterCount()
    {
        
        if (_poolDebugData != null)
        {
            _monsterCountText.text = $"Active Monsters: {_poolDebugData.ActiveCount}";
        }

    }
}
