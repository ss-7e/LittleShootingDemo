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
        if(other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.OnHitByBullet(SimpleBulletData.Direction, SimpleBulletData.Damage);
        }
        Destroy(gameObject);
    }

}
