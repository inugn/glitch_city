using TMPro;
using UnityEngine;

namespace _Project.UI
{
    public class StatRowUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text statNameText;
        [SerializeField] private TMP_Text baseValueText;
        [SerializeField] private TMP_Text multiplierText;
        
        public void SetData(string name, float baseValue, float multiplier)
        {
            statNameText.text = name;
            baseValueText.text = baseValue.ToString("0.##");

            if (Mathf.Approximately(multiplier, 1f))
            {
                multiplierText.gameObject.SetActive(false);
            }
            else
            {
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = $"x{multiplier:0.##}";
            }
        }
    }
}