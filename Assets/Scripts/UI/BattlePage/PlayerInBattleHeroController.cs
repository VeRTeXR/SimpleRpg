using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.BattlePage
{
    public class PlayerInBattleHeroController: MonoBehaviour, IBroadcaster, ISubscriber
    {
        [Header("Visual References")]
        [SerializeField] private Button heroButton;
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject selectionArrow;

        public int AttackPoint => _heroData.attack;
        private PlayerOwnedHeroData _heroData;
        private bool _isInUnitSelectionPhase;

        private void Awake()
        {
            Signaler.Instance.Subscribe<StartPlayerUnitSelection>(this, OnStartPlayerUnitSelection);
        }

        private bool OnStartPlayerUnitSelection(StartPlayerUnitSelection signal)
        {
            _isInUnitSelectionPhase = true;
            return true;
        }


        public void Initialize(PlayerOwnedHeroData playerOwnedHeroData)
        {
            _heroData = playerOwnedHeroData;
            heroImage.color = _heroData.color;
            //TODO:: Initialize with Player selected hero data or null
            Debug.Log("Initialize Hero!");

            SetupButtonEvent();

        }

        private void SetupButtonEvent()
        {
            heroButton.onClick.RemoveAllListeners();
            heroButton.onClick.AddListener(TriggerHeroSelection);
        }

        private void TriggerHeroSelection()
        {
            Signaler.Instance.Broadcast(this, new ClearBattleSelectionArrow());
            selectionArrow.SetActive(true);
            Signaler.Instance.Broadcast(this, new PlayerSelectHero {heroController = this});
            Signaler.Instance.Broadcast(this, new StartTargetSelectionPhase());
        }


        public void ClearSelectionArrow()
        {
            selectionArrow.SetActive(false);
        }

    }
}