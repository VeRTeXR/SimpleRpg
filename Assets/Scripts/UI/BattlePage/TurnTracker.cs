using echo17.Signaler.Core;
using TMPro;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace UI.BattlePage
{
    public class TurnTracker : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [SerializeField] private TextMeshProUGUI turnText;
        private PlayerInBattleHeroController _selectedHero;
        private EnemyInBattleController _selectedEnemy;
        private BattlePageController _battlePage;

        private void Awake()
        {
            _battlePage = GetComponent<BattlePageController>();
            turnText.gameObject.SetActive(false);
            
            Signaler.Instance.Subscribe<BattleStart>(this, OnBattleStart);
            Signaler.Instance.Subscribe<StartPlayerUnitSelection>(this, OnStartPlayerUnitSelection);
            Signaler.Instance.Subscribe<PlayerSelectTarget>(this, OnPlayerSelectTarget);
            Signaler.Instance.Subscribe<PlayerSelectHero>(this, OnPlayerSelectHero);
            Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStart);
            Signaler.Instance.Subscribe<BattleRoundOver>(this, OnBattleRoundOver);
        }

        private bool OnBattleRoundOver(BattleRoundOver signal)
        {
            if (signal.isPlayerWin)
                _battlePage.Exit();
            turnText.gameObject.SetActive(false);
            return true;
        }

        private bool OnStartPlayerUnitSelection(StartPlayerUnitSelection signal)
        {
            SetPlayerTurnText();
            return true;
        }

        private bool OnEnemyTurnStart(StartEnemyTurn signal)
        {
            _selectedHero = null;
            _selectedEnemy = null;
            turnText.gameObject.SetActive(true);
            turnText.text = Globals.EnemyTurn;

            var targetHero = RandomizeTargetForEnemy();
            ApplyDamageToHero(targetHero, signal.enemyController); 
            
            return true;
        }

        private void ApplyDamageToHero(PlayerInBattleHeroController targetHero, EnemyInBattleController signalEnemyController)
        {
            var attackPoint = Random.Range(signalEnemyController.MinAttack, signalEnemyController.MaxAttack);
            targetHero.OnDamage(attackPoint);
        }

        private PlayerInBattleHeroController RandomizeTargetForEnemy()
        {
            var heroesList = _battlePage.GetAvailableHeroes();
            var targetRng = Random.Range(0, heroesList.Count);
            return heroesList[targetRng];
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
            Signaler.Instance.Broadcast(this, new ClearBattleSelectionArrow());
            return true;
        }

        private void ApplyDamageToTarget()
        {
            _selectedEnemy.OnDamage(_selectedHero.AttackPoint);
        }

        private bool OnBattleStart(BattleStart signal)
        {
            SetPlayerTurnText();
            var heroesList = _battlePage.GetAvailableHeroes();
            foreach (var playerInBattleHeroController in heroesList) playerInBattleHeroController.SetIsPlayerTurn(true);
            return true;
        }

        private void SetPlayerTurnText()
        {
            turnText.gameObject.SetActive(true);
            turnText.text = Globals.PlayerTurn;
        }
    } 
}
