using System;
using echo17.Signaler.Core;
using PlayerData;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI.BattlePage
{
    public class PlayerInBattleHeroController: MonoBehaviour, IBroadcaster, ISubscriber, IPointerDownHandler,IPointerUpHandler
    {
        [Header("Visual References")]
        [SerializeField] private Image heroImage;
        [SerializeField] private GameObject selectionArrow;
        [SerializeField] private GameObject selectionOutline;
        
        private PlayerOwnedHeroData _heroData;
        
        private int _currentHealth;

        private DamageTextGenerator _damageTextGenerator;
        private HealthBarController _healthBarController;
        private MessageSubscription<StartEnemyTurn> _startEnemyTurnSubscription;
        private MessageSubscription<StartPlayerUnitSelection> _startPlayerUnitSelectionSubscription;
        private bool _isHoldTimerStart;
        private float _holdTime;
        private bool _isPlayerTurn;
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
            ResetHoldInput();
            SetIsPlayerTurn(false);
            return true;
        }

        private bool OnStartPlayerUnitSelection(StartPlayerUnitSelection signal)
        {
            SetIsPlayerTurn(true);
            return true;
        }


        public void Initialize(PlayerOwnedHeroData playerOwnedHeroData)
        {
            _heroData = playerOwnedHeroData;
            heroImage.color = _heroData.color;
            _currentHealth = _heroData.currentHealth;
            _healthBarController.SetFill(_heroData.maxHealth, _currentHealth);
        }

        public void ClearSelectionArrow()
        {
            selectionArrow.SetActive(false);
            selectionOutline.SetActive(false);
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
            Signaler.Instance.Broadcast(this, new PlayerHeroKilled {heroController = this});
        }

        private void Update()
        {
            if (_isHoldTimerStart)
                _holdTime += Time.deltaTime;
        }
        
        private void OnDestroy()
        {
            _startEnemyTurnSubscription.UnSubscribe();
            _startPlayerUnitSelectionSubscription.UnSubscribe();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isPlayerTurn) return;
            _isHoldTimerStart = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {            
            if (!_isPlayerTurn) return;
                
            ResetHoldInput();

            if (_holdTime > Globals.TriggerBattleHeroDetailTooltipTime)
                Signaler.Instance.Broadcast(this,
                    new ShowUnitTooltip {ownedUnitData = _heroData, requesterObject = gameObject});
            else
            {
                Signaler.Instance.Broadcast(this, new ClearBattleSelectionArrow());

                selectionOutline.transform.localScale = Vector3.zero;
                selectionOutline.SetActive(true);
                LeanTween.scale(selectionOutline, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
                selectionArrow.SetActive(true);
                
                Signaler.Instance.Broadcast(this, new PlayerSelectHero {heroController = this});
                Signaler.Instance.Broadcast(this, new StartTargetSelectionPhase());
            }
        }
                
        private void ResetHoldInput()
        {
            _holdTime = 0;
            _isHoldTimerStart = false;
        }

        public void SetIsPlayerTurn(bool isPlayerTurn)
        {
            _isPlayerTurn = isPlayerTurn;
        }
    }
}