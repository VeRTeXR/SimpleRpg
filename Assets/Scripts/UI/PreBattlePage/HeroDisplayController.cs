using System;
using echo17.Signaler.Core;
using PlayerData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI.PreBattlePage
{
    public class HeroDisplayController:MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBroadcaster
    {
        [Header("Visual References")] 
        [SerializeField] private Image selectedOutlineImage; 
        [SerializeField] private Image heroImage;
        
        
        private PlayerOwnedHeroData _heroData;
        private bool _isHoldTimerStart;
        private float _holdTime;
        private PlayerDataController _playerDataController;



        public void Initialize(PlayerOwnedHeroData ownedHeroData, PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
            _heroData = ownedHeroData;
            heroImage.color = ownedHeroData.color;
            
            RefreshSelectionStatus();
        }

        private void Update()
        {
            if (_isHoldTimerStart) 
                _holdTime += Time.deltaTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHoldTimerStart = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //TODO:: Enable tooltip here!
            Debug.LogError(_holdTime);
            if (_holdTime < Globals.TriggerSelectionTime)
                HeroSelection();
                // ShowDetailTooltip();
            // else
                // HeroSelection();
            _isHoldTimerStart = false;
            _holdTime = 0;
        }

        private void HeroSelection()
        {
         
            if (_playerDataController.IsHeroAlreadyInCurrentTeam(_heroData))
            {
                Debug.LogError("Unequip");
                _playerDataController.UnequipHeroFromCurrentTeam(_heroData);
                RefreshSelectionStatus();
                _playerDataController.SavePlayerData();
                return;
            }

            if (_playerDataController.GetCurrentTeamList().Count < Globals.MaxUnitInTeam)
            {
                Debug.LogError("Equip");
                _playerDataController.EquipHeroToCurrentTeam(_heroData);
                RefreshSelectionStatus();
                _playerDataController.SavePlayerData();
            }
            else
                Signaler.Instance.Broadcast(this, new ShowWarningText {text = Globals.TeamFullWarning});
        }

        private void RefreshSelectionStatus()
        {
            if(_playerDataController.IsHeroAlreadyInCurrentTeam(_heroData))
                selectedOutlineImage.gameObject.SetActive(true);
            else 
                selectedOutlineImage.gameObject.SetActive(false);
        }

        private void ShowDetailTooltip()
        {
            Signaler.Instance.Broadcast(this, new ShowUnitTooltip {requesterObject = gameObject, ownedUnitData = _heroData});
        }

    }
}