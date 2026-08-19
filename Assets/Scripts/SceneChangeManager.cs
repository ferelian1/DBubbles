using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
    {
        MainMenu,
        EndlessScene,
        Level1,
        Level2,
        Level3
    }

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }

    

    private const string ENDLESS_SCENE_NAME = "EndlessScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void LoadNextScene()
    {
        // Load the next scene in the build settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
    public void LoadScene(SceneName sceneName)
    {
        // Load the specified scene using the SceneManager
        SceneManager.LoadScene(sceneName.ToString());
    }

}
    