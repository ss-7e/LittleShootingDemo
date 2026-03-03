using UnityEngine;
using System;
using System.Xml.Serialization;

/// <summary>
/// 用于管理游戏中的通讯
/// </summary>
/// 
[DefaultExecutionOrder(-100)]
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public event Action<Vector3, float , float> OnPlayerShoot;
    public event Action OnShot;
    public event Action OnBulletHit;

    public void TriggerPlayerShoot(Vector3 fireDircetion, float strength, float lastTime)
    {
        OnPlayerShoot?.Invoke(fireDircetion, strength, lastTime);
        OnShot?.Invoke();
    }

    public void TriggerBulletHit()
    {
        OnBulletHit?.Invoke();
    }
}