using UnityEngine;

public interface IHitable
{
    void OnHit(Vector3 hitDirection, float damage);
}