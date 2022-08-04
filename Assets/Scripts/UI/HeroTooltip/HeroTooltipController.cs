using echo17.Signaler.Core;
using PlayerData;
using TMPro;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HeroTooltip
{
    public class HeroTooltipController : MonoBehaviour, ISubscriber
    {
        [Header("Visual References")]        
        [SerializeField] private GameObject tooltipInstance;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button overlayButton;
        
        private PlayerOwnedHeroData _selectedOwnedHeroData;

        private void Awake()
        {
            tooltipInstance.SetActive(false);
            Signaler.Instance.Subscribe<ShowUnitTooltip>(this, OnShowUnitTooltip);

            SetupButtonEvents();
        }

        private void SetupButtonEvents()
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(HideTooltip);
        }

        private void HideTooltip()
        {
            overlayButton.gameObject.SetActive(false);
            tooltipInstance.SetActive(false);
        }

        private bool OnShowUnitTooltip(ShowUnitTooltip signal)
        {
            overlayButton.gameObject.SetActive(true);
            tooltipInstance.transform.position = signal.requesterObject.transform.position;
            _selectedOwnedHeroData = signal.ownedUnitData;

            nameText.text = _selectedOwnedHeroData.name;
            healthText.text = _selectedOwnedHeroData.currentHealth.ToString();
            attackText.text = _selectedOwnedHeroData.attack.ToString();
            levelText.text = _selectedOwnedHeroData.level.ToString();
            experienceText.text = _selectedOwnedHeroData.experience.ToString();
            
            tooltipInstance.SetActive(true);
            return true;
        }
    }
}
