using System.Collections;
using TMPro;
using UnityEngine;
using _Project.Scripts.Core;

namespace _Project.UI
{
    public sealed class CreditsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text creditsText;

        private EventBus bus;
        private GameSession session;
        private bool isBound;
        private Coroutine bindRoutine;

        private void Awake()
        {
            if (creditsText == null)
                creditsText = GetComponentInChildren<TMP_Text>(true);
        }

        private void OnEnable()
        {
            bindRoutine = StartCoroutine(BindRoutine());
        }

        private void OnDisable()
        {
            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
                bindRoutine = null;
            }
            Unbind();
        }

        private IEnumerator BindRoutine()
        {
            // Ретраим, пока не получится (или объект не выключат)
            while (!isBound)
            {
                if (creditsText != null && App.Instance?.Services != null)
                {
                    bus = App.Instance.Services.Get<EventBus>();
                    session = App.Instance.Services.Get<GameSession>();

                    if (bus != null && session != null)
                    {
                        bus.CreditsChanged += OnCreditsChanged;
                        isBound = true;

                        Debug.Log($"[CreditsUI] Bound OK busId={bus.GetHashCode()} uiId={GetHashCode()}");

                        OnCreditsChanged(session.Credits);
                        yield break;
                    }
                }

                yield return null; // пробуем снова на следующем кадре
            }
        }

        private void Unbind()
        {
            if (!isBound) return;

            if (bus != null)
                bus.CreditsChanged -= OnCreditsChanged;

            isBound = false;
            bus = null;
            session = null;
        }

        private void OnCreditsChanged(int value)
        {
            Debug.Log($"[CreditsUI] OnCreditsChanged({value}) busId={(bus != null ? bus.GetHashCode() : 0)} uiId={GetHashCode()}");

            if (creditsText != null)
                creditsText.text = $"Кредитов: {value}";
        }
    }
}
