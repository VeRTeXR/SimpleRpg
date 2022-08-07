using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using TMPro;
using UI.MainMenuPage;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.PreBattlePage
{
    //This component populate current team heroes for display before enter a BattlePage or TeamSelectionPage
    public class PreBattlePageController : MonoBehaviour, ISubscriber, IBroadcaster, IRequiredPlayerDataController
    {
        [Header("Visual References")]
        [SerializeField] private Button toBattleButton;
        [SerializeField] private Button toTeamSelectionButton; 
        [SerializeField] private GameObject preBattleLayoutObject;
        [SerializeField] private HorizontalLayoutGroup selectedHeroesLayoutGroup;
        [SerializeField] private TextMeshProUGUI currentRoundText;
        [SerializeField] private TextMeshProUGUI noHeroInTeamText;
        
        
        [Header("Data References")] 
        [SerializeField] private GameObject heroDisplayPrefab;

        private PlayerDataController _playerDataController;
        private List<GameObject> _heroDisplayObjectList = new List<GameObject>();

        private void Awake()
        {
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
            Signaler.Instance.Subscribe<TransitionToPreBattle>(this, OnTransitionToPreBattle);
            noHeroInTeamText.gameObject.SetActive(false);
        }

        private bool OnGameOver(GameOver signal)
        {
            preBattleLayoutObject.SetActive(false);
            return true;
        }

        private bool OnTransitionToPreBattle(TransitionToPreBattle signal)
        {
            ClearExistingDisplay();
            SetupButtonEvents();
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController {requester = this});
            preBattleLayoutObject.SetActive(true);
             
            return true;
        }

        private void SetupButtonEvents()
        {
            toTeamSelectionButton.onClick.RemoveAllListeners();
            toTeamSelectionButton.onClick.AddListener(TransitionToTeamSelectionPage);
            toBattleButton.onClick.RemoveAllListeners();
            toBattleButton.onClick.AddListener(TransitionToBattle);
        }

        private void TransitionToBattle()
        {
            if (_playerDataController.GetCurrentTeamList().Count < Globals.MaxUnitInTeam)
            {
                Signaler.Instance.Broadcast(this, new ShowWarningText{text = Globals.BattleTeamUnitRequirementHint});
                return;
            }

            ClearExistingDisplay();            
            Signaler.Instance.Broadcast(this, new TransitionToBattle());
        }

        private void TransitionToTeamSelectionPage()
        {
            ClearExistingDisplay();
            Signaler.Instance.Broadcast(this, new TransitionToTeamSelection());
        }

        private void ClearExistingDisplay()
        {
            foreach (var heroObject in _heroDisplayObjectList) Destroy(heroObject);
            _heroDisplayObjectList.Clear();
        }


        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
            Initialize();
        }
        private void Initialize()
        {
            PopulateCurrentRound();
            PopulateCurrentlySelectedHeroes();
        }

        private void PopulateCurrentRound()
        {
            currentRoundText.text = "Current Battle Round : "+_playerDataController.GetCurrentPlayerRound();
        }

        private void PopulateCurrentlySelectedHeroes()
        {
            var currentTeam = _playerDataController.GetCurrentTeamList();
            if (currentTeam.Count <= 0)
            {
                noHeroInTeamText.gameObject.SetActive(true);
                return;
            }
            
            noHeroInTeamText.gameObject.SetActive(false);
            foreach (var ownedHero in currentTeam)
            {
                var heroDisplayObject = Instantiate(heroDisplayPrefab, selectedHeroesLayoutGroup.transform);
                heroDisplayObject.GetComponent<HeroDisplayController>().InitializeDisplay(ownedHero);
                _heroDisplayObjectList.Add(heroDisplayObject);
            }
        }
    }
}
