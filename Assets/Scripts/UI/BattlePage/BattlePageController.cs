using System.Collections.Generic;
using Data;
using echo17.Signaler.Core;
using PlayerData;
using UI.MainMenuPage;
using UI.PreBattlePage;
using UnityEngine;

namespace UI.BattlePage
{
    //This component responsible for populating battles and handling of battle page elements
    public class BattlePageController : MonoBehaviour, ISubscriber,IBroadcaster, IRequiredPlayerDataController
    {

        [Header("Visual Reference")]
        [SerializeField] private GameObject layoutGameObject;
        [SerializeField] private List<Transform> playerHeroesTransforms = new List<Transform>();
        [SerializeField] private Transform enemyTransform; 
        [Header("Data Reference")] 
        [SerializeField] private GameObject playerHeroesBattlePrefab;
        [SerializeField] private GameObject enemyBattlePrefab;
        [SerializeField] private EnemyDataPool enemyDataPool;
       
        
        private List<PlayerInBattleHeroController> _inBattleHeroList = new List<PlayerInBattleHeroController>();
        private PlayerDataController _playerDataController;
        private List<GameObject> _playerHeroObjectList = new List<GameObject>();
        private GameObject _enemyObjectInstance;

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToPreBattle>(this, OnTransitionToPreBattle);
            Signaler.Instance.Subscribe<TransitionToTeamSelection>(this, OnTransitionToTeamSelection);
            Signaler.Instance.Subscribe<TransitionToBattle>(this, OnTransitionToBattle);
            Signaler.Instance.Subscribe<ClearBattleSelection>(this, OnClearSelection);
            Signaler.Instance.Subscribe<PlayerHeroKilled>(this, OnPlayerHeroKilled);
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController{requester = this});
        }

        private bool OnTransitionToTeamSelection(TransitionToTeamSelection signal)
        {
            ClearExistingObjects();
            return true;
        }

        private bool OnTransitionToPreBattle(TransitionToPreBattle signal)
        {
            ClearExistingObjects();
            return true;
        }


        private void ClearExistingObjects()
        {
            if(_enemyObjectInstance != null) 
                Destroy(_enemyObjectInstance);
            if (_playerHeroObjectList.Count > 0)
                foreach (var heroObject in _playerHeroObjectList)
                    Destroy(heroObject);
                    
            _playerHeroObjectList.Clear();
            _inBattleHeroList.Clear();
        }

        private bool OnPlayerHeroKilled(PlayerHeroKilled signal)
        {
            _inBattleHeroList.Remove(signal.heroController);
            Destroy(signal.heroController.gameObject);

            if (_inBattleHeroList.Count <= 0)
            {
                Exit();
                Signaler.Instance.Broadcast(this, new BattleRoundOver{isPlayerWin = false});
            }
            return true;
        }

        private bool OnClearSelection(ClearBattleSelection signal)
        {
            foreach (var playerHero in _inBattleHeroList) playerHero.ClearSelection();
            return true;
        }

        private bool OnTransitionToBattle(TransitionToBattle signal)
        {
            _inBattleHeroList.Clear();
            layoutGameObject.SetActive(true);
            PopulatePlayerHeroes();
            PopulateEnemy();

            Signaler.Instance.Broadcast(this, new BattleStart());
            return true;
        }

        private void PopulateEnemy()
        {
            var enemyData = RandomizeEnemyFromPool();
            _enemyObjectInstance = Instantiate(enemyBattlePrefab, enemyTransform);
            var enemyInBattleControllerInstance = _enemyObjectInstance.GetComponent<EnemyInBattleController>();
            enemyInBattleControllerInstance.Initialize(enemyData, this);
        }

        private EnemyData RandomizeEnemyFromPool()
        {
            var rng = Random.Range(0, enemyDataPool.enemyDataList.Count);
            return enemyDataPool.enemyDataList[rng];
        }

        private void PopulatePlayerHeroes()
        {
            var currentTeam = _playerDataController.GetCurrentTeamList();
            for (var i = 0; i < playerHeroesTransforms.Count; i++)
            {
                if (i > playerHeroesTransforms.Count) return;
                if (currentTeam[i] == null) continue;
                
                var playerHeroObject = Instantiate(playerHeroesBattlePrefab, playerHeroesTransforms[i]);
                var inBattleHeroControllerInstance = playerHeroObject.GetComponent<PlayerInBattleHeroController>();
                inBattleHeroControllerInstance.Initialize(currentTeam[i]);
                _playerHeroObjectList.Add(playerHeroObject);
                _inBattleHeroList.Add(inBattleHeroControllerInstance);
            }
        }

        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController;
        }

        public List<PlayerInBattleHeroController> GetAvailableHeroes()
        {
            return _inBattleHeroList;
        }

        public void Exit()
        {
            layoutGameObject.SetActive(false);
        }
    }
}
