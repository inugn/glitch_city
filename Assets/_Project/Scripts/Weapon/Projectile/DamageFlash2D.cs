using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    [DisallowMultipleComponent]
    public sealed class DamageFlash2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] renderers;

        [Header("Flash")]
        [SerializeField] private float duration = 0.10f;
        [SerializeField] private Color flashColor = new Color(1f, 0.35f, 0.35f, 1f);

        private Color[] originalColors;
        private float until;
        private bool flashing;

        private void Awake()
        {
            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<SpriteRenderer>(true);

            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                originalColors[i] = renderers[i] != null ? renderers[i].color : Color.white;
        }

        public void Flash()
        {
            until = Time.time + duration;

            if (!flashing)
            {
                flashing = true;
                Apply(flashColor);
            }
            else
            {
                // уже мигаем — просто продлеваем
                Apply(flashColor);
            }
        }

        private void Update()
        {
            if (!flashing) return;

            if (Time.time >= until)
            {
                flashing = false;
                Restore();
            }
        }

        private void Apply(Color c)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].color = c;
            }
        }

        private void Restore()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].color = originalColors[i];
            }
        }
    }
}