using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public sealed class BossHealthView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Slider healthSlider;

        public void Show()
        {
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetMaxHealth(int max)
        {
            healthSlider.maxValue = max;
            healthSlider.value = max;
        }

        public void SetHealth(int current)
        {
            healthSlider.value = current;
        }
    }

}