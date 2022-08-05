using System.Collections.Generic;
using PlayerData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.RoundSummaryPage
{
    public class InTeamHeroStatusSummaryController: MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private HorizontalLayoutGroup heroLayoutGroup;        
       
        [Header("Data References")]
        [SerializeField] private GameObject heroDisplayPrefab;

        private List<PlayerOwnedHeroData> _currentTeam;


        public void Animate()
        {
            gameObject.SetActive(true);
        }


        public void Animate(List<PlayerOwnedHeroData> currentTeamList, List<PlayerOwnedHeroData> levelUpUnitList)
        {
            _currentTeam = currentTeamList;
                
        }
    }
}