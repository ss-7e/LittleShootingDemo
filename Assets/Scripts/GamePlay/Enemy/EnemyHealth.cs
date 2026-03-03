using UnityEngine;
using UnityEngine.UIElements;

public partial class Enemy : MonoBehaviour
{
    [Header("敌人生命值相关")]
    public float MaxHealth = 100;
    public GameObject HealthbarUI;
    public Material HealthBarMaterial;
    public float HealthBarHideDelay;
    public float HealthBarDecreaseSpeed;

    private float _currentHealth;
    private float _currentHealthDelay;
    private float _healthBarTimer;
    private Material _healthBarMaterial;
    private Material _healthBarDelayMaterial;

    private void InitHealth()
    {
        _currentHealth = MaxHealth;
        _currentHealthDelay = MaxHealth;
        _healthBarMaterial = new Material(HealthBarMaterial);
        HealthbarUI.transform.Find("HealthBar").GetComponent<UnityEngine.UI.Image>().material = _healthBarMaterial;
        _healthBarDelayMaterial = new Material(HealthBarMaterial);
        HealthbarUI.transform.Find("HealthBarDelay").GetComponent<UnityEngine.UI.Image>().material = _healthBarDelayMaterial;
    }

    private void TakeDamage(float damage)
    {
        _healthBarTimer = HealthBarHideDelay;
        _currentHealth -= damage;
        // 如果生命值小于等于0，触发死亡事件
        if (_currentHealth <= 0)
        {
            HealthbarUI.SetActive(false);
            Die();
            return;
        }
        HealthbarUI.SetActive(true);
        _healthBarMaterial.SetFloat("_FillAmount", _currentHealth / MaxHealth);

    }

    private void HealthBarUpdate()
    {
        if (_healthBarTimer > 0) 
        {
            HealthbarUI.transform.LookAt(Camera.main.transform);
            _healthBarTimer -= Time.deltaTime;
            _currentHealthDelay = Mathf.Lerp(_currentHealthDelay, _currentHealth, HealthBarDecreaseSpeed * Time.deltaTime);
            _healthBarDelayMaterial.SetFloat("_FillAmount", _currentHealthDelay / MaxHealth);
            if (_healthBarTimer <= 0) HealthbarUI.SetActive(false);
        }
    }
}