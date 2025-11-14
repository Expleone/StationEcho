using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    public int targetFPS = 180;

    void Awake()
    {
        // Set the target frame rate
        Application.targetFrameRate = targetFPS;
    }
}