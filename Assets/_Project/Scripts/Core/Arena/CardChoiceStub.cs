using System;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public sealed class CardChoiceStub : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panelRoot; // сюда надо назначить объект Panel "CardChoicePanel"

        [Header("Pause (stub)")]
        [SerializeField] private bool pauseWithTimeScale = true;

        private Action onChosen;
        private bool isOpen;

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

        public void Open(Action onCardChosen)
        {
            onChosen = onCardChosen;
            isOpen = true;

            if (panelRoot == null)
            {
                Debug.LogError("[CardChoiceStub] Open() called but panelRoot is NULL.", this);
                return;
            }

            panelRoot.SetActive(true);

            if (pauseWithTimeScale)
                Time.timeScale = 0f;
        }

        // Вызывается кнопками UI
        public void ChooseAnyCard()
        {
            if (!isOpen) return; // защита от повторных кликов/вызовов

            isOpen = false;

            if (pauseWithTimeScale)
                Time.timeScale = 1f;

            if (panelRoot != null)
                panelRoot.SetActive(false);

            var cb = onChosen;
            onChosen = null;
            cb?.Invoke();
        }
    }
}
