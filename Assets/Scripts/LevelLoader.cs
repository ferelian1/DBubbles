using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string nextLevelScene;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelScene);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("level-1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("level-2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("level-3");
    }
}