using UnityEngine;

[System.Serializable]
public struct BulletData
{
    public float Speed;
    public float Damage;
    public float Lifetime;
    public Vector3 Direction;
}
public class SimpleBullet : MonoBehaviour
{

    public BulletData SimpleBulletData;
    private void Start()
    {
        transform.LookAt(SimpleBulletData.Direction);
    }

    private void Update()
    {

        transform.position += SimpleBulletData.Speed * Time.deltaTime * SimpleBulletData.Direction.normalized;
        
        SimpleBulletData.Lifetime -= Time.deltaTime;
        if( SimpleBulletData.Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IHitable>(out var hit))
        {
            var hits = other.GetComponents<IHitable>();
            Debug.Log($"Bullet hit {other.gameObject.name} with {hits.Length} hitables");
            foreach (var h in hits)
                h.OnHit(SimpleBulletData.Direction, SimpleBulletData.Damage);
            EventManager.Instance.TriggerBulletHit();
            Destroy(gameObject);
        }
    }

}
