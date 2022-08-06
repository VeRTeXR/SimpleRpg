using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using TMPro;
using UI.BattlePage;
using UI.MainMenuPage;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.RoundSummaryPage
{
    public class RoundSummaryPageController : MonoBehaviour,ISubscriber, IRequiredPlayerDataController, IBroadcaster
    {
        [Header("Visual References")]
        [SerializeField] private Button backToPreBattleButton;
        [SerializeField] private Button backToTeamSelectionButton;
        [SerializeField] private TextMeshProUGUI roundStateText;
        [SerializeField] private GameObject layoutObject;
        [SerializeField] private GrantHeroPopupController grantHeroPopupController;
        [SerializeField] private InTeamHeroStatusSummaryController inTeamHeroStatusSummaryController;
        
        
        private PlayerDataController _playerDataController;

      
        private void Awake()
        {
            Signaler.Instance.Subscribe<BattleRoundOver>(this, OnBattleRoundOver);
            Signaler.Instance.Subscribe<GameOver>(this,OnGameOver);
        }

        private bool OnGameOver(GameOver signal)
        {
            layoutObject.SetActive(false);
            return true;
        }

        private void Start()
        {
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController {requester = this});
        }

        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
        }

       

        private bool OnBattleRoundOver(BattleRoundOver signal)
        {
            SetupButtonEvents();
            
            PopulateVisual(signal.isPlayerWin);
            UpdatePlayerTeamStatus(signal.inBattleHeroList);
            IncrementPlayerRound();
            _playerDataController.SavePlayerData();
            return true;
        }
        private void UpdatePlayerTeamStatus(List<PlayerInBattleHeroController> playerInBattleHeroControllers)
        {
            _playerDataController.UpdateCurrentTeamHealth(playerInBattleHeroControllers);
        }
        private void PopulateVisual(bool signalIsPlayerWin)
        {
            layoutObject.SetActive(true);
            if (signalIsPlayerWin)
                roundStateText.text = Globals.PlayerWonHeader;
            else
                roundStateText.text = Globals.PlayerLose;

        }
        

        private void IncrementPlayerRound()
        {
            _playerDataController.IncrementRound();
            if (_playerDataController.GetCurrentPlayerRound() % Globals.RoundToGrantHeroToPlayer == 0)
            {
                if (_playerDataController.GetOwnedHeroList().Count < Globals.HeroInventoryLimit)
                {
                    var grantedHeroData = _playerDataController.GrantRandomHero();
                    grantHeroPopupController.ShowPopup(grantedHeroData);
                }
            }

            var levelUpUnitList = _playerDataController.IncrementExpForEachUnit();
            inTeamHeroStatusSummaryController.Animate(_playerDataController.GetCurrentTeamList(),levelUpUnitList);
            _playerDataController.SavePlayerData();
        }

        private void SetupButtonEvents()
        {
            backToPreBattleButton.onClick.RemoveAllListeners();
            backToPreBattleButton.onClick.AddListener(TransitionBackToPreBattle);
            
            backToTeamSelectionButton.onClick.RemoveAllListeners();
            backToTeamSelectionButton.onClick.AddListener(TransitionBackToTeamSelection);
        }

        private void TransitionBackToTeamSelection()
        {
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new TransitionToTeamSelection());
        }

        private void TransitionBackToPreBattle()
        {
            //TODO:: Handle no more hero left, transition back to start page
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());
        }

    }
}
