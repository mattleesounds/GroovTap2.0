using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class AudioSessionManager : MonoBehaviour
{
    public static AudioSessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSession();
    }

    private void InitializeAudioSession()
    {
#if UNITY_IOS
        _SetAudioSession();
#endif
    }

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _SetAudioSession();
#endif
}