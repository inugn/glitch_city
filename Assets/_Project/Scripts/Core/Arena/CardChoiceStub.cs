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

        [Header("Pause (stub)")]
        [SerializeField] private bool pauseWithTimeScale = true;

        private bool isOpen;

        private float prevTimeScale = 1f;
        private bool pausedHere;

        private Action onClosed;
        private Action<CardData> onCardChosen;

        private List<CardData> currentCards;

        private void Awake()
        {
            if (panelRoot == null)
            {
                var found = GameObject.Find("CardChoicePanel");
                if (found != null) panelRoot = found;
            }

            if (controller == null && panelRoot != null)
                controller = panelRoot.GetComponentInChildren<CardChoiceController>(true);

            if (panelRoot != null) panelRoot.SetActive(false);
            else Debug.LogError("[CardChoiceStub] panelRoot is NULL. Assign CardChoicePanel.", this);

            if (controller == null)
                Debug.LogError("[CardChoiceStub] controller is NULL. Assign CardChoiceController.", this);
        }

        private void OnDisable()
        {
            // Важно: при выключении (смена сцены/выключение UI) НЕ стартуем волну “случайно”
            ForceCloseWithoutCallback();
        }

        // Legacy API: ArenaFlow может вызывать как раньше
        public void Open(Action onClosedCallback)
        {
            OpenInternal(null, onClosedCallback);
        }

        // Новый API: если кому-то нужна выбранная карта
        public void Open(Action<CardData> onCardChosenCallback)
        {
            OpenInternal(onCardChosenCallback, null);
        }

        private void OpenInternal(Action<CardData> onCardChosenCallback, Action onClosedCallback)
        {
            if (isOpen) return;

            if (panelRoot == null || controller == null)
            {
                Debug.LogError("[CardChoiceStub] Open() failed: panelRoot/controller not assigned.", this);
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

                ApplyCard(card);
                Close(card);
            });
        }

        private void ApplyCard(CardData card)
        {
            // Пока MVP: просто лог. Эффекты подключим позже.
            Debug.Log($"[CardChoice] Chosen: {card.Title}", this);

            if (card.PositiveEffect != null)
                Debug.Log($"[CardChoice] Positive: {card.PositiveEffect.GetDescription()}", this);

            if (card.NegativeEffect != null)
                Debug.Log($"[CardChoice] Negative: {card.NegativeEffect.GetDescription()}", this);
        }

        private void Close(CardData chosen)
        {
            if (!isOpen) return;

            isOpen = false;

            RestorePause();

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
                pausedHere = false; // кто-то другой уже поставил паузу
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
