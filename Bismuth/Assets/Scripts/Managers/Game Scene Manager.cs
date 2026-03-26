using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // 게임 씬 이동
    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadNextStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CurrentSceneIndex() + 1);
    }

    // 씬 재시작
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CurrentSceneIndex());
    }

    public int CurrentSceneIndex()
    {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        return SceneManager.GetActiveScene().buildIndex;
    }
    
    // 타이틀 씬 이동
    public void LoadTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene((int)SceneIndex.TitleScene);
    }

    public void GameQuit()
        => Application.Quit();
}
public enum SceneIndex
{
    TitleScene,
}