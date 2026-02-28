using Unity.VisualScripting;
using UnityEngine;

public class SimpleGun : WeaponBase
{
    public float FireRate = 5f;   // 每秒射击次数
    public float RecoilForce = 5f; // 后坐力强度
    public float SeperateRadius = 0.3f; // 子弹分散半径
    public GameObject BulletPrefab; // 子弹预制体
    public Transform FirePoint;     // 枪口
    public BulletData BulletDataSetting = new()
    {
        Speed = 25f,
        Damage = 10,
        Lifetime = 2f,
        Direction = Vector3.zero
    };                              // 子弹数据

    private bool _shooting = false;
    private float _fireTimeGap = 0f;
    private Vector3 _defaltLocalPosition;

    public override void OnWeaponAim()
    {
        base.OnWeaponAim();

    }

    private void Start()
    {
        _defaltLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_fireTimeGap >= 1f / FireRate)
        {
            if (_shooting)
            {
                Shoot();
            }
        }
        else 
        { 
            _fireTimeGap += Time.deltaTime;
        }
        if(transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _defaltLocalPosition, 10f * Time.deltaTime);
        }
    }
    private bool WeaponAimed()
    {
        Vector3 mouseAimPos = InputManager.MouseInput.MouseWorldPosition;
        Vector3 aimDirection = mouseAimPos - FirePoint.position;
        if (aimDirection.sqrMagnitude < 0.01f)
        {
            return false; // 避免零向量
        }
        aimDirection.y = 0;
        float distance = aimDirection.magnitude;
        aimDirection = aimDirection.normalized;
        if ((aimDirection + transform.forward).magnitude < 0.1f)
        {
            Vector3 randomOffset = Random.insideUnitSphere * SeperateRadius;
            BulletDataSetting.Direction = -transform.forward * distance + randomOffset;
            BulletDataSetting.Direction.y = 0; 
            return true;
        }
        else if ((aimDirection + transform.forward).magnitude < 0.4f)
        {
            Vector3 randomOffset = Random.insideUnitSphere * SeperateRadius * 0.5f;
            BulletDataSetting.Direction = -transform.forward * distance + randomOffset;
            BulletDataSetting.Direction.y = 0;
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        if(!WeaponAimed())
        {
            return;
        }

        // 实例化子弹 TODO：改成对象池？
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);

        EventManager.Instance.TriggerPlayerShoot(BulletDataSetting.Direction.normalized, RecoilForce, 0.08f);
        SimpleBullet baseBullet = bullet.GetComponent<SimpleBullet>();
        baseBullet.SimpleBulletData = BulletDataSetting;
        _fireTimeGap = 0f;
        WeaponeRecoil();
    }

    protected virtual void WeaponeRecoil()
    {
        transform.localPosition += new Vector3(0, 0, 1) * RecoilForce * 0.25f;
        Owner.transform.position += transform.forward * RecoilForce * 0.05f;
    }
    protected override void OnStartShoot()
    {
        _shooting = true;
    }

    protected override void OnStopShoot()
    {
        _shooting = false;
    }
}
