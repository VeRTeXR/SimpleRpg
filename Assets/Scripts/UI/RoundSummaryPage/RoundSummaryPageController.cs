using System;
using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using TMPro;
using UI.BattlePage;
using UI.MainMenuPage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.RoundSummaryPage
{
    public class RoundSummaryPageController : MonoBehaviour,ISubscriber, IRequiredPlayerDataController, IBroadcaster
    {
        [Header("Visual References")]
        [SerializeField] private Button backToPreBattleButton;
        [SerializeField] private TextMeshProUGUI roundStateText;
        [SerializeField] private GameObject layoutObject;
        [SerializeField] private GrantHeroPopupController grantHeroPopupController;
        
        private PlayerDataController _playerDataController;

      
        private void Awake()
        {
            Signaler.Instance.Subscribe<BattleRoundOver>(this, OnBattleRoundOver);
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
            // if (_playerDataController.GetCurrentPlayerRound() % 5 == 0)
            // {
                if (_playerDataController.GetOwnedHeroList().Count < Globals.HeroInventoryLimit)
                {
                    var grantedHero = _playerDataController.GrantRandomHero();
                    Debug.LogError(grantedHero.id);
                    grantHeroPopupController.ShowPopup(grantedHero);
                    //TODO:: Show hero grant popup??
                }
            // }

            _playerDataController.SavePlayerData();
        }

        private void SetupButtonEvents()
        {
            backToPreBattleButton.onClick.RemoveAllListeners();
            backToPreBattleButton.onClick.AddListener(TransitionBackToPreBattle);
        }

        private void TransitionBackToPreBattle()
        {
            //TODO:: Handle no more hero left, transition back to start page
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());
        }

    }
}
