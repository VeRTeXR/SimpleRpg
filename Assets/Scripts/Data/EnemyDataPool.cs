using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    
    [CreateAssetMenu(menuName = "Create EnemyDataPool", fileName = "EnemyDataPool", order = 0)]
    public class EnemyDataPool : ScriptableObject
    {
        public List<EnemyData> enemyDataList = new List<EnemyData>();
    }
}
