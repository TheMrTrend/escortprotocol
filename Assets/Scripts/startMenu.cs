using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "Level 2";
    [SerializeField] private Image fadeImage;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        PlayClick();
        if (bgmSource != null)
            StartCoroutine(FadeOutMusicAndLoad());
        else
            StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeOutMusicAndLoad()
    {
        float startVolume = bgmSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (bgmSource != null)
                bgmSource.volume = Mathf.Lerp(startVolume, 0.05f, t / fadeDuration);
            yield return null;
        }

        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadeImage != null)
        {
            fadeImage.CrossFadeAlpha(1f, 1f, false);
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene(sceneName);
    }

    private void PlayClick()
    {
        if (clickSound != null)
            clickSound.Play();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
