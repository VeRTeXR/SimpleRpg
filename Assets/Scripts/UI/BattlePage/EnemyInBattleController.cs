using System;
using Data;
using echo17.Signaler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePage
{
    public class EnemyInBattleController:MonoBehaviour, ISubscriber, IBroadcaster
    {
        [Header("Visual References")]
        [SerializeField] private Image enemyIcon;
        [SerializeField] private Image enemyOutline;
        [SerializeField] private Button enemyButton;
        private EnemyData _enemyData;
        private int _currentHealth;
        private bool _isInTargetSelectionPhase;

        private void Awake()
        {
            Signaler.Instance.Subscribe<StartTargetSelectionPhase>(this, OnStartTargetSelectionPhase);
            SetupButtonEvents();
        }

        private void SetupButtonEvents()
        {
            enemyButton.onClick.RemoveAllListeners();
            enemyButton.onClick.AddListener(SelectTarget);
        }

        private void SelectTarget()
        {
            if (!_isInTargetSelectionPhase) return;
            _isInTargetSelectionPhase = false;
            EnableOutline(false);
            Signaler.Instance.Broadcast(this, new PlayerSelectTarget { target = this});
        }

        private bool OnStartTargetSelectionPhase(StartTargetSelectionPhase signal)
        {
            _isInTargetSelectionPhase = true;
            EnableOutline(true);
            return true;
        }

        private void EnableOutline(bool isEnable)
        {
            enemyOutline.gameObject.SetActive(isEnable);
        }


        public void Initialize(EnemyData enemyData)
        {
            Debug.Log("Initialize Enemy!");
            _enemyData = enemyData;
            enemyIcon.color = _enemyData.enemyColor;
            _currentHealth = _enemyData.totalHp;
        }

        public void OnDamage(int selectedHeroAttackPoint)
        {
            _currentHealth -= selectedHeroAttackPoint;

            //TODO:: Animate Health Bar
            if (_currentHealth <= 0)
            {
                TriggerRoundOver();
            }
            else
            {
                Signaler.Instance.Broadcast(this, new StartEnemyTurn());
            }
        }

        private void TriggerRoundOver()
        {
            Debug.LogError("RoundOver");
        }
    }
}