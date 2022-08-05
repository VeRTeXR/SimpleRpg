using System;
using UnityEngine;

namespace UI.BattlePage
{
    public class DamageTextGenerator: MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private Transform textParent;
        [Header("Data References")]
        [SerializeField] private GameObject textPrefab;

        [SerializeField] private LeanTweenType animationEase;

        public void ShowDamageDealt(int damage)
        {
            var textInstance = Instantiate(textPrefab, textParent);
            textInstance.transform.localPosition = Vector3.zero;

            LeanTween.moveLocalY(textInstance, textInstance.transform.localPosition.y + 100, 0.25f)
                .setEase(animationEase).setOnComplete(() =>
                {
                    Destroy(textInstance);   
                });
        }
        
    }
}