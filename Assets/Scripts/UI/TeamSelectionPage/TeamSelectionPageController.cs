using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using UI.MainMenuPage;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TeamSelectionPage
{
    public class TeamSelectionPageController : MonoBehaviour, ISubscriber, IBroadcaster, IRequiredPlayerDataController
    {
        [Header("Visual References")]
        [SerializeField] private GameObject layoutObject;

        [SerializeField] private GridLayoutGroup ownedHeroesGrid;
        [SerializeField] private Button backButton;

        [Header("Date References")] 
        [SerializeField] private GameObject heroIconPrefab;

        private PlayerDataController _playerDataController;
        private List<GameObject> _heroObjectList = new List<GameObject>();

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToTeamSelection>(this, OnTransitionToTeamSelection);
        }

        private bool OnTransitionToTeamSelection(TransitionToTeamSelection signal)
        {
            ClearExistingObject();
            SetupButtonEvents();
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController{requester = this});
            return true;
        }

        private void ClearExistingObject()
        {
            foreach (var heroObject in _heroObjectList) Destroy(heroObject);
            _heroObjectList.Clear();
        }

        private void SetupButtonEvents()
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(TransitionToPreBattle);
        }
        
        private void TransitionToPreBattle()
        {
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());
        }


        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
            layoutObject.SetActive(true);
            PopulateAllAvailableHeroes();
        }

        private void PopulateAllAvailableHeroes()
        {
            var ownedHeroList = _playerDataController.GetOwnedHeroList();
            
            foreach (var ownedHero in ownedHeroList)
            {
                var heroIconObject = Instantiate(heroIconPrefab, ownedHeroesGrid.transform);
                heroIconObject.GetComponent<HeroDisplayController>().Initialize(ownedHero, _playerDataController);
                _heroObjectList.Add(heroIconObject);
            }
        }
    }
}
