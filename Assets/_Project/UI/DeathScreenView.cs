using System.Collections;
using UnityEngine;

namespace _Project.UI
{
    public class DeathScreenView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        private Coroutine fadeRoutine;

        public void Show()
        {
            gameObject.SetActive(true);
            StartFade(0f, 1f);
        }

        public void Hide()
        {
            StartFade(1f, 0f, () => gameObject.SetActive(false));
        }

        private void StartFade(float from, float to, System.Action onComplete = null)
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeRoutine(from, to, onComplete));
        }

        private IEnumerator FadeRoutine(float from, float to, System.Action onComplete)
        {
            canvasGroup.alpha = from;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, time / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = to;
            onComplete?.Invoke();
        }
    }
}