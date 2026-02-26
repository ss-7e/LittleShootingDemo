using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    private class CameraShakingModule : ICameraControlModule
    {
        public void UpdateState(CameraControl camera)
        {
            // Implement camera shaking logic here
        }
    }
}
