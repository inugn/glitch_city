using _Project.Scripts.Core;
using _Project.Scripts.Core.Input;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerSpriteController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform weaponPivot;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateFlip();
        }

        private void UpdateFlip()
        {
            if (mainCamera == null || spriteRenderer == null || weaponPivot == null)
                return;

            Vector2 mouseWorldPos = App.Instance.Services.Get<ICoreInputReader>().MouseWorldPosition;

            bool flip = mouseWorldPos.x < weaponPivot.position.x;
            spriteRenderer.flipX = flip;
        }
    }
}