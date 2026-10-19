using UnityEngine;

namespace _Project.Scripts.Core.Input
{
    public sealed class PauseInputBinder : MonoBehaviour
    {
        [SerializeField] private CoreInputReader input;

        private PauseService pauseService;
        private bool isReady;

        private void Start()
        {
            // Start гарантированно позже всех Awake(), значит App уже должен быть поднят
            if (App.Instance == null)
            {
                Debug.LogError("App.Instance is null. Ensure AppBootstrap with App exists in the scene.");
                enabled = false;
                return;
            }

            pauseService = App.Instance.Services.Get<PauseService>();
            isReady = true;
        }

        private void OnEnable()
        {
            if (input != null)
                input.PausePressed += OnPausePressed;
        }

        private void OnDisable()
        {
            if (input != null)
                input.PausePressed -= OnPausePressed;
        }

        private void OnPausePressed()
        {
            if (!isReady || pauseService == null) return;
            pauseService.Toggle();
        }
    }
}