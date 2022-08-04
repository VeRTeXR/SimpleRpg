using echo17.Signaler.Core;
using PlayerData;
using Unity.VisualScripting;
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

        public void Initialize(PlayerOwnedHeroData ownedHeroData)
        {
            _heroData = ownedHeroData;
            heroImage.color = ownedHeroData.color;
        }

        private void Update()
        {
            if (_isHoldTimerStart)
            {
                _holdTime += Time.deltaTime;
            }
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
            // TODO:: if current team is full show warning
            Signaler.Instance.Broadcast(this, new ShowWarningText {text = Globals.TeamFullWarning});
            //TODO :: if hero already selected, deselect. else put into current team
        }

        private void ShowDetailTooltip()
        {
            Signaler.Instance.Broadcast(this, new ShowUnitTooltip {requesterObject = gameObject, ownedUnitData = _heroData});
        }
    }
}