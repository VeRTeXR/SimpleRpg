using UnityEngine;
using UnityEngine.UI;

namespace UI.BattlePage
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        private float _currentFill;

        public void SetFill(int maxHealth, int currentHealth)
        {
            _currentFill = (float)currentHealth/maxHealth;        
            fillImage.fillAmount = _currentFill;
        }    


    }
}
