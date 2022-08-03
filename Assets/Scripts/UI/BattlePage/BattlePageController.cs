using System.Collections.Generic;
using Data;
using echo17.Signaler.Core;
using PlayerData;
using UI.PreBattlePage;
using UnityEngine;

namespace UI.BattlePage
{
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

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToBattle>(this, OnTransitionToBattle);
            Signaler.Instance.Subscribe<ClearBattleSelectionArrow>(this, OnClearSelectionArrow);
            
            Signaler.Instance.Broadcast(this, new RequestPlayerDataController{requester = this});
        }

        private bool OnClearSelectionArrow(ClearBattleSelectionArrow signal)
        {
            foreach (var playerHero in _inBattleHeroList) playerHero.ClearSelectionArrow();
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
            var enemyObjectInstance = Instantiate(enemyBattlePrefab, enemyTransform);
            var enemyInBattleControllerInstance = enemyObjectInstance.GetComponent<EnemyInBattleController>();
            enemyInBattleControllerInstance.Initialize(enemyData);
        }

        private EnemyData RandomizeEnemyFromPool()
        {
            var rng = Random.Range(0, enemyDataPool.enemyDataList.Count);
            return enemyDataPool.enemyDataList[rng];
        }

        private void PopulatePlayerHeroes()
        {
            //TODO:: Load player selected heroes here!
            var currentTeam = _playerDataController.GetCurrentTeam();

            for (var i = 0; i < playerHeroesTransforms.Count; i++)
            {
                if (currentTeam[i] != null)
                {
                    var playerHeroObject = Instantiate(playerHeroesBattlePrefab, playerHeroesTransforms[i]);
                    var inBattleHeroControllerInstance = playerHeroObject.GetComponent<PlayerInBattleHeroController>();
                    inBattleHeroControllerInstance.Initialize(currentTeam[i]);
                    _playerHeroObjectList.Add(playerHeroObject);
                    _inBattleHeroList.Add(inBattleHeroControllerInstance);
                }
            }
        }

        public void SetPlayerDataController(PlayerDataController playerDataController)
        {
            _playerDataController = playerDataController; 
        }
    }
}
