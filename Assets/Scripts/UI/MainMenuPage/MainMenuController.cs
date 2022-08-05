using echo17.Signaler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenuPage
{
    public class MainMenuController : MonoBehaviour, IBroadcaster, ISubscriber
    {
        [Header("Visual References")]
        [SerializeField] private GameObject layout; 
        [SerializeField] private Button startButton;
        private bool _isGameStartButtonClicked;

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToMainMenu>(this, OnTransitionToMainMenu);
            SetupButtonEvents();

        }

        private bool OnTransitionToMainMenu(TransitionToMainMenu signal)
        {
            _isGameStartButtonClicked = false;
            SetupButtonEvents();
            layout.SetActive(true);
            return true;
        }

        private void SetupButtonEvents()
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            if (_isGameStartButtonClicked) return;
            _isGameStartButtonClicked = true;
            Signaler.Instance.Broadcast(this, new TransitionToPreBattle());

        }
    }
}
