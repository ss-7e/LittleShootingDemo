using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Player Owner { get; private set; }

    private void OnEnable()
    {
        InputManager.Instance.OnShoot += OnStartShoot;
        InputManager.Instance.OnStopShoot += OnStopShoot;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnShoot -= OnStartShoot;
        InputManager.Instance.OnStopShoot -= OnStopShoot;
    }


    protected abstract void OnStartShoot();

    protected abstract void OnStopShoot();

    /// <summary>
    /// TODO: 目前瞄准是近似的（武器不在转动中心），后续更新一些数学计算
    public virtual void OnWeaponAim()
    {

    }
    public void SetOwner(Player player)
    {
        Owner = player;
    }
}
