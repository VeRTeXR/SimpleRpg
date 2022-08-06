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
            
            RefreshSelectionStatus();
        }
        
        private void Update()
        {
            if (_isHoldTimerStart) 
                _holdTime += Time.deltaTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isHoldTimerStart)
                AnimateHold();
            _isHoldTimerStart = true;
        }

        private void AnimateHold()
        {
             LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1.1f), Globals.ShowUnitTooltipTime).setEase(LeanTweenType.pingPong);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            gameObject.transform.localScale = Vector3.one;

            if (_holdTime < Globals.ShowUnitTooltipTime)
                HeroSelection();
            else
                ShowDetailTooltip();
            
            _isHoldTimerStart = false;
            _holdTime = 0;
        }

        private void HeroSelection()
        {
            if (_isDisplayOnly) return;
            
            if (_playerDataController.IsHeroAlreadyInCurrentTeam(_heroData))
            {
                _playerDataController.UnequipHeroFromCurrentTeam(_heroData);
                _playerDataController.SavePlayerData();
                
                RefreshSelectionStatus();

                return;
            }

            if (_playerDataController.GetCurrentTeamList().Count < Globals.MaxUnitInTeam)
            {
                _playerDataController.EquipHeroToCurrentTeam(_heroData);
                _playerDataController.SavePlayerData();
                
                RefreshSelectionStatus();
            }
            else
                Signaler.Instance.Broadcast(this, new ShowWarningText {text = Globals.TeamFullWarning});
        }

        private void RefreshSelectionStatus()
        {
            if (_isDisplayOnly)
            {
                selectedOutlineImage.gameObject.SetActive(false);
                selectedOutlineImage.transform.localScale = Vector3.zero;
            }
            else
            {
                if (_playerDataController.IsHeroAlreadyInCurrentTeam(_heroData))
                {
                    selectedOutlineImage.gameObject.SetActive(true);
                    LeanTween.scale(selectedOutlineImage.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.25f);
                }
                else
                    LeanTween.scale(selectedOutlineImage.gameObject, Vector3.zero, 0.25f);
            }
        }

        private void ShowDetailTooltip()
        {
            Signaler.Instance.Broadcast(this, new ShowUnitTooltip {requesterObject = gameObject, ownedUnitData = _heroData});
        }

    }
}