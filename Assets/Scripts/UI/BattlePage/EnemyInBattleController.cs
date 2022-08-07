using Data;
using echo17.Signaler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePage
{
    //This component controls the interactions and visual feedback for Enemy that is being spawn into battles
    public class EnemyInBattleController:MonoBehaviour, ISubscriber, IBroadcaster
    {
       
        [Header("Visual References")]
        [SerializeField] private Image enemyIcon;
        [SerializeField] private Image enemyOutline;
        [SerializeField] private Button enemyButton;
       
        public int MinAttack => _enemyData.minAttack;
        public int MaxAttack => _enemyData.maxAttack;
        
        private EnemyData _enemyData;
        private int _currentHealth;
        private bool _isInTargetSelectionPhase;
        private DamageTextGenerator _damageTextGenerator;
        private HealthBarController _healthBarController;
        private BattlePageController _battlePageController;
        private MessageSubscription<StartTargetSelectionPhase> _targetSelectSubscription;


        private void Awake()
        {
            _damageTextGenerator = GetComponent<DamageTextGenerator>();
            _healthBarController = GetComponent<HealthBarController>();
           _targetSelectSubscription =  Signaler.Instance.Subscribe<StartTargetSelectionPhase>(this, OnStartTargetSelectionPhase);
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


        public void Initialize(EnemyData enemyData, BattlePageController battlePageController)
        {
            _battlePageController = battlePageController;
            _enemyData = enemyData;
            enemyIcon.color = _enemyData.color;
            _currentHealth = _enemyData.totalHp;
            _healthBarController.SetFill(_enemyData.totalHp, _currentHealth);
            EnableOutline(false);
        }

        public void OnDamage(int selectedHeroAttackPoint)
        {
            _currentHealth -= selectedHeroAttackPoint;
            _damageTextGenerator.ShowDamageDealt(selectedHeroAttackPoint);
            _healthBarController.SetFill(_enemyData.totalHp, _currentHealth);
            if (_currentHealth <= 0)
                TriggerRoundOver();
            else
                Signaler.Instance.Broadcast(this, new StartEnemyTurn {enemyController = this});
        }

        private void TriggerRoundOver()
        {
            Signaler.Instance.Broadcast(this, new BattleRoundOver{isPlayerWin = true, inBattleHeroList = _battlePageController.GetAvailableHeroes()});
        }

        private void OnDestroy()
        {
            _targetSelectSubscription.UnSubscribe();
        }
    }
}