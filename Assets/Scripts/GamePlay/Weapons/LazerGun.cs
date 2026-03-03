using UnityEngine;

public class LazerGun : WeaponBase
{
    public GameObject LazerLinePrefab;
    public Transform ShootPoint;
    public float MaxPowerDPS;
    public float MaxPoewerTime;


    private float _currentPowerDPS;
    private float _currentPowerTime;
    private IHitable _currentTarget;
    private bool _isShooting = false;
    protected override void OnStartShoot()
    {
        _isShooting = true;
        _currentPowerDPS = 0;
        _currentPowerTime = 0;
    }
    protected override void OnStopShoot()
    {
        _isShooting = false;
    }
    void Update()
    {
        if (_isShooting)
        {
            LazerShot();
        }
    }
    private void LazerShot()
    {
        Ray ray = new(ShootPoint.position, ShootPoint.forward);
        Physics.Raycast(ray, out RaycastHit hit, 100f);
        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            hitObject.TryGetComponent<IHitable>(out var target); 
            if (_currentTarget != target)
            {
                _currentTarget = target;
                _currentPowerTime = 0;
            }
            else
            {
                target.OnHit(ray.direction, _currentPowerDPS * Time.deltaTime);
                _currentPowerTime += Time.deltaTime;
                _currentPowerDPS = Mathf.Lerp(0, MaxPowerDPS, _currentPowerTime / MaxPoewerTime);
            }
        }
        else
        {
            _currentTarget = null;
            _currentPowerTime = 0;
        }
    }

    private void CreateLazerLine()
    {
        GameObject lazerLine = Instantiate(LazerLinePrefab, ShootPoint.position, Quaternion.identity);
        lazerLine.transform.forward = ShootPoint.forward;
    }
}