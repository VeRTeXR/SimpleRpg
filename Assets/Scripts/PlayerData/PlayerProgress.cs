using System;
using System.Collections.Generic;

namespace PlayerData
{
    [Serializable]
    public class PlayerProgress
    {
        public int battleRound = 1;
        public List<PlayerOwnedHeroData> playerOwnedHeroList = new List<PlayerOwnedHeroData>();
        public List<PlayerOwnedHeroData> currentTeam = new List<PlayerOwnedHeroData>();
    }
}