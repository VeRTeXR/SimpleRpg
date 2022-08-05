using System;
using echo17.Signaler.Core;
using PlayerData;
using TMPro;
using UI.MainMenuPage;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.GameOverPage
{
    public class GameOverPageController : MonoBehaviour, ISubscriber, IBroadcaster, IRequiredPlayerDataController
    {
        [Header("Visual References")]
        [SerializeField] private GameObject layoutObject;
        [SerializeField] private Button restartButton;
        [SerializeField] private TextMeshProUGUI totalRoundText;
        private PlayerDataController _playerDataController;

        private void Awake()
        {
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
        }

        private void Start()
        {
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController {requester = this});
        }

        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
        }

        private bool OnGameOver(GameOver signal)
        {
            layoutObject.SetActive(true);
            totalRoundText.text = "Round Survived : "+(_playerDataController.GetCurrentPlayerRound()-1);
          
            SetupButtonEvents();
            return true;
        }

        private void SetupButtonEvents()
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(TransitionBackToMainMenu);
        }

        private void TransitionBackToMainMenu()
        {
            layoutObject.SetActive(false);

            Signaler.Instance.Broadcast(this, new RegeneratePlayerData());
            Signaler.Instance.Broadcast(this, new TransitionToMainMenu());
        }

     }
}
