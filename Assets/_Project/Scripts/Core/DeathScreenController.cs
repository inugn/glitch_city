using System.Collections;
using _Project.UI;
using UnityEngine;

namespace _Project.Scripts.Core
{
    public class DeathScreenController : MonoBehaviour
    {
        [SerializeField] private DeathScreenView view;
        [SerializeField] private float returnDelay = 3f;

        private EventBus bus;
        private SceneLoader sceneLoader;

        private void Start()
        {
            var app = App.Instance;
            if (app == null)
            {
                Debug.LogError("[DeathScreenController] App.Instance is null");
                enabled = false;
                return;
            }

            bus = app.Services.Get<EventBus>();
            sceneLoader = app.Services.Get<SceneLoader>();

            if (bus == null)
            {
                Debug.LogError("[DeathScreenController] EventBus not found");
                enabled = false;
                return;
            }

            bus.PlayerDead += OnPlayerDead;
        }

        private void OnDestroy()
        {
            if (bus != null)
                bus.PlayerDead -= OnPlayerDead;
        }

        private void OnPlayerDead()
        {
            view.Show();
            StartCoroutine(ReturnToStartZone());
        }

        private IEnumerator ReturnToStartZone()
        {
            yield return new WaitForSeconds(returnDelay);

            view.Hide();
            sceneLoader.Load(SceneNames.StartZone);
        }
    }
}