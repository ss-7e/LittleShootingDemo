using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    [System.Serializable]
    public class CameraFollowSettings : ICameraModuleSettings
    {
        public bool Enable { get; private set; } = true;
        public Transform Target;
        public bool LookAtTarget = false;
    }

    public class CameraFollow : ICameraControlModule
    {
        public float SmoothSpeed = 0.125f;
        public float AheadDistance = 2f;

        private ICameraModuleSettings _cameraFollowSet;
        private Transform _followTarget;
        private Vector3 _offset;

        public CameraFollow(Transform target, Vector3 offset)
        {
            _followTarget = target;
            _offset = offset;
        }

        public void UpdateState(CameraControl camera)
        {
            if (_followTarget != null)
            {
                camera._targetPos = _followTarget.position + _offset;
            }
        }
    }
}
