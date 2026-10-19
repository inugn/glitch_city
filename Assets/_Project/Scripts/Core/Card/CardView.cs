using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Core.Card
{
    public sealed class CardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image rarityBackground;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text positiveText;
        [SerializeField] private TMP_Text negativeText;
        [SerializeField] private Button button;

        public void Setup(
            string rarity,
            Color rarityColor,
            string positiveDescription,
            string negativeDescription,
            System.Action onClick)
        {
            rarityText.text = rarity;
            rarityBackground.color = rarityColor;

            positiveText.text = positiveDescription;
            negativeText.text = negativeDescription;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}