using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CursorUIControl : MonoBehaviour
{
    [Header("光标设置")]
    public RectTransform CursorTransform;  // 光标的RectTransform
    public Canvas ParentCanvas;            // 所在的Canvas
    public float FollowSpeed = 20f;        // 跟随速度（平滑移动用）


    [Header("光标视觉元素")]
    public Image CursorIcon;                        // 光标中心的图标
    public Image LockIndicator;                     // 锁定指示器（扩散光圈）
    public GameObject CursorOnHit;                  // 命中光标效果
    public GameObject CursorCrossLine;              // 准星外部十字线
    public float CrossLineMaxDistance = 200f;       // 准星十字线最大扩散距离
    public float CrossLineDefaltDistance = 120f;    // 准星十字线默认距离
    public ParticleSystem CursorParticles;          // 可选的粒子特效
    public Color DefaltCursorColor;

    [Header("动画参数")]
    public float LockPulseSpeed = 2f;      // 锁定时的脉冲速度
    public float LockMaxScale = 1.5f;       // 锁定光圈最大缩放
    public float CrossExpandSpeed = 10f;      // 准星扩散速度
    public float CrossReduceSpeed = 5f;       // 准星收缩速度

    // 状态管理
    private CursorState _currentState = CursorState.Normal;
    private Vector3 _targetPosition;
    private Coroutine _lockAnimationCoroutine;
    private RectTransform _crosslineRect;
    public enum CursorState
    {
        Normal,
        Hover,      // 悬停
        LockOn,     // 锁定
        Interact    // 可交互
    }

    void Start()
    {
        // 隐藏系统光标
        Cursor.visible = false;

        // 初始化位置
        if (CursorTransform != null)
        {
            _targetPosition = Input.mousePosition;
            CursorTransform.position = _targetPosition;
        }

        if(CursorCrossLine != null)
        {
            _crosslineRect = CursorCrossLine.GetComponent<RectTransform>();
            _crosslineRect.sizeDelta = Vector2.one * CrossLineDefaltDistance;
        }

        // 初始状态为普通
        SetState(CursorState.Normal);
        DefaltCursorColor = CursorIcon.color;
        EventManager.Instance.OnBulletHit += OnBulletHit;
        EventManager.Instance.OnPlayerShoot += OnWeaponFire;
    }

    void Update()
    {
        // 更新目标位置
        _targetPosition = Input.mousePosition;

        if (CursorTransform != null)
        {
            CursorTransform.position = Vector3.Lerp(
                CursorTransform.position,
                _targetPosition,
                Time.deltaTime * FollowSpeed
            );
        }

        if(CursorCrossLine != null)
        {
            _crosslineRect.sizeDelta = Vector2.Lerp(
                _crosslineRect.sizeDelta,
                Vector2.one * CrossLineDefaltDistance,
                Time.deltaTime * CrossReduceSpeed
            );
        }
    }

    public void SetState(CursorState newState, GameObject target = null)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        // 停止所有正在运行的动画协程
        if (_lockAnimationCoroutine != null)
            StopCoroutine(_lockAnimationCoroutine);

        // 根据状态切换视觉表现
        switch (newState)
        {
            case CursorState.Normal:
                CursorIcon.color = DefaltCursorColor;
                CursorIcon.transform.localScale = Vector3.one;
                if (LockIndicator != null)
                    LockIndicator.gameObject.SetActive(false);
                if (CursorParticles != null)
                    CursorParticles.Stop();
                break;

            case CursorState.Hover:
                CursorIcon.color = Color.cyan;
                CursorIcon.transform.localScale = Vector3.one * 1.2f;
                break;

            case CursorState.LockOn:
                CursorIcon.color = Color.red;
                CursorIcon.transform.localScale = Vector3.one;
                if (LockIndicator != null)
                {
                    LockIndicator.gameObject.SetActive(true);
                    // 开始扩散脉冲动画
                    _lockAnimationCoroutine = StartCoroutine(LockPulseAnimation());
                }

                // 可选：让光标跟随锁定目标（如锁定框固定在敌人身上）
                if (target != null)
                {
                    StartCoroutine(FollowTarget(target));
                }
                break;

            case CursorState.Interact:
                CursorIcon.color = Color.green;
                CursorIcon.transform.localScale = Vector3.one * 1.1f;
                break;
        }
    }

    private void OnBulletHit()
    {
        CursorOnHit.SetActive(false);
        CursorOnHit.SetActive(true);
    }

    private void OnWeaponFire(Vector3 _, float accuracyReduction, float lastTime)
    {
        if (CursorCrossLine != null)
        {
            StartCoroutine(ExpandCrossLine(accuracyReduction, lastTime));
        }
    }

    IEnumerator ExpandCrossLine(float amout, float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            _crosslineRect.sizeDelta = Vector2.Lerp(
                _crosslineRect.sizeDelta,
                Vector2.one * (CrossLineDefaltDistance + amout * CrossLineMaxDistance),
                Time.deltaTime * CrossExpandSpeed
            );
            yield return null;
        }
    }

    // 锁定脉冲动画（扩散+淡出）
    IEnumerator LockPulseAnimation()
    {
        if (LockIndicator == null) yield break;

        float timer = 0;
        Color originalColor = LockIndicator.color;
        Vector3 originalScale = LockIndicator.transform.localScale;

        while (_currentState == CursorState.LockOn)
        {
            timer += Time.deltaTime * LockPulseSpeed;

            // 正弦波动画：让光圈有呼吸感
            float pulse = Mathf.Sin(timer * Mathf.PI * 2) * 0.3f + 1;

            // 如果是需要一次性扩散（如锁定瞬间的冲击波），可以用另一种算法
            float expand = Mathf.PingPong(timer, 1) * (LockMaxScale - 1) + 1;

            LockIndicator.transform.localScale = Vector3.one * expand;

            // 透明度变化
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(0.5f, 0, Mathf.PingPong(timer, 1));
            LockIndicator.color = newColor;

            yield return null;
        }

        // 恢复默认
        LockIndicator.gameObject.SetActive(false);
        LockIndicator.transform.localScale = originalScale;
        LockIndicator.color = originalColor;
    }

    // 让光标跟随某个目标（锁定框跟踪）
    IEnumerator FollowTarget(GameObject target)
    {
        if (target == null) yield break;

        Camera mainCam = Camera.main;

        while (_currentState == CursorState.LockOn && target != null)
        {
            // 将3D世界坐标转为屏幕坐标
            Vector3 screenPos = mainCam.WorldToScreenPoint(target.transform.position);

            // 让光标直接跳到目标位置（锁定效果）
            CursorTransform.position = Vector3.Lerp(CursorTransform.position, screenPos, Time.deltaTime * FollowSpeed);

            yield return null;
        }
    }

    void OnDestroy()
    {
        // 恢复系统光标显示
        Cursor.visible = true;
    }
}