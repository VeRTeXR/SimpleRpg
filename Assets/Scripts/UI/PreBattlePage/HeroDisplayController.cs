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
        private bool _isDisplayOnly;


        public void Initialize(PlayerOwnedHeroData ownedHeroData, PlayerDataController playerDataController)
        {
            _isDisplayOnly = false;
            _playerDataController = playerDataController;
            _heroData = ownedHeroData;
            heroImage.color = ownedHeroData.color;
            
            RefreshSelectionStatus();
        }

        
        public void InitializeDisplay(PlayerOwnedHeroData ownedHeroData)
        {
            _isDisplayOnly = true;
            _heroData = ownedHeroData;
            heroImage.color = ownedHeroData.color;
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
            Debug.LogError(_holdTime);
            if (_holdTime < Globals.TriggerSelectionTime)
                ShowDetailTooltip();
            else
                HeroSelection();
            
            _isHoldTimerStart = false;
            _holdTime = 0;
        }

        private void HeroSelection()
        {
            if (_isDisplayOnly) return;
            
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
            if (_isDisplayOnly)
            {
                selectedOutlineImage.gameObject.SetActive(false);
                return;
            }
            
            selectedOutlineImage.gameObject.SetActive(_playerDataController.IsHeroAlreadyInCurrentTeam(_heroData));
        }

        private void ShowDetailTooltip()
        {
            Signaler.Instance.Broadcast(this, new ShowUnitTooltip {requesterObject = gameObject, ownedUnitData = _heroData});
        }

    }
}