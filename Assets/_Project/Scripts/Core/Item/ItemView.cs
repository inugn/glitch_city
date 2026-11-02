using TMPro;
using UnityEngine;

namespace _Project.Scripts.Items
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private TMP_Text nameText;

        public void SetName(string value)
        {
            if (nameText != null)
                nameText.text = value;
        }

        public void SetIcon(Sprite sprite)
        {
            if (icon != null)
                icon.sprite = sprite;
        }
        
        public virtual void SetDisabled(bool disabled)
        {
            float alpha = disabled ? 0.4f : 1f;

            if (icon != null)
                icon.color = new Color(1f, 1f, 1f, alpha);

            if (nameText != null)
                nameText.alpha = alpha;
        }
    }
}