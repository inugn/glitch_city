using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    [CreateAssetMenu(menuName = "_Project/Items/Medkit", fileName = "MedkitDef_")]
    public sealed class MedkitDefinition : ScriptableObject
    {
        public string medkitId = "Аптечка";

        [Header("Economy")]
        public int price;

        [Header("Healing")]
        public int healAmount;

        [Header("UI")]
        public Sprite icon;
    }
}