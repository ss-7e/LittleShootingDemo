using UnityEngine;
using System;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    // 单例
    public static InputManager Instance { get; private set; }
    public static MouseAimManager MouseInput {  get; private set; }

    // 移动方向向量（归一化）
    public Vector2 InputVector2D { get; private set; }

    // 事件
    public event Action<Vector2> OnMoveInput;
    public event Action OnJump;
    public event Action OnSprint;
    public event Action OnInteract;
    public event Action OnShoot;
    public event Action OnStopShoot;

    // 按键配置（直接公开，方便修改）
    [Header("按键设置")]
    public KeyCode ForwardKey = KeyCode.W;
    public KeyCode BackwardKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;
    public KeyCode Power = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public KeyCode InteractKey = KeyCode.E;
    public KeyCode ShootKey = KeyCode.Mouse0;
    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        MouseInput = new MouseAimManager();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // 处理移动输入
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(ForwardKey)) vertical += 1;
        if (Input.GetKey(BackwardKey)) vertical -= 1;
        if (Input.GetKey(LeftKey)) horizontal -= 1;
        if (Input.GetKey(RightKey)) horizontal += 1;

        Vector2 newMoveDir = new Vector2(horizontal, vertical).normalized;

        // 只有当输入变化或不为零时才触发事件
        if (newMoveDir != InputVector2D)
        {
            InputVector2D = newMoveDir;
            OnMoveInput(newMoveDir);
        }

        // 处理动作输入（按下触发一次）
        if (Input.GetKeyDown(Power))
            OnJump?.Invoke();

        if (Input.GetKeyDown(SprintKey))
            OnSprint?.Invoke();

        if (Input.GetKeyDown(InteractKey))
            OnInteract?.Invoke();

        if (Input.GetKeyDown(ShootKey))
            OnShoot?.Invoke();
        
        if(Input.GetKeyUp(ShootKey))
            OnStopShoot?.Invoke();

        MouseInput.Update();
    }

    // 如果需要修改按键，直接在这里改或者在Inspector面板改
    public void SetKey(string action, KeyCode newKey)
    {
        switch (action)
        {
            case "forward": ForwardKey = newKey; break;
            case "backward": BackwardKey = newKey; break;
            case "left": LeftKey = newKey; break;
            case "right": RightKey = newKey; break;
            case "jump": Power = newKey; break;
            case "sprint": SprintKey = newKey; break;
            case "interact": InteractKey = newKey; break;
            case "shoot": ShootKey = newKey; break;
        }
    }
}