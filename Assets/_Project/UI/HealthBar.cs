using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text hpText;
        
        public void UpdateHealth(int current, int max)
        {
            if (max <= 0) return;

            slider.value = (float)current / max;
            hpText.text = $"{current} / {max}";
        }
    }
}