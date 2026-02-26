using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraControl : MonoBehaviour
{
    public interface ICameraControlModule
    {
        void UpdateState(CameraControl camera);
    }

    [Header("相机控制参数")]
    public CameraFollowSettings FollowSettings;
    
    private List<ICameraControlModule> _modules = new();
    private void Start()
    {
        Vector3 offset = transform.position - FollowSettings.Target.position;
        _modules.Add(new CameraFollow(FollowSettings.Target, offset));
        _modules.Add(new CameraShakingModule());

    }
    void Update()
    {
        foreach (var module in _modules)
        {
            module.UpdateState(this);
        }
    }
}
