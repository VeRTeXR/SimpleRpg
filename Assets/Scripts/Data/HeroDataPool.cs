using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Create HeroDataPool", fileName = "HeroDataPool", order = 0)]
    public class HeroDataPool: ScriptableObject
    {
        public List<HeroData> heroDataList = new List<HeroData>();
    }
}
