using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    // 平滑时间（秒）
    public float moveSmoothTime = 0.1f;

    private Vector3 movement = Vector3.zero;
    private Vector3 inputMoveDir;
    // SmoothDamp 使用的速度跟踪器
    private Vector3 moveDirVelocity = Vector3.zero;

    void Start()
    {
        InputManager.Instance.OnMoveInput += HandleMove;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnMoveInput -= HandleMove;
    }

    void Update()
    {
        // 目标速度：基于输入方向并乘以速度
        Vector3 targetVelocity = inputMoveDir * speed;
        // 平滑过渡到目标速度
        movement = Vector3.SmoothDamp(movement, targetVelocity, ref moveDirVelocity, moveSmoothTime);
        // 应用移动（乘以 Time.deltaTime）
        transform.position += movement * Time.deltaTime;
    }

    private void HandleMove(Vector2 direction)
    {
        //inputMoveDir = new Vector3(direction.x + direction.y, 0f, direction.y - direction.x).normalized;
        inputMoveDir = new Vector3(direction.x, 0f, direction.y).normalized;
    }
}