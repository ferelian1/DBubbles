using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
public void LoadNextLevel()
    {
        SceneChangeManager.Instance.LoadNextScene();
    }
}