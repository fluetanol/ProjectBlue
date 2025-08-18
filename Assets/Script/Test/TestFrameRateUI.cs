using TMPro;
using UnityEngine;

public class TestFrameRateUI : MonoBehaviour
{

    public TMP_Text FPSText;
    // Update is called once per frame
    private float averageFPS = 0;
    private float fpsUpdateSum = 0;
    private int frameCount = 0;


    void LateUpdate()
    {
        averageFPS = CalculateFPS(ref fpsUpdateSum, ref frameCount);
        FPSText.text = "FPS: " + averageFPS.ToString("F2");
    }

    private float CalculateFPS(ref float fpsUpdateSum, ref int frameCount)
    {
        fpsUpdateSum += (1.0f / Time.unscaledDeltaTime);

        float averageFPS = fpsUpdateSum / ++frameCount;
        return averageFPS;
    }



}
