using UnityEngine;


public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] EnemyPrefab;    // 目前只有两种敌人,直接配置
    public float SpawnInterval = 5f;    // 生成间隔时间
    public int MaxEnemies = 10;         // 最大敌人数量
    public Transform PlayerTransform;   // 玩家位置,敌人生成后会朝玩家走去
    [Range(0, 1)]
    public float SpawnChance = 0.5f;    // 爆炸敌人生成概率

    [Header("敌人死亡设定")]
    public bool DeleteEnemyOnDeath = true; // 是否在敌人死亡后删除对象
    public bool KeepEnemyPhysicsOnDeath = true; // 是否在敌人死亡后保留物理效果

    public float InnerRadius;
    public float OuterRadius;

    private float _spawnTimer = 0f;
    private int _currentEnemyCount = 0;
    private void Update()
    {
        if (_currentEnemyCount >= MaxEnemies)
            return;
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= SpawnInterval)
        {
            SpawnEnemyUnit();
            _spawnTimer = 0f;
        }
    }


    private void SpawnEnemyUnit()
    {
        GameObject parent = GameObject.Find("Enemys");
        if(parent == null)
        {
            parent = new GameObject("Enemys");
        }
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // 面积均匀分布：使用平方根调整
        float radius = Mathf.Sqrt(Random.Range(
            InnerRadius * InnerRadius,
            OuterRadius * OuterRadius
        ));

        Vector3 position = new Vector3(
            Mathf.Cos(angle) * radius,
            0.1f,
            Mathf.Sin(angle) * radius
        );
        GameObject e;
        if (Random.Range(0f, 1f) > 0.5f)
            e = Instantiate(EnemyPrefab[0], position + PlayerTransform.position, Quaternion.identity, parent.transform);
        else
            e = Instantiate(EnemyPrefab[1], position + PlayerTransform.position, Quaternion.identity, parent.transform);
        Enemy enemy = e.GetComponent<Enemy>();
        enemy.Target = PlayerTransform;
        enemy.OnDeath += OnEnemyDestroyed;
        enemy.DeleteGameObjectOnDeath = DeleteEnemyOnDeath;
        enemy.KeepPhysicsOnDeath = KeepEnemyPhysicsOnDeath;
        _currentEnemyCount++;
    }

    private void OnEnemyDestroyed()
    {
        _currentEnemyCount--;
    }
}