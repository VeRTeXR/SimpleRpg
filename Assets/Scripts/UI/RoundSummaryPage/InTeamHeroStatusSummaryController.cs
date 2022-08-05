using System.Collections.Generic;
using echo17.Signaler.Core;
using PlayerData;
using UI.MainMenuPage;
using UI.PreBattlePage;
using UnityEngine;
using UnityEngine.UI;

namespace UI.RoundSummaryPage
{
    public class InTeamHeroStatusSummaryController: MonoBehaviour, ISubscriber
    {
        [Header("Visual References")]
        [SerializeField] private HorizontalLayoutGroup heroLayoutGroup;        
       
        [Header("Data References")]
        [SerializeField] private GameObject heroDisplayPrefab;
        [SerializeField] private GameObject statusChangeTextPrefab;
        private List<PlayerOwnedHeroData> _currentTeam;
        private List<GameObject> _heroDisplayInstanceList = new List<GameObject>();

        private void Awake()
        {
            Signaler.Instance.Subscribe<TransitionToTeamSelection>(this, Exit);
            Signaler.Instance.Subscribe<TransitionToPreBattle>(this, Exit);
        }

        private bool Exit(TransitionToPreBattle signal)
        {
            ClearExistingEntries();
            return true;
        }

      
        private bool Exit(TransitionToTeamSelection signal)
        {
            ClearExistingEntries();
            return true;
        }
        
        private void ClearExistingEntries()
        {
            foreach (var heroDisplayObject in _heroDisplayInstanceList) Destroy(heroDisplayObject);
            _heroDisplayInstanceList.Clear();
        }

        public void Animate(List<PlayerOwnedHeroData> currentTeamList, List<PlayerOwnedHeroData> levelUpUnitList)
        {
            _currentTeam = currentTeamList;
            foreach (var inTeamHeroData in _currentTeam)
            {
                var heroDisplayInstance = Instantiate(heroDisplayPrefab, heroLayoutGroup.transform);
                heroDisplayInstance.GetComponent<HeroDisplayController>().InitializeDisplay(inTeamHeroData);

                _heroDisplayInstanceList.Add(heroDisplayInstance);
                if (levelUpUnitList.Contains(inTeamHeroData))
                    PopulateLevelUpText(heroDisplayInstance);
                else
                    PopulateStatusText(heroDisplayInstance);
            }                
        }

        private void PopulateLevelUpText(GameObject heroDisplayInstance)
        {
            var statusChangeTextInstance = Instantiate(statusChangeTextPrefab, heroDisplayInstance.transform);
            statusChangeTextInstance.GetComponent<StatusChangeTextController>().Initialize("Lv UP!", Color.yellow);
    
        }

        private void PopulateStatusText(GameObject heroDisplayInstance)
        {
            var statusChangeTextInstance = Instantiate(statusChangeTextPrefab, heroDisplayInstance.transform);
            statusChangeTextInstance.GetComponent<StatusChangeTextController>().Initialize("XP + 1", Color.white);
        }
    }
}