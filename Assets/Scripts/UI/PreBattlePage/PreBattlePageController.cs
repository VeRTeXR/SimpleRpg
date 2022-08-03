using System.Diagnostics;
using echo17.Signaler.Core;
using PlayerData;
using UI.MainMenuPage;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace UI.PreBattlePage
{
    public class PreBattlePageController : MonoBehaviour, ISubscriber, IBroadcaster, IRequiredPlayerDataController
    {
        [Header("Visual References")]
        [SerializeField]
        private Button toBattleButton;
        [SerializeField]
        private Button toTeamSelectionButton; 
        [SerializeField] private HorizontalLayoutGroup selectedHeroesLayoutGroup;
        [SerializeField] private GameObject preBattleLayoutObject;

        [Header("Data References")] 
        [SerializeField] private GameObject heroDisplayPrefab;

        private PlayerDataController _playerDataController;

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToPreBattle>(this, OnTransitionToPreBattle);
        }

        private bool OnTransitionToPreBattle(TransitionToPreBattle signal)
        {
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
            Signaler.Instance.Broadcast(this, new TransitionToBattle());
        }

        private void TransitionToTeamSelectionPage()
        {
            Signaler.Instance.Broadcast(this, new TransitionToTeamSelection());
        }



        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
            Initialize();
        }
        private void Initialize()
        {
            PopulateCurrentlySelectedHeroes();


        }

        private void PopulateCurrentlySelectedHeroes()
        {
            var currentTeam = _playerDataController.GetCurrentTeam();
            foreach (var ownedHero in currentTeam)
            {
                var heroDisplayObject = Instantiate(heroDisplayPrefab, selectedHeroesLayoutGroup.transform);
                heroDisplayObject.GetComponent<HeroDisplayController>().Initialize(ownedHero);
                
                
                Debug.LogError("ownedCurrentTeam : "+ownedHero.id);
            }
        }
    }
}
