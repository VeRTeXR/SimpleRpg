using PlayerData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PreBattlePage
{
    public class HeroDisplayController:MonoBehaviour
    {

        [SerializeField] private Image heroImage;
        private PlayerOwnedHeroData _heroData;

        public void Initialize(PlayerOwnedHeroData ownedHeroData)
        {
            _heroData = ownedHeroData;
            heroImage.color = ownedHeroData.color;
        }
    }
}