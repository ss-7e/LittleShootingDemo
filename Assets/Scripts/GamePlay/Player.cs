using System.Collections;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Header("Player Movement Settings")]
    public float Speed = 5f;
    public float MoveSmoothTime = 0.2f;
    public float RotationSmoothTime = 0.1f;
    public float RotationSpeed = 720f; // TODO：根据武器修改？
    public Transform AimerTransform;
    public WeaponBase Weapon; // TODO: 临时编辑器用，后续删掉

    private WeaponBase _currentWeapon;
    private Vector3 _movement = Vector3.zero;
    private Vector3 _inputMoveDir;
    private Vector3 _moveDirVelocity = Vector3.zero;
    private PlayerAim _playerAim;

    void Start()
    {
        if(Weapon != null)
        {
            _currentWeapon = Weapon;
            _currentWeapon.SetOwner(this);
        }
        InputManager.Instance.OnMoveInput += PlayerHandleMove;
        _playerAim = new PlayerAim();
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnMoveInput -= PlayerHandleMove;
    }

    void Update()
    {
        float fowardBoost = -Vector3.Dot(transform.forward, _inputMoveDir) * 0.15f  + 1f; // 往瞄准方向移动时增加速度，远离时减速
        Vector3 targetVelocity = fowardBoost * Speed * _inputMoveDir;
        _movement = Vector3.SmoothDamp(_movement, targetVelocity, ref _moveDirVelocity, MoveSmoothTime);
        transform.position += _movement * Time.deltaTime;
        _playerAim.SetAimRotation(this);
    }

    private void PlayerHandleMove(Vector2 direction)
    {
        _inputMoveDir = new Vector3(direction.x, 0f, direction.y).normalized;
    }
}