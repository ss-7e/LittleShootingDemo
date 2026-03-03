using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    [System.Serializable]
    public class CameraShakeSettings : ICameraModuleSettings
    {
        public bool Enable { get; private set; } = true;
        public float ShakeStrengthScale = 1f; //震动强度缩放
    }
    private class CameraShakeModule : ICameraControlModule
    {
        private class ShakeInstance
        {
            public float Duration {get; private set; }
            public float Strength { get; private set; }
            public float Timer { get; private set; }
            public ShakeInstance(float strength, float duration)
            {
                Duration = duration;
                Strength = strength;
                Timer = duration;
            }
            public Vector3 Shake()
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    float shakeProgress = 1 - (Timer / Duration);
                    float currentStrength = Mathf.Lerp(Strength, 0, shakeProgress);
                    return Random.insideUnitSphere * currentStrength; //后续可能改为三角函数做周期震动
                }
                else
                {
                    Timer = 0;
                }
                return Vector3.zero;
            }
        }
        
        public CameraShakeModule(ICameraModuleSettings cameraModuleSettings)
        {
            _shakeSettings = cameraModuleSettings as CameraShakeSettings;
            EventManager.Instance.OnPlayerShoot += TriggerShake;
            EventManager.Instance.OnExplode += TriggerShake;
        }

        CameraShakeSettings _shakeSettings;
        List<ShakeInstance> _shakeList = new(); 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="strength"></param>
        public void TriggerShake(Vector3 shakeDirection, float strength, float duration)
        {
            _shakeList.Add(new ShakeInstance(strength * _shakeSettings.ShakeStrengthScale, duration));
        }
        public void UpdateState(CameraControl camera)
        {
            for (int i = _shakeList.Count - 1; i >= 0; i--)
            {
                ShakeInstance shake = _shakeList[i];
                Vector3 shakeOffset = shake.Shake();
                camera._targetPos += shakeOffset;
                camera.transform.position += shakeOffset;
                if (shake.Timer <= 0)
                {
                    _shakeList.Remove(shake);
                }
            }
        }
    }
}
