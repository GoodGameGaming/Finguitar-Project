using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFPS : MonoBehaviour
{
    public int forcedFrameRate = 60;

    private void Awake()
    {
        // QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = forcedFrameRate;
    }
}
