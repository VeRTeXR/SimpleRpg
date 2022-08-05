using echo17.Signaler.Core;
using UI.PreBattlePage;
using UnityEngine;

namespace UI.WarningText
{
    public class WarningTextController : MonoBehaviour, ISubscriber
    {
        [SerializeField] private GameObject warningTextPrefab;
        
        [Header("Animation Attributes")]
        [SerializeField] private float textYPositionOffset = 30f;
        [SerializeField] private LeanTweenType textAnimationEaseType = LeanTweenType.easeInCirc;
        [SerializeField] private float textAnimationTime = 1.5f;


        private void Awake()
        {
            Signaler.Instance.Subscribe<ShowWarningText>(this, OnShowWarningText);
        }

        private bool OnShowWarningText(ShowWarningText signal)
        {
            var textInstance = Instantiate(warningTextPrefab, transform);
            textInstance.transform.localPosition = Vector3.zero;
            
            LeanTween.moveLocalY(textInstance, transform.localPosition.y + textYPositionOffset, textAnimationTime)
                .setEase(textAnimationEaseType).setOnComplete(
                    () => { Destroy(textInstance); });
            return true;
        }
    }
}
