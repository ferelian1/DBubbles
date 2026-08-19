using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnLevelButtonClicked()
    {
        SceneChangeManager.Instance.LoadScene(SceneName.Level1);
    }

    public void OnEndlessButtonClicked()
    {
        SceneChangeManager.Instance.LoadScene(SceneName.EndlessScene);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
