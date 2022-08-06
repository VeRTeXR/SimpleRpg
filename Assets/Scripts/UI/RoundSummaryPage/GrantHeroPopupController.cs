using System;
using Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.RoundSummaryPage
{
    public class GrantHeroPopupController:MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private Image heroIcon;
        [SerializeField] private TextMeshProUGUI heroName;
        [SerializeField] private Button overlayButton;
        [SerializeField] private Button okButton;
        private HeroData _grantedHero;

        public void ShowPopup(HeroData grantedHero)
        {
            _grantedHero = grantedHero;
            gameObject.SetActive(true);
            SetupButtonEvents();
            PopulateHeroData();
        }

        private void PopulateHeroData()
        {
            heroIcon.color = _grantedHero.heroColor;
            heroName.text = _grantedHero.name;
        }

        private void SetupButtonEvents()
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(Exit);
            
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(Exit);
        }

        public void Exit()
        {
            gameObject.SetActive(false);
        }
    }
}