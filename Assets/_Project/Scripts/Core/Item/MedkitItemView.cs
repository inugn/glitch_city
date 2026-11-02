using _Project.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Core.Item
{
    public class MedkitItemView : ItemView
    {
        [SerializeField] private TMP_Text price;
        [SerializeField] private TMP_Text heal;
        [SerializeField] private Image coinIcon;
        
        public void SetPrice(string value)
        {
            if (price != null)
                price.text = value;
        }

        public void SetHeal(string value)
        {
            if (heal != null)
                heal.text = value;
        }
        
        public override void SetDisabled(bool disabled)
        {
            base.SetDisabled(disabled);
            float alpha = disabled ? 0.4f : 1f;
            
            if (coinIcon != null)
                coinIcon.color = new Color(1f, 1f, 1f, alpha);
            
            if (price != null)
                price.alpha = alpha;
            
            if (heal != null)
                heal.alpha = alpha;
        }
    }
}