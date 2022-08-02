using System.ComponentModel;
using echo17.Signaler.Core;
using UI.MainMenuPage;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.PreBattlePage
{
    public class PreBattlePageController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [Header("Visual References")]
        [SerializeField]
        private Button toBattleButton;
        [SerializeField]
        private Button toTeamSelectionButton; 
        [SerializeField] private HorizontalLayoutGroup selectedHeroesLayoutGroup;
        [SerializeField] private GameObject preBattleLayoutObject;

        [Header("Data References")] 
        [SerializeField] private GameObject heroDisplayPrefab;
        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToPreBattle>(this, OnTransitionToPreBattle);
        }

        private bool OnTransitionToPreBattle(TransitionToPreBattle signal)
        {
            SetupButtonEvents();
            Initialize();
            preBattleLayoutObject.SetActive(true);
             
            return true;
        }

        private void SetupButtonEvents()
        {
            toTeamSelectionButton.onClick.RemoveAllListeners();
            toTeamSelectionButton.onClick.AddListener(TransitionToTeamSelectionPage);
            toBattleButton.onClick.RemoveAllListeners();
            toBattleButton.onClick.AddListener(TransitionToBattle);

        }

        private void TransitionToBattle()
        {
            Signaler.Instance.Broadcast(this, new TransitionToBattle());
        }

        private void TransitionToTeamSelectionPage()
        {
            Signaler.Instance.Broadcast(this, new TransitionToTeamSelection());
        }

        private void Initialize()
        {
            // ES3.LoadRawString(Globals.PlayerSaveJsonPath);
            
            //TODO:: Load existing data
            //TODO:: Populate existing heroes
            
                
            
        }
    }
}
