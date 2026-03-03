using System;
using UnityEngine;

public partial class Enemy : MonoBehaviour, IHitable
{
    public event Action OnDeath;

    [Header("击打材质效果设置")]
    public float HitEffectDuration = 0.2f;       // 总效果持续时间
    public float DarkPhaseDuration = 0.05f;      // 变黑阶段持续时间
    public Color DarkColor = Color.black;        // 变黑时的颜色
    public Color BrightColor = Color.white;      // 变亮时的颜色
    public float BrightEmissionIntensity = 3f;   // 变亮时的自发光强度

    [Header("击退效果")]
    public Vector3 HitIntoAir = new(0, 1f, 0);  // 击退时的向上力
    public float HitForce = 0.05f;              // 击退的力度

    [Header("死亡效果")]
    public float DieHitForce = 0.5f;            // 死亡时的击退力度
    public Vector3 DieHitVector = Vector3.zero;
    public bool DeleteGameObjectOnDeath = true; // 是否在死亡后删除敌人对象
    public bool KeepPhysicsOnDeath = true;      // 保留尸体碰撞

    private Material _material;
    private Color _originalBaseColor;
    private Color _originalEmissionColor;
    private float _hitEffectTimer = 0;
    private bool _isHitting = false;
    private Rigidbody _rigidbody;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _originalBaseColor = _material.GetColor("_BaseColor");
        _originalEmissionColor = _material.GetColor("_EmissionColor");
        _material.EnableKeyword("_EMISSION");
        
        _rigidbody = GetComponent<Rigidbody>();
        InitHealth();
        
    }

    public void OnHit(Vector3 hitDirection, float damage = 10)
    {

        // 触发击打效果

        _hitEffectTimer = HitEffectDuration;

        _isHitting = true;
        hitDirection += HitIntoAir;
        HybridUprightSystem hybridUprightSystem = GetComponent<HybridUprightSystem>();
        hybridUprightSystem?.ApplyImpact(hitDirection, transform.position);


        // 造成伤害

        TakeDamage(damage);
    }


    void Update()
    {
        GetToPlayer();
        HealthBarUpdate();

        // 处理击打效果计时
        if (_hitEffectTimer > 0)
        {
            _hitEffectTimer -= Time.deltaTime;

            // 应用击打效果
            ApplyHitEffect();
        }
        else if (_isHitting)
        {
            // 效果结束，恢复原始颜色
            _isHitting = false;
            ResetMaterialToOriginal();
        }
    }

    private void ApplyHitEffect()
    {
        if (_material == null) return;

        // 计算当前时间在整个效果周期中的位置（0到1）
        float normalizedTime = 1f - (_hitEffectTimer / HitEffectDuration);

        Color currentColor;
        Color currentEmission;

        // 判断当前处于哪个阶段
        if (normalizedTime < DarkPhaseDuration / HitEffectDuration)
        {
            // 第一阶段：变黑阶段
            float phaseProgress = normalizedTime / (DarkPhaseDuration / HitEffectDuration);

            // 从原始颜色渐变到黑色
            currentColor = Color.Lerp(_originalBaseColor, DarkColor, phaseProgress);

            // 自发光逐渐减弱到0
            currentEmission = Color.Lerp(_originalEmissionColor, Color.black, phaseProgress);
        }
        else
        {
            // 第二阶段：变亮阶段
            float phaseProgress = (normalizedTime - DarkPhaseDuration / HitEffectDuration) /
                                 (1f - DarkPhaseDuration / HitEffectDuration);

            // 从黑色渐变到亮色，然后渐变回原始颜色
            if (phaseProgress < 0.3f)  // 前30%的亮阶段快速变亮
            {
                float brightProgress = phaseProgress / 0.3f;
                currentColor = Color.Lerp(DarkColor, BrightColor, brightProgress);
                currentEmission = Color.Lerp(Color.black, BrightColor * BrightEmissionIntensity, brightProgress);
            }
            else  // 后70%的亮阶段逐渐恢复原始颜色
            {
                float recoverProgress = (phaseProgress - 0.3f) / 0.7f;
                currentColor = Color.Lerp(BrightColor, _originalBaseColor, recoverProgress);
                currentEmission = Color.Lerp(BrightColor * BrightEmissionIntensity, _originalEmissionColor, recoverProgress);
            }
        }

        // 应用颜色
        _material.SetColor("_BaseColor", currentColor);

        // 应用自发光
        if (_material.HasProperty("_EmissionColor"))
        {
            _material.SetColor("_EmissionColor", currentEmission);
        }
    }

    private void ResetMaterialToOriginal()
    {
        if (_material == null) return;

        // 恢复原始颜色
        _material.SetColor("_BaseColor", _originalBaseColor);

        // 恢复原始自发光
        if (_material.HasProperty("_EmissionColor"))
        {
            _material.SetColor("_EmissionColor", _originalEmissionColor);
        }
    }

    private void Die()
    {
        ResetMaterialToOriginal();
        OnDeath?.Invoke();
        FallDown();

        if (DeleteGameObjectOnDeath)
        {
            Destroy(GetComponent<Collider>(), 2f);
            Destroy(gameObject, 5f); // 5秒后删除对象，给击退和倒地效果时间展示
        }
        else if (!KeepPhysicsOnDeath)
        {
            Destroy(_rigidbody, 2f);
            Destroy(GetComponent<Collider>(), 2f);
        }
    }
    private void FallDown()
    {
        _rigidbody.isKinematic = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero; // 重置速度，确保击退效果一致
        _rigidbody.drag = 0;

        Destroy(this);
        Destroy(GetComponent<HybridUprightSystem>());   // 禁用直立系统
        Destroy(GetComponent<ConfigurableJoint>());     // 销毁关节组件，允许敌人倒地
        _rigidbody.AddForceAtPosition((transform.position - Target.position ).normalized * DieHitForce + DieHitVector, transform.position + Vector3.up, ForceMode.Impulse); // 添加击退力
    }
}