using System;
using System.Collections.Generic;
using Data;
using echo17.Signaler.Core;
using Newtonsoft.Json.Converters;
using UI.PreBattlePage;
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
            {
                _playerProgress = ES3.Load<PlayerProgress>(Globals.PlayerProgressKey);
                LoadedSaveData(_playerProgress);
            }
            else
                GenerateNewPlayerProgressData();
        }

        private void GenerateNewPlayerProgressData()
        {
            _playerProgress = new PlayerProgress();
            _playerProgress.battleRound = 1;
            for (var i = 0; i < Globals.StarterUnitCount; i++) 
                AddRandomHeroToPlayerProgress(_playerProgress, i);

            EquipFirstThreeHeroes();

            ES3.Save<PlayerProgress>(Globals.PlayerProgressKey, _playerProgress);
        }

        private void EquipFirstThreeHeroes()
        {
            for (var i = 0; i < 3; i++)
                _playerProgress.currentTeam.Add(_playerProgress.heroIndexToOwnedHeroDataPair[i]);
        }

        private void AddRandomHeroToPlayerProgress(PlayerProgress playerProgress, int heroIndex)
        {
            var epochStart = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            var timestamp = (DateTime.UtcNow - epochStart).TotalSeconds;
            var heroUnit = RandomizeHeroFromPool();

            playerProgress.heroIndexToOwnedHeroDataPair.Add(heroIndex,
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
            
        }

        private HeroData RandomizeHeroFromPool()
        {
            var rngIndex = Random.Range(0, heroDataPool.heroDataList.Count-1);
            return heroDataPool.heroDataList[rngIndex]; 
        }

        private void LoadedSaveData(PlayerProgress playerProgress)
        {
            Debug.Log("Loaded:"+ playerProgress.heroIndexToOwnedHeroDataPair.Count);
            foreach (var ownedHero in playerProgress.heroIndexToOwnedHeroDataPair)
            {
                Debug.Log("Id: "+ ownedHero.Key + " :::: "+ownedHero.Value.currentHealth);
            }
        }

        public List<PlayerOwnedHeroData> GetCurrentTeam()
        {
            return _playerProgress.currentTeam;
        }

        public Dictionary<int, PlayerOwnedHeroData> GetOwnedHeroDictionary()
        {
            return _playerProgress.heroIndexToOwnedHeroDataPair;
        }

        public int GetCurrentPlayerRound()
        {
            return _playerProgress.battleRound;
        }
    }
}
