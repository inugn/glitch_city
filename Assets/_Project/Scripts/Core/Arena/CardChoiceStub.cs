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
            // НЕ чистим onChosen тут.
            // CardChoicePanel может выключаться/включаться как UI.
            if (pauseWithTimeScale && Time.timeScale == 0f)
                Time.timeScale = 1f;

            isOpen = false;
        }


        public void Open(Action onCardChosen)
        {
            if (onCardChosen == null)
            {
                Debug.LogError("[CardChoiceStub] Open() called with NULL callback. Fix ArenaFlow: cardChoice.Open(OnCardChosen).", this);
                return;
            }
            
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
        
        public void ChooseAnyCard()
        {
            if (!isOpen) return;

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
