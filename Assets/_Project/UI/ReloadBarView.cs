using _Project.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public sealed class ReloadBarView : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private RangedWeapon weapon;

        public void Bind(RangedWeapon rangedWeapon)
        {
            if (weapon != null)
                Unbind();

            weapon = rangedWeapon;

            weapon.OnReloadStarted += OnReloadStarted;
            weapon.OnReloadFinished += OnReloadFinished;

            slider.value = 1f;
            slider.gameObject.SetActive(false);
        }

        private void Unbind()
        {
            weapon.OnReloadStarted -= OnReloadStarted;
            weapon.OnReloadFinished -= OnReloadFinished;
        }

        private void Update()
        {
            if (weapon == null || !weapon.IsReloading)
                return;

            slider.value = weapon.ReloadProgress;
        }

        private void OnReloadStarted()
        {
            slider.gameObject.SetActive(true);
            slider.value = 0f;
        }

        private void OnReloadFinished()
        {
            slider.value = 1f;
            slider.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (weapon != null)
                Unbind();
        }
    }
}