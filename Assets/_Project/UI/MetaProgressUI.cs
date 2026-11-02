using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Project.Scripts.Core;

namespace _Project.UI
{
    public sealed class MetaProgressUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text xpText;
        [SerializeField] private Slider xpSlider; // optional

        [Header("Format")]
        [SerializeField] private string levelPrefix = "Уровень: ";
        [SerializeField] private string xpPrefix = "XP: ";

        private MetaProgress meta;
        private bool isBound;
        private Coroutine bindRoutine;

        private void Awake()
        {
            if (levelText == null) levelText = GetComponentInChildren<TMP_Text>(true);
        }

        private void OnEnable()
        {
            bindRoutine = StartCoroutine(BindRoutine());
        }

        private void OnDisable()
        {
            if (bindRoutine != null)
            {
                StopCoroutine(bindRoutine);
                bindRoutine = null;
            }
            Unbind();
        }

        private IEnumerator BindRoutine()
        {
            while (!isBound)
            {
                if (App.Instance?.Services != null)
                {
                    meta = App.Instance.Services.Get<MetaProgress>();
                    if (meta != null)
                    {
                        meta.Changed += Refresh;
                        isBound = true;

                        Debug.Log($"[MetaProgressUI] Bound OK metaId={meta.GetHashCode()} uiId={GetHashCode()}");

                        Refresh();
                        yield break;
                    }
                }

                yield return null;
            }
        }

        private void Unbind()
        {
            if (!isBound) return;

            if (meta != null)
                meta.Changed -= Refresh;

            isBound = false;
            meta = null;
        }

        private void Refresh()
        {
            if (meta == null) return;

            int need = Mathf.Max(1, meta.XpToNextLevel);
            int cur = Mathf.Clamp(meta.CurrentXp, 0, need);

            if (levelText != null)
                levelText.text = $"{levelPrefix}{meta.PlayerLevel}";

            if (xpText != null)
                xpText.text = $"{xpPrefix}{cur} / {need}";

            if (xpSlider != null)
                xpSlider.value = (float)cur / need;
        }
    }
}
