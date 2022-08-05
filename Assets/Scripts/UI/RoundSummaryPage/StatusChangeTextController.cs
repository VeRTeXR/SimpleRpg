using TMPro;
using UnityEngine;

namespace UI.RoundSummaryPage
{
    public class StatusChangeTextController:MonoBehaviour
    {
        [Header("Animation Attributes")]
        [SerializeField] private float textYPositionOffset = 30f;
        [SerializeField] private LeanTweenType textAnimationEaseType = LeanTweenType.easeInCirc;
        [SerializeField] private float textAnimationTime = 1.5f;

        private TextMeshProUGUI _statusChangeText;
        public void Initialize(string statusChangesText, Color textColor)
        {
            _statusChangeText = gameObject.GetComponent<TextMeshProUGUI>();            
            _statusChangeText.text = statusChangesText;
            _statusChangeText.color = textColor;
            transform.localPosition = Vector3.zero;
            LeanTween.moveLocalY(gameObject, transform.localPosition.y + textYPositionOffset, textAnimationTime)
                .setEase(textAnimationEaseType).setOnComplete(
                    () => { Destroy(gameObject); });
        }
    }
}