using System;
using echo17.Signaler.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.MainMenuPage
{
    public class MainMenuController : MonoBehaviour, IBroadcaster
    {

        [SerializeField] private Button startButton;
        private bool _isGameStarted;

        private void Awake()
        {
            SetupButtonEvents();
        }

        private void SetupButtonEvents()
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            if (_isGameStarted) return;
            _isGameStarted = true;

            AnimateFadeInToPreBattle();



        }

        private void AnimateFadeInToPreBattle()
        {
            //TODO :: Tweening transition
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());
        }
    }
}
