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



    private Vector3 _movement = Vector3.zero;
    private Vector3 _inputMoveDir;
    private Vector3 _moveDirVelocity = Vector3.zero;
    private PlayerAim _playerAim;

    void Start()
    {
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
        Vector3 targetVelocity = _inputMoveDir * Speed;
        _movement = Vector3.SmoothDamp(_movement, targetVelocity, ref _moveDirVelocity, MoveSmoothTime);
        transform.position += _movement * Time.deltaTime;
        _playerAim.SetAimRotation(this);
    }

    private void PlayerHandleMove(Vector2 direction)
    {
        _inputMoveDir = new Vector3(direction.x, 0f, direction.y).normalized;
    }
}