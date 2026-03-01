using UnityEngine;

public class HybridUprightSystem : MonoBehaviour
{
    private Rigidbody _rb;
    private ConfigurableJoint _joint;

    [Header("直立设置")]
    public float UprightStrength = 1000f;
    public bool UseSpring = true;

    [Header("冲击响应")]
    public float impactMultiplier = 1f;
    public float recoveryDelay = 0.5f; // 冲击后恢复延迟
    public Vector3 hitOffset = new Vector3(0, 0.5f, 0); // 冲击点相对于物体中心的偏移

    private float lastImpactTime;
    private bool isRecovering;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        SetupUprightJoint();
    }

    void SetupUprightJoint()
    {
        _joint = gameObject.GetComponent<ConfigurableJoint>();

        // 配置关节
        _joint.rotationDriveMode = RotationDriveMode.Slerp;

        JointDrive slerpDrive = new();
        slerpDrive.positionSpring = UprightStrength;
        slerpDrive.positionDamper = UprightStrength * 0.1f;
        slerpDrive.maximumForce = Mathf.Infinity;
        _joint.slerpDrive = slerpDrive;

        _joint.targetRotation = Quaternion.identity;

        // 允许一定自由度的运动
        _joint.angularXMotion = ConfigurableJointMotion.Free;
        _joint.angularYMotion = ConfigurableJointMotion.Free;
        _joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    // 应用冲击力
    public void ApplyImpact(Vector3 force, Vector3 point)
    {
        point += hitOffset; // 调整冲击点位置
        lastImpactTime = Time.time;
        isRecovering = true;

        // 临时降低直立强度，使冲击效果更明显
        if (UseSpring)
        {
            JointDrive slerpDrive = _joint.slerpDrive;
            slerpDrive.positionSpring = UprightStrength * 0.3f; // 临时降低
            _joint.slerpDrive = slerpDrive;
        }

        // 应用冲击力
        _rb.AddForceAtPosition(force * impactMultiplier, point, ForceMode.Impulse);

        // 开始恢复协程
        StartCoroutine(RecoverUpright());
    }

    System.Collections.IEnumerator RecoverUpright()
    {
        yield return new WaitForSeconds(recoveryDelay);

        // 逐渐恢复直立强度
        float elapsed = 0;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (UseSpring)
            {
                JointDrive slerpDrive = _joint.slerpDrive;
                slerpDrive.positionSpring = Mathf.Lerp(UprightStrength * 0.3f, UprightStrength, t);
                _joint.slerpDrive = slerpDrive;
            }

            yield return null;
        }

        isRecovering = false;
    }
}