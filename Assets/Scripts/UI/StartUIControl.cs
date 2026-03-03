using UnityEngine;
public class StartUIControl : MonoBehaviour
{
    public PauseMenuControl PauseMenuControl;

    private void Start()
    {
        Time.timeScale = 0f;
        PauseMenuControl.enabled = false;
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        PauseMenuControl.enabled = true;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif 

        Application.Quit();
    }
}