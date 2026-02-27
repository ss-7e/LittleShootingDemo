using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    public interface ICameraControlModule
    {
        void UpdateState(CameraControl camera);
    }
    
    public interface ICameraModuleSettings
    {
        bool Enable { get;}
    }

    [Header("相机控制参数")]
    public CameraFollowSettings FollowSettings;
    public CameraAimSettings AimSettings;
    public CameraRecoilSettings RecoilSettings;
    public float SmoothTime = 0.5f;


    private List<ICameraControlModule> _modules = new();
    private Vector3 _targetPos;
    private void Start()
    {
        Vector3 offset = transform.position - FollowSettings.Target.position;
        _targetPos = transform.position;
        _modules.Add(new CameraFollow(FollowSettings.Target, offset));
        _modules.Add(new CameraShakeModule());
        _modules.Add(new CameraAimModule(AimSettings));
        _modules.Add(new CameraRecoilModule(RecoilSettings));
    }
    // TODO：这么写似乎会争夺控制？
    void Update()
    {
        foreach (var module in _modules)
        {
            module.UpdateState(this);
            transform.position = Vector3.Lerp(transform.position, _targetPos, SmoothTime * Time.deltaTime);
        }
    }
}
