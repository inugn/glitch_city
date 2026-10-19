using System;
using System.Collections.Generic;
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

        private Action onChosen;
        private bool isOpen;
        
        private List<CardData> _currentCards;

        private void Awake()
        {
            // Попробуем найти панель автоматически, если не назначили в инспекторе
            if (panelRoot == null)
            {
                var found = GameObject.Find("CardChoicePanel");
                if (found != null) panelRoot = found;
            }

            if (panelRoot != null)
                panelRoot.SetActive(false);
            else
                Debug.LogError("[CardChoiceStub] panelRoot is NULL. Assign CardChoicePanel to panelRoot in Inspector (or name it exactly 'CardChoicePanel').", this);
        }

        private void OnDisable()
        {
            // На случай, если объект отключили во время паузы
            if (pauseWithTimeScale && Time.timeScale == 0f)
                Time.timeScale = 1f;

            isOpen = false;
            onChosen = null;
        }

        public void Open(Action<CardData> onCardChosen)
        {
            isOpen = true;

            _currentCards = CardGenerator.Generate(3);

            if (panelRoot == null)
            {
                Debug.LogError("[CardChoiceStub] Open() called but panelRoot is NULL.", this);
                return;
            }

            panelRoot.SetActive(true);

            if (pauseWithTimeScale)
                Time.timeScale = 0f;
            
            controller.Open(card =>
            {
                if (pauseWithTimeScale)
                    Time.timeScale = 1f;

                panelRoot.SetActive(false);

                onCardChosen?.Invoke(card);
            });
        }

        // Вызывается кнопками UI
        public void ChooseCard(int index)
        {
            if (!isOpen) return;
            if (index < 0 || index >= _currentCards.Count) return;

            var chosen = _currentCards[index];

            ApplyCard(chosen);

            Close();
        }

        private void ApplyCard(CardData card)
        {
            // TODO применение к игроку и волне
            Debug.Log($"Редкость: {card.Title}");

            Debug.Log("Эффект игрока:");
            Debug.Log(card.PositiveEffect.GetDescription());

            Debug.Log("Эффект волны:");
            Debug.Log(card.NegativeEffect.GetDescription());
        }

        private void Close()
        {
            isOpen = false;

            if (pauseWithTimeScale)
                Time.timeScale = 1f;

            panelRoot.SetActive(false);

            var cb = onChosen;
            onChosen = null;
            cb?.Invoke();
        }
    }
}
