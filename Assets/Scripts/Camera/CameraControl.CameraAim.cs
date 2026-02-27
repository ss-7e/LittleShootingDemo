using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    /// <summary>
    /// 将相机往瞄准方向偏移一段，提供更多视野
    /// TODO: 直接添加offset/根据象限切换offset
    /// </summary>
    /// 
    public enum CameraAimMode
    {
        AllDirection,
        FourDimention,
        EightDimention
    }
    [System.Serializable]
    public class CameraAimSettings : ICameraModuleSettings
    {
        public CameraAimMode AimMode;
        public bool Enable { get; private set; } = true;
        [Range(0, 0.5f)]
        public float AimOffsetScaleX = 0.1f;
        [Range(0, 1f)]
        public float AimOffsetScaleY = 0.1f;
        public float BoostAim = 2f; //右键瞄准增加移动倍率
    }
    private class CameraAimModule : ICameraControlModule
    {
        private CameraAimSettings _cameraAimSet;
        public CameraAimModule(ICameraModuleSettings setting)
        {
            _cameraAimSet = setting as CameraAimSettings;
        }

        public void UpdateState(CameraControl camera)
        {
            Vector2 mouseOffset = new(); 
            switch (_cameraAimSet.AimMode)
            {
                case CameraAimMode.AllDirection:
                    mouseOffset = AimAllDirection();
                    break;
                case CameraAimMode.FourDimention:
                    mouseOffset = AimFourDirection();
                    break;
                case CameraAimMode.EightDimention:
                    mouseOffset = AimEightDirection();
                    break;

            }
            Vector3 cameraOffset = new(mouseOffset.x * _cameraAimSet.AimOffsetScaleX, 0f, mouseOffset.y * _cameraAimSet.AimOffsetScaleY);
            camera._targetPos += cameraOffset; 
        }

        private Vector2 AimAllDirection()
        {
            return (InputManager.MouseInput.MouseScreenPosition - new Vector2(Screen.width, Screen.height) * 0.5f) * 0.05f;
        }

        private Vector2 AimFourDirection()
        {
            Vector2 mousePos = InputManager.MouseInput.MouseScreenPosition;
            float x = mousePos.x > Screen.width * 0.5f ? Screen.width * 0.025f : -Screen.width * 0.025f;
            float y = mousePos.y > Screen.height * 0.5f ? Screen.height * 0.025f : -Screen.height * 0.025f;
            return new Vector2(x, y);
        }

        private Vector2 AimEightDirection()
        {
            Vector2 mousePos = InputManager.MouseInput.MouseScreenPosition;
            float x = 0, y = 0;
            if(mousePos.x > Screen.width * 0.5f)
            {
                if(mousePos.x > Screen.width * 0.75f)
                {
                    x = Screen.width * 0.025f;
                }
                else
                {
                    x = Screen.width * 0.0125f;
                }
            }
            else
            {
                if(mousePos.x < Screen.width * 0.25f)
                {
                    x = -Screen.width * 0.025f;
                }
                else
                {
                    x = -Screen.width * 0.0125f;
                }
            }
            if(mousePos.y > Screen.height * 0.5f)
            {
                if(mousePos.y > Screen.height * 0.75f)
                {
                    y = Screen.height * 0.025f;
                }
                else
                {
                    y = Screen.height * 0.0125f;
                }
            }
            else
            {
                if(mousePos.y < Screen.height * 0.25f)
                {
                    y = -Screen.height * 0.025f;
                }
                else
                {
                    y = -Screen.height * 0.0125f;
                }
            }
            return new Vector2(x, y);
        }
    }
}
