using System;
using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    [Header("移动参数")]
    public float MoveForce = 10f;      // 移动力
    public float MaxSpeed = 5f;        // 最大速度
    public float Acceleration = 8f;    // 加速度
    public Transform Target;

    private void GetToPlayer() // TODO: 临时做的走向玩家
    {
        Vector3 moveVector = Target.position - transform.position;
        if(moveVector.magnitude > 2f)
        {
            Vector3 targetVelocity = moveVector.normalized * MaxSpeed;
            Vector3 velocityDiff = targetVelocity - _rigidbody.velocity;
            velocityDiff = Vector3.ClampMagnitude(velocityDiff, Acceleration * Time.fixedDeltaTime);
            _rigidbody.AddForce(velocityDiff * _rigidbody.mass, ForceMode.VelocityChange);
        }
        else
        {
            // 减速
            Vector3 currentHorizontalVelocity = new(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            Vector3 deceleration = -currentHorizontalVelocity.normalized * Acceleration;

            if (currentHorizontalVelocity.magnitude > 0.1f)
            {
                _rigidbody.AddForce(deceleration * _rigidbody.mass, ForceMode.VelocityChange);
            }
        }
    }
}