using UnityEngine;
using TMPro;
using System.Collections;

public class LevelTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float fadeDuration = 2f;

    private void Awake()
    {
        if (levelText == null)
            levelText = GetComponent<TextMeshProUGUI>();

        SetLevelText();
    }

    private void SetLevelText()
    {
        string sceneName =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "level-1":
                levelText.text = "LEVEL 1";
                break;

            case "level-2":
                levelText.text = "LEVEL 2";
                break;

            case "level-3":
                levelText.text = "LEVEL 3";
                break;

            case "Endless":
                levelText.text = "ENDLESS";
                break;

            default:
                levelText.text = "";
                break;
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Color color = levelText.color;
        color.a = 1f;
        levelText.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha =
                Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            color.a = alpha;
            levelText.color = color;

            yield return null;
        }

        color.a = 0f;
        levelText.color = color;
    }
}