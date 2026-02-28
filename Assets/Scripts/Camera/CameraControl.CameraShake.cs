using System.Collections.Generic;
using System.Collections.Specialized;
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
        }

        CameraShakeSettings _shakeSettings;
        ShakeInstance _currentShake; //暂时这么写，后续可以改成列表支持叠加
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="strength"></param>
        public void TriggerShake(Vector3 shakeDirection, float strength, float duration)
        {
            _currentShake = new ShakeInstance(strength * _shakeSettings.ShakeStrengthScale, duration);
        }
        public void UpdateState(CameraControl camera)
        {
            if (_currentShake != null)
            {
                Vector3 shakeOffset = _currentShake.Shake();
                camera._targetPos += shakeOffset;
                camera.transform.position += shakeOffset;
                if (_currentShake.Timer <= 0)
                {
                    _currentShake = null;
                }
            }
        }
    }
}
