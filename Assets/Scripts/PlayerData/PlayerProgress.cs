using System;
using System.Collections.Generic;

namespace PlayerData
{
    [Serializable]
    public class PlayerProgress
    {
        public int battleRound = 1;
        public Dictionary<int, PlayerOwnedHeroData> heroIndexToOwnedHeroDataPair =
            new Dictionary<int, PlayerOwnedHeroData>();
        public List<PlayerOwnedHeroData> currentTeam = new List<PlayerOwnedHeroData>();

    }
}