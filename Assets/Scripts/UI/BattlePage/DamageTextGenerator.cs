using TMPro;
using UnityEngine;

namespace UI.BattlePage
{
    //This component generate a generic damage text with provided animation attributes
    public class DamageTextGenerator: MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private Transform textParent;
     
        [Header("Data References")]
        [SerializeField] private GameObject textPrefab;
        
        [Header("Animation Attributes")]
        [SerializeField] private LeanTweenType animationEase;
        [SerializeField] private float animationYOffset = 150f;
        [SerializeField] private float animationTime = 0.25f;

        public void ShowDamageDealt(int damage)
        {
            var textInstance = Instantiate(textPrefab, textParent);
            textInstance.transform.localPosition = Vector3.zero;
            textInstance.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            
            LeanTween.moveLocalY(textInstance, textInstance.transform.localPosition.y + animationYOffset, animationTime)
                .setEase(animationEase).setOnComplete(() =>
                {
                    Destroy(textInstance);   
                });
        }
        
    }
}