using UnityEngine;

public class ExposiveItem : MonoBehaviour, IHitable
{
    [Header("爆炸物数值")]
    public float ExplosionRadius = 10f;
    public float ExplosionForce = 50f;
    public float UpwardsModifier = 1f; // 向上的力修正
    public float Damage = 30f; // 爆炸伤害
    public float Health = 50f; // 爆炸物的生命值
    public float ExplosiveDuration = 0.5f; // 爆炸屏幕震动持续时间

    public GameObject ExposionEffectPrefab; // 爆炸特效预制体
    

    private float _currentHealth;
    private void Start()
    {
        _currentHealth = Health;
    }
    public void OnHit(Vector3 hitDirection, float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            Explode();
            if(ExposionEffectPrefab != null)
            {
                Destroy(Instantiate(ExposionEffectPrefab, transform.position, Quaternion.identity), ExplosiveDuration);
            }
        }
    }

    private void Explode()
    {
        EventManager.Instance.TriggerExplosion(transform.position, ExplosionForce / 15f, ExplosiveDuration);
        // 获取爆炸中心
        Vector3 explosionPosition = transform.position;
        // 获取所有在爆炸范围内的碰撞体
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, ExplosionRadius);
        foreach (Collider hit in colliders)
        {
            if(hit.gameObject == gameObject) continue; // 跳过自己
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 应用爆炸力
                rb.AddExplosionForce(ExplosionForce, explosionPosition, ExplosionRadius, UpwardsModifier, ForceMode.Impulse);
            }
            var hitables = hit.GetComponents<IHitable>();
            foreach (var target in hitables)
            {
                // 计算伤害衰减（距离越远伤害越小）
                float distance = Vector3.Distance(explosionPosition, hit.transform.position);
                float damageMultiplier = Mathf.Clamp01(1 - (distance / ExplosionRadius));
                float finalDamage = Damage * damageMultiplier;
                target.OnHit((hit.transform.position - explosionPosition).normalized, finalDamage);
            }

        }
        // 销毁炸弹对象
        Destroy(gameObject, ExplosiveDuration / 2);
    }
}