using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public partial class CameraControl : MonoBehaviour
{

    private class CameraShakeModule : ICameraControlModule
    {
        private class ShakeInstance
        {
            public float Duration {get; private set; }
            public float Strength { get; private set; }
            public float Timer { get; private set; }
            public ShakeInstance(float duration, float strength)
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
        
        ShakeInstance _currentShake; //暂时这么写，后续可以改成列表支持叠加
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="strength"></param>
        public void TriggerShake(float duration, float strength)
        {
            _currentShake = new ShakeInstance(duration, strength);
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
