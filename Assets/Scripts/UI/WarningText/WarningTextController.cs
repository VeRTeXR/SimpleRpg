using echo17.Signaler.Core;
using UI.PreBattlePage;
using UnityEngine;

namespace UI.WarningText
{
    public class WarningTextController : MonoBehaviour, ISubscriber
    {
        [SerializeField] private GameObject warningTextPrefab;
        
        private void Awake()
        {
            Signaler.Instance.Subscribe<ShowWarningText>(this, OnShowWarningText);
        }

        private bool OnShowWarningText(ShowWarningText signal)
        {
            var textInstance = Instantiate(warningTextPrefab, transform);
            textInstance.transform.localPosition = Vector3.zero;

            //TODO:: Prettify text animation here
            var seq = LeanTween.sequence();
            seq.append(2);
            seq.append(
                () => { Destroy(textInstance); });

            return true;
        }
    }
}
