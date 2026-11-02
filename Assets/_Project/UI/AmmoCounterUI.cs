using _Project.Scripts.Weapon;
using TMPro;
using UnityEngine;

namespace _Project.UI
{
    public sealed class AmmoCounterUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text ammoText;

        private RangedWeapon weapon;

        public void Bind(RangedWeapon newWeapon)
        {
            Unbind();

            weapon = newWeapon;

            if (weapon == null)
            {
                ammoText.text = "--";
                return;
            }

            weapon.OnAmmoChanged += UpdateAmmo;
            UpdateAmmo(weapon.CurrentAmmo);
        }

        private void Unbind()
        {
            if (weapon != null)
                weapon.OnAmmoChanged -= UpdateAmmo;

            weapon = null;
        }

        private void UpdateAmmo(int current)
        {
            ammoText.text = $"{current}/";
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}