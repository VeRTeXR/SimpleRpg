using System;
using echo17.Signaler.Core;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI.BattlePage
{
    public class TurnTracker : MonoBehaviour, ISubscriber
    {
        [SerializeField] private TextMeshProUGUI turnText;
        private PlayerInBattleHeroController _selectedHero;
        private EnemyInBattleController _selectedEnemy;

        private void Awake()
        {
            Signaler.Instance.Subscribe<BattleStart>(this, OnBattleStart);
            Signaler.Instance.Subscribe<PlayerSelectTarget>(this, OnPlayerSelectTarget);
            Signaler.Instance.Subscribe<PlayerSelectHero>(this, OnPlayerSelectHero);

            Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStart);
            turnText.gameObject.SetActive(false);
        }

        private bool OnEnemyTurnStart(StartEnemyTurn signal)
        {
            _selectedHero = null;
            _selectedEnemy = null;
            turnText.gameObject.SetActive(true);
            turnText.text = Globals.EnemyTurn;
            
            return true;
        }

        private bool OnPlayerSelectHero(PlayerSelectHero signal)
        {
            _selectedHero = signal.heroController;
            return true;
        }

        private bool OnPlayerSelectTarget(PlayerSelectTarget signal)
        {
            _selectedEnemy = signal.target;
            ApplyDamageToTarget();
            return true;
        }

        private void ApplyDamageToTarget()
        {
            _selectedEnemy.OnDamage(_selectedHero.AttackPoint);
        }

        private bool OnBattleStart(BattleStart signal)
        {
            StartPlayerTurn();
            return true;
        }

        private void StartPlayerTurn()
        {
            turnText.text = Globals.PlayerTurn;
        }
    } 
}
