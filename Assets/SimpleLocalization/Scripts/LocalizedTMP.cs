using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;

namespace SimpleLocalization.Scripts {
    /// <summary>
    /// Localize TMP component.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMP : MonoBehaviour {
        public string LocalizationKey;

        public void Start() {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy() {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize() {
            GetComponent<TextMeshProUGUI>().text = LocalizationManager.Localize(LocalizationKey);
        }
    }
}
