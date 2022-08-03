using System;
using echo17.Signaler.Core;
using PlayerData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePage
{
    public class PlayerInBattleHeroController: MonoBehaviour, IBroadcaster, ISubscriber
    {
        [Header("Visual References")]
        [SerializeField] private Button heroButton;
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject selectionArrow;

        private PlayerOwnedHeroData _heroData;
        private bool _isInUnitSelectionPhase;
        private int _currentHealth;

        private DamageTextGenerator _damageTextGenerator;
        private HealthBarController _healthBarController;
        private MessageSubscription<StartEnemyTurn> _startEnemyTurnSubscription;
        private MessageSubscription<StartPlayerUnitSelection> _startPlayerUnitSelectionSubscription;
        public PlayerOwnedHeroData HeroData => _heroData;
        public int CurrentHealth => _currentHealth;
        public string Id=> _heroData.id;
        public int AttackPoint => _heroData.attack;

        private void Awake()
        {
            _damageTextGenerator = GetComponent<DamageTextGenerator>();
            _healthBarController = GetComponent<HealthBarController>();
            _startEnemyTurnSubscription = Signaler.Instance.Subscribe<StartEnemyTurn>(this, OnEnemyTurnStart);
            _startPlayerUnitSelectionSubscription =
                Signaler.Instance.Subscribe<StartPlayerUnitSelection>(this, OnStartPlayerUnitSelection);
        }

        private bool OnEnemyTurnStart(StartEnemyTurn signal)
        {
            heroButton.enabled = false;
            return true;
        }

        private bool OnStartPlayerUnitSelection(StartPlayerUnitSelection signal)
        {
            heroButton.enabled = true;
            _isInUnitSelectionPhase = true;
            return true;
        }


        public void Initialize(PlayerOwnedHeroData playerOwnedHeroData)
        {
            _heroData = playerOwnedHeroData;
            heroImage.color = _heroData.color;
            _currentHealth = _heroData.currentHealth;
            _healthBarController.SetFill(_heroData.maxHealth, _currentHealth);
            
            SetupButtonEvent();
        }

        private void SetupButtonEvent()
        {
            heroButton.onClick.RemoveAllListeners();
            heroButton.onClick.AddListener(TriggerHeroSelection);
        }

        private void TriggerHeroSelection()
        {
            Signaler.Instance.Broadcast(this, new ClearBattleSelectionArrow());
            selectionArrow.SetActive(true);
            Signaler.Instance.Broadcast(this, new PlayerSelectHero {heroController = this});
            Signaler.Instance.Broadcast(this, new StartTargetSelectionPhase());
        }


        public void ClearSelectionArrow()
        {
            selectionArrow.SetActive(false);
        }

        public void OnDamage(int attackPoint)
        {
            _currentHealth -= attackPoint;
            _damageTextGenerator.ShowDamageDealt(attackPoint);
            _healthBarController.SetFill(_heroData.maxHealth, _currentHealth);
            
            Debug.LogError("hero hp : "+_currentHealth+ " ::: "+attackPoint);
            if (_currentHealth <= 0)
                PlayerHeroKilled();
            
            Signaler.Instance.Broadcast(this, new StartPlayerUnitSelection());
        }

        private void PlayerHeroKilled()
        {
            //TODO:: trigger dead animation seq
            Signaler.Instance.Broadcast(this, new PlayerHeroKilled {heroController= this });
        }

        private void OnDestroy()
        {
            _startEnemyTurnSubscription.UnSubscribe();
            _startPlayerUnitSelectionSubscription.UnSubscribe();
        }
    }
}