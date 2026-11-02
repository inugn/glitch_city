using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Card;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public sealed class CardChoiceStub : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private CardChoiceController controller;

        [Header("Pause")]
        [SerializeField] private bool pauseWithTimeScale = true;

        private bool isOpen;

        private float prevTimeScale = 1f;
        private bool pausedHere;

        private Action onClosed;
        private Action<CardData> onCardChosen;

        private List<CardData> currentCards;

        private void Awake()
        {
            AutoWire();

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void AutoWire()
        {
            // 1) Если controller не назначен — ищем в сцене, включая НЕактивные
            if (controller == null)
            {
#if UNITY_2023_1_OR_NEWER
                controller = FindFirstObjectByType<CardChoiceController>(FindObjectsInactive.Include);
#else
                controller = FindObjectOfType<CardChoiceController>(true);
#endif
            }

            // 2) Если panelRoot не назначен — берём объект контроллера (или можно подняться к root)
            if (panelRoot == null && controller != null)
                panelRoot = controller.gameObject; // или controller.transform.root.gameObject;

            // 3) Логи — чтобы сразу видеть, почему не открывается
            if (panelRoot == null)
                Debug.LogError("[CardChoiceStub] panelRoot is NULL. Assign CardChoicePanel OR ensure there is CardChoiceController in scene.", this);

            if (controller == null)
                Debug.LogError("[CardChoiceStub] controller is NULL. Assign CardChoiceController OR ensure it exists in scene.", this);
        }

        private void OnDisable()
        {
            // важно: не триггерим коллбеки при выключении/смене сцены
            ForceCloseWithoutCallback();
        }

        public void Open(Action onClosedCallback)
        {
            OpenInternal(null, onClosedCallback);
        }

        public void Open(Action<CardData> onCardChosenCallback)
        {
            OpenInternal(onCardChosenCallback, null);
        }

        private void OpenInternal(Action<CardData> onCardChosenCallback, Action onClosedCallback)
        {
            if (isOpen) return;

            // супер важно: после смен сцен ссылки могли отвалиться
            AutoWire();

            if (panelRoot == null || controller == null)
            {
                Debug.LogError("[CardChoiceStub] Open() failed: panelRoot/controller not assigned or not found.", this);
                return;
            }

            isOpen = true;
            onCardChosen = onCardChosenCallback;
            onClosed = onClosedCallback;

            currentCards = CardGenerator.Generate(3);
            if (currentCards == null || currentCards.Count == 0)
            {
                Debug.LogError("[CardChoiceStub] CardGenerator.Generate returned no cards.", this);
                ForceCloseWithoutCallback();
                return;
            }

            panelRoot.SetActive(true);
            ApplyPause();

            controller.Open(currentCards, card =>
            {
                if (!isOpen) return;

                ApplyCardSafe(card);
                Close(card);
            });
        }

        private void ApplyCardSafe(CardData card)
        {
            // ВАЖНО: у тебя тут может быть Null, если EffectApplier НЕ зарегистрирован как сервис.
            // Поэтому делаем безопасно.
            var services = App.Instance?.Services;
            if (services == null)
            {
                Debug.LogWarning("[CardChoiceStub] App/Services not ready. Card effect was NOT applied.", this);
                return;
            }

            var applier = services.Get<EffectApplier>();
            if (applier == null)
            {
                Debug.LogWarning("[CardChoiceStub] EffectApplier not found in Services. Card effect was NOT applied.", this);
                return;
            }

            applier.ApplyCard(card); // оставляю как у тебя, но если метода нет — скажи, поправим под твою версию
        }

        private void Close(CardData chosen)
        {
            if (!isOpen) return;

            isOpen = false;
            RestorePause();

            if (controller != null)
                controller.Close();

            if (panelRoot != null)
                panelRoot.SetActive(false);

            var chosenCb = onCardChosen;
            var closedCb = onClosed;

            onCardChosen = null;
            onClosed = null;
            currentCards = null;

            chosenCb?.Invoke(chosen);
            closedCb?.Invoke();
        }

        private void ForceCloseWithoutCallback()
        {
            if (!isOpen) return;

            isOpen = false;
            RestorePause();

            if (controller != null)
                controller.Close();

            if (panelRoot != null)
                panelRoot.SetActive(false);

            onCardChosen = null;
            onClosed = null;
            currentCards = null;
        }

        private void ApplyPause()
        {
            if (!pauseWithTimeScale) return;

            if (Time.timeScale <= 0f)
            {
                pausedHere = false;
                return;
            }

            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            pausedHere = true;
        }

        private void RestorePause()
        {
            if (!pauseWithTimeScale) return;
            if (!pausedHere) return;

            Time.timeScale = prevTimeScale <= 0f ? 1f : prevTimeScale;
            pausedHere = false;
        }
    }
}
