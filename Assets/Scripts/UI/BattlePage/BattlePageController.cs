using System.Collections.Generic;
using echo17.Signaler.Core;
using UI.PreBattlePage;
using UnityEngine;

namespace UI.BattlePage
{
    public class BattlePageController : MonoBehaviour, ISubscriber
    {

        [Header("Visual Reference")]
        [SerializeField] private GameObject layoutGameObject;
        [SerializeField] private List<Transform> playerHeroesTransforms = new List<Transform>();
        [SerializeField] private Transform enemyTransform; 
        [Header("Data Reference")] 
        [SerializeField] private GameObject playerHeroesBattlePrefab;
        [SerializeField] private GameObject enemyBattlePrefab;
        private List<PlayerInBattleHeroController> _inBattleHeroList = new List<PlayerInBattleHeroController>();

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToBattle>(this, OnTransitionToBattle);
        }

        private bool OnTransitionToBattle(TransitionToBattle signal)
        {
            _inBattleHeroList.Clear();
            layoutGameObject.SetActive(true);
            PopulatePlayerHeroes();
            PopulateEnemy();
            return true;
        }

        private void PopulateEnemy()
        {
            var enemyObjectInstance = Instantiate(enemyBattlePrefab, enemyTransform);
            var enemyInBattleControllerInstance = enemyObjectInstance.GetComponent<EnemyInBattleController>();
            enemyInBattleControllerInstance.Initialize();
        }

        private void PopulatePlayerHeroes()
        {
            //TODO:: Load player selected heroes here!

            foreach (var heroSpawn in playerHeroesTransforms)
            {
                var playerHeroObjectInstance = Instantiate(playerHeroesBattlePrefab, heroSpawn);
                var inBattleHeroControllerInstance = playerHeroObjectInstance.GetComponent<PlayerInBattleHeroController>();
                inBattleHeroControllerInstance.Initialize();
                _inBattleHeroList.Add(inBattleHeroControllerInstance);

                playerHeroObjectInstance.transform.localPosition = Vector3.zero;
            }
        }
    }
}
