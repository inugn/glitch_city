using System;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public sealed class CardChoiceStub : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;

        private Action onChosen;
        private PauseService pause;

        private void Awake()
        {
            pause = App.Instance.Services.Get<PauseService>();
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        public void Open(Action onCardChosen)
        {
            onChosen = onCardChosen;
            pause.Pause();

            if (panelRoot != null)
                panelRoot.SetActive(true);
        }
        
        public void ChooseAnyCard()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            pause.Resume();
            onChosen?.Invoke();
        }
    }
}