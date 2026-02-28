using UnityEngine;
public partial class CameraControl : MonoBehaviour
{
    /// <summary>
    /// 射击后座的相机反馈效果，移动相机往射击方向一点
    /// </summary>
    /// 

    [System.Serializable]
    public class CameraRecoilSettings : ICameraModuleSettings
    {
        public bool Enable { get; private set; } = true;
        public float MaxRecoilOffset = 0.5f; // 最大后座偏移
        public float RecoilScale = 1f; // 后座强度缩放    
    }
    private class CameraRecoilModule : ICameraControlModule
    {
        private CameraRecoilSettings _cameraRecoilSet;
        private Vector3 _recoilDirection;
        private float _recoilTimer;
        private float _recoilDuration;
        private float _recoilStrength;
        public CameraRecoilModule(ICameraModuleSettings setting)
        {
            _cameraRecoilSet = setting as CameraRecoilSettings;
            _recoilTimer = 0f;
            EventManager.Instance.OnPlayerShoot += RecoilTrigger;
        }
        

        //TODO: 目前只能每次覆盖上次的后座
        public void RecoilTrigger(Vector3 recoilDirection, float recoilStrength, float recoilDuration)
        {
            _recoilDuration = recoilDuration;
            _recoilTimer = recoilDuration;
            _recoilStrength = recoilStrength;
            _recoilDirection = recoilDirection.normalized;
        }
        public void UpdateState(CameraControl camera)
        {
            if(_recoilTimer > 0)
            {
                _recoilTimer -= Time.deltaTime;
                float recoilProgress = 1 - (_recoilTimer / _recoilDuration); 
                float currentRecoilOffset = Mathf.Lerp(_cameraRecoilSet.MaxRecoilOffset, 0, recoilProgress) * _recoilStrength * _cameraRecoilSet.RecoilScale;
                camera._targetPos += _recoilDirection * currentRecoilOffset;
                camera.transform.position += _recoilDirection * currentRecoilOffset;
            }
            else
            {
                _recoilTimer = 0f;
            }
        }
    }
}
