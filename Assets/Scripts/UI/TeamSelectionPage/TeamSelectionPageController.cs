using System;
using echo17.Signaler.Core;
using UI.MainMenuPage;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TeamSelectionPage
{
    public class TeamSelectionPageController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [Header("Visual References")]
        [SerializeField] private GameObject layoutObject;
        [SerializeField] private Button backButton;

        [Header("Date References")] 
        [SerializeField] private GameObject heroesIconPrefab;
        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToTeamSelection>(this, OnTransitionToTeamSelection);
        }

        private bool OnTransitionToTeamSelection(TransitionToTeamSelection signal)
        {
            SetupButtonEvents();

            layoutObject.SetActive(true);
            return true;
        }

        private void SetupButtonEvents()
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(TransitionToPreBattle);
        }

        private void TransitionToPreBattle()
        {
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());
        }
    }
}
