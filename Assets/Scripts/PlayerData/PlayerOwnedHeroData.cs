using System;
using UnityEngine;

namespace PlayerData
{
    [Serializable]
    public class PlayerOwnedHeroData
    {
        public string id;
        public Color color;
        public string name;
        public int level;
        public int maxHealth;
        public int currentHealth;
        public int attack;
        public int experience;
    }
}