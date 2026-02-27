using System.Collections;
using UnityEngine;

public partial class Player
{
    /// <summary>
    /// 设置玩家朝向，使其面向鼠标点击的位置
    /// TODO:
    /// </summary>

    private class PlayerAim
    {
        MouseAimManager _mouseInputManager = InputManager.MouseInput;
        public void SetAimRotation(Player player)
        {
            Transform playerTransform = player.transform;
            Vector3 targetDirection = playerTransform.position - _mouseInputManager.MouseWorldPosition;
            targetDirection.y = 0; // 保持水平旋转
            if (targetDirection.sqrMagnitude > 0.001f) // 避免零向量
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}