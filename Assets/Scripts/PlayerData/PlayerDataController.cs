using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using echo17.Signaler.Core;
using UI.BattlePage;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace PlayerData
{
    public class PlayerDataController : MonoBehaviour,ISubscriber
    {
        [Header("Data References")]
        [SerializeField] private HeroDataPool heroDataPool;

        private PlayerProgress _playerProgress;


        private void Awake()
        {
            Signaler.Instance.Subscribe<RequestPlayerDataController>(this, OnRequestPlayerDataController);
            PopulatePlayerSaveData();
        }

        private bool OnRequestPlayerDataController(RequestPlayerDataController signal)
        {
            signal.requester.SetPlayerDataController(this);
            return true;
        }

        private void PopulatePlayerSaveData()
        {
            if (!ES3.FileExists(Globals.SaveDataPath)) ES3.Save<int>("SaveInitialized", 1);

            if (ES3.KeyExists(Globals.PlayerProgressKey))
                _playerProgress = ES3.Load<PlayerProgress>(Globals.PlayerProgressKey);
            else
                GenerateNewPlayerProgressData();
        }

        private void GenerateNewPlayerProgressData()
        {
            _playerProgress = new PlayerProgress();
            _playerProgress.battleRound = 1;
            for (var i = 0; i < Globals.StarterUnitCount; i++) 
                AddRandomHeroToPlayerProgress(_playerProgress);

            EquipFirstThreeHeroes();

            ES3.Save<PlayerProgress>(Globals.PlayerProgressKey, _playerProgress);
        }

        private void EquipFirstThreeHeroes()
        {
            for (var i = 0; i < 3; i++)
                _playerProgress.currentTeam.Add(_playerProgress.playerOwnedHeroList[i]);
        }

        private HeroData AddRandomHeroToPlayerProgress(PlayerProgress playerProgress)
        {
            var epochStart = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            var timestamp = (DateTime.UtcNow - epochStart).TotalSeconds;
            var heroUnit = RandomizeHeroFromPool();

            playerProgress.playerOwnedHeroList.Add(
                new PlayerOwnedHeroData
                {
                    id = heroUnit.id + "," + timestamp+","+Random.Range(float.MinValue, float.MaxValue),
                    level = 1,
                    name = heroUnit.name,
                    color =  heroUnit.heroColor,
                    maxHealth = heroUnit.health,
                    currentHealth = heroUnit.health,
                    attack = heroUnit.attack, 
                    experience = 0
                });

            return heroUnit;
        }

        private HeroData RandomizeHeroFromPool()
        {
            var rngIndex = Random.Range(0, heroDataPool.heroDataList.Count-1);
            return heroDataPool.heroDataList[rngIndex]; 
        }

       

        public List<PlayerOwnedHeroData> GetCurrentTeamList()
        {
            return _playerProgress.currentTeam;
        }

        public List<PlayerOwnedHeroData> GetOwnedHeroList()
        {
            return _playerProgress.playerOwnedHeroList;
        }

        public int GetCurrentPlayerRound()
        {
            return _playerProgress.battleRound;
        }

        public void IncrementRound()
        {
            _playerProgress.battleRound++;
        }

        public HeroData GrantRandomHero()
        {
            return AddRandomHeroToPlayerProgress(_playerProgress);
        }

        public void SavePlayerData()
        {
            ES3.Save<PlayerProgress>(Globals.PlayerProgressKey, _playerProgress);
        }

        public void UpdateCurrentTeamHealth(List<PlayerInBattleHeroController> heroListFromBattle)
        {
            var currentTeam = _playerProgress.currentTeam;

            if (heroListFromBattle == null || heroListFromBattle.Count <= 0)
            {
                _playerProgress.currentTeam = new List<PlayerOwnedHeroData>();
                return;
            }
            
            foreach (var playerOwnedHeroData in currentTeam)
            foreach (var inBattleHeroController in heroListFromBattle)
                if (string.Equals(playerOwnedHeroData.id, inBattleHeroController.Id))
                    playerOwnedHeroData.currentHealth = inBattleHeroController.CurrentHealth;

            var remainingHeroDataList = new List<PlayerOwnedHeroData>();
            foreach (var inBattleHeroController in heroListFromBattle)
                remainingHeroDataList.Add(inBattleHeroController.HeroData);
            
            if (heroListFromBattle.Count != _playerProgress.currentTeam.Count)
                foreach (var currentTeamHeroData in currentTeam)
                    if (!remainingHeroDataList.Contains(currentTeamHeroData))
                        currentTeam.Remove(currentTeamHeroData);

            _playerProgress.currentTeam = currentTeam;
        }

        public bool IsHeroAlreadyInCurrentTeam(PlayerOwnedHeroData heroData)
        {
            var currentTeam = GetCurrentTeamList();
            return currentTeam.Any(inTeamHeroData => string.Equals(heroData.id, inTeamHeroData.id));
        }

        public void UnequipHeroFromCurrentTeam(PlayerOwnedHeroData heroData)
        {
            var modifiedTeam = new List<PlayerOwnedHeroData>();
            foreach (var currentTeamHeroData in _playerProgress.currentTeam)
            {
                if(heroData.id == currentTeamHeroData.id)
                    continue;
                modifiedTeam.Add(currentTeamHeroData);
            }
            _playerProgress.currentTeam = modifiedTeam;
        }

        public void EquipHeroToCurrentTeam(PlayerOwnedHeroData heroData)
        {
            _playerProgress.currentTeam.Add(heroData);
        }

        public List<PlayerOwnedHeroData> IncrementExpForEachUnit()
        {
            var levelUpUnitList = new List<PlayerOwnedHeroData>();
            foreach (var inTeamHeroData in _playerProgress.currentTeam)
            {
                inTeamHeroData.experience += 1;
                if (inTeamHeroData.experience >= Globals.LevelExperienceLimit)
                {
                    levelUpUnitList.Add(inTeamHeroData);
                    IncrementUnitLevel(inTeamHeroData);
                }
            }

            return levelUpUnitList;
        }

        private void IncrementUnitLevel(PlayerOwnedHeroData inTeamHeroData)
        {
            inTeamHeroData.experience = 0;
            inTeamHeroData.level++;
            inTeamHeroData.maxHealth = Mathf.CeilToInt(inTeamHeroData.maxHealth * 1.1f);
            inTeamHeroData.currentHealth = inTeamHeroData.maxHealth;
            inTeamHeroData.attack = Mathf.CeilToInt(inTeamHeroData.attack * 1.1f);            
        }
    }
}
