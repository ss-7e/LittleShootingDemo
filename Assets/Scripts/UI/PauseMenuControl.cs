using UnityEditor;
using UnityEngine;

public class PauseMenuControl : MonoBehaviour
{
    public GameObject PauseMenuUI; // 暂停菜单UI对象
    private bool isPaused = false;
    void Update()
    {
        // 按下Esc键切换暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void ResumeGame()
    {
        InputManager.Instance.enabled = true;   // 禁用输入管理器
        Cursor.visible = false;                 // 隐藏鼠标指针
        PauseMenuUI.SetActive(false);           // 隐藏暂停菜单UI
        Time.timeScale = 1f;                    // 恢复游戏时间
        isPaused = false;
    }
    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);  // 恢复默认鼠标指针
        InputManager.Instance.enabled = false;                  // 禁用输入管理器
        PauseMenuUI.SetActive(true);                            // 显示暂停菜单UI
        Time.timeScale = 0f;                                    // 暂停游戏时间
        isPaused = true;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 在编辑器中停止播放模式
        EditorApplication.isPlaying = false;
#endif
        // 在构建的游戏中退出应用
        Application.Quit();

    }
}   