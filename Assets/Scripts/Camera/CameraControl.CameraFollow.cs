using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    [System.Serializable]
    public class CameraFollowSettings
    {
        public bool Enable = true;
        public Transform Target;
        public float SmoothTime = 0.3f;
        public bool LookAtTarget = false;
    }

    public class CameraFollow : ICameraControlModule
    {
        public float SmoothSpeed = 0.125f;
        public float AheadDistance = 2f;


        private Transform _target;
        private Vector3 _offset;

        public CameraFollow(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        public void UpdateState(CameraControl camera)
        {
            
            if (_target != null)
            {
                camera.transform.position = _target.position + _offset;
                camera.transform.LookAt(_target);
            }
        }
    }
}
