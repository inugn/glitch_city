using _Project.Scripts.Core.World;
using UnityEngine;

namespace _Project.Scripts.Core.Arena
{
    public sealed class ArenaFlow : MonoBehaviour
    {
        [SerializeField] private DoorTrigger rightDoor;
        [SerializeField] private CardChoiceStub cardChoice;

        private GameSession session;

        private void Awake()
        {
            // Ничего из App.Instance здесь НЕ трогаем.
            // Здесь только валидация ссылок из инспектора.
            if (rightDoor == null)
                Debug.LogError("[ArenaFlow] rightDoor is not assigned in Inspector.", this);

            if (cardChoice == null)
                Debug.LogError("[ArenaFlow] cardChoice is not assigned in Inspector.", this);
        }

        private void Start()
        {
            if (rightDoor == null || cardChoice == null)
            {
                enabled = false;
                return;
            }

            if (App.Instance == null)
            {
                Debug.LogError("[ArenaFlow] App.Instance is null. Ensure AppBootstrap (App) exists in this scene.", this);
                enabled = false;
                return;
            }

            if (App.Instance.Services == null)
            {
                Debug.LogError("[ArenaFlow] App.Services is null. App did not initialize services correctly.", this);
                enabled = false;
                return;
            }

            session = App.Instance.Services.Get<GameSession>();
            if (session == null)
            {
                Debug.LogError("[ArenaFlow] GameSession service not found. Ensure it is registered in App.Services.", this);
                enabled = false;
                return;
            }

            // На арене правая дверь закрыта, пока не пройдены 3 волны
            rightDoor.SetLocked(true);

            // Если хочешь: сбросить волну при входе на арену
            // session.SetWave(0);

            StartNextWave();
        }

        public void StartNextWave()
        {
            // Перед волной показываем “выбор карточки”
            cardChoice.Open(OnCardChosen);
        }

        private void OnCardChosen()
        {
            // Здесь позже будут применяться эффекты карточки.
            // Сейчас просто считаем, что волна началась.
        }

        // Заглушка: вызови эту функцию кнопкой в UI, чтобы “завершить волну”
        public void DebugCompleteWave()
        {
            if (session == null)
            {
                Debug.LogError("[ArenaFlow] session is null. ArenaFlow not initialized.", this);
                return;
            }

            var nextWave = session.WaveIndex + 1;

            if (nextWave <= 3)
            {
                session.SetWave(nextWave);
                StartNextWave();
                return;
            }

            // Волны пройдены: арена завершена -> открываем дверь
            rightDoor.SetLocked(false);
        }
    }
}
