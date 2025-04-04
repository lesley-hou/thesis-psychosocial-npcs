using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneResetter : MonoBehaviour
{
    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (fadeOverlay != null)
            StartCoroutine(FadeFromBlack());
    }

    public void RestartScene()
    {
        StartCoroutine(FadeAndRestart());
    }

    private IEnumerator FadeAndRestart()
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeToBlack()
    {
        float time = 0f;
        Color original = fadeOverlay.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeOverlay.color = new Color(original.r, original.g, original.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadeOverlay.color = new Color(original.r, original.g, original.b, 1f);
    }

    private IEnumerator FadeFromBlack()
    {
        float time = 0f;
        Color original = fadeOverlay.color;
        fadeOverlay.color = new Color(original.r, original.g, original.b, 1f);

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadeOverlay.color = new Color(original.r, original.g, original.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadeOverlay.color = new Color(original.r, original.g, original.b, 0f);
    }
}