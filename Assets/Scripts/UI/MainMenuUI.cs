using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform levelPanel;
    [SerializeField] private float slideDistance = 500f;

    private bool startButtonClicked = false;
    private float initialPositionX;
    public void OnPlayButtonClicked()
    {
        levelPanel.DOKill();

        if (!startButtonClicked)
        {
            levelPanel.gameObject.SetActive(true);

            levelPanel
            .DOAnchorPosX(initialPositionX + slideDistance, 1f)
            .SetEase(Ease.OutBack);
            startButtonClicked = true;
        }
        else
        {
            levelPanel
            .DOAnchorPosX(initialPositionX - slideDistance, 1f)
            .SetEase(Ease.OutBack).OnComplete(() =>
            {
                levelPanel.gameObject.SetActive(false);
                startButtonClicked = false;
            });
        }
    }
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
