using DG.Tweening;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class ResourceViewEffect : MonoBehaviour
    {
        [Min(0)]
        public float shakeDuration = 1f;
        [Min(0)]
        public float shakePower = 0.5f;
        
        [Space]
        public ResourceView view = null;

        private void UpdateView(int value)
        {
            if (view.layoutElement != null) view.layoutElement.ignoreLayout = value == 0;
            if (view.canvasGroup != null) view.canvasGroup.alpha = value == 0 ? 0f : 1f;
            
            if (value > 0)
            {
                view.iconImage.rectTransform.DOComplete();
                view.iconImage.rectTransform.DOShakeScale(shakeDuration, shakePower);
            
                view.amountText.rectTransform.DOComplete();
                view.amountText.rectTransform.DOShakeScale(shakeDuration, shakePower); 
            }
        }
        
        private void OnEnable()
        {
            view.onAmountChanged.AddListener(UpdateView);
        }
        
        private void OnDisable()
        {
            view.onAmountChanged.RemoveListener(UpdateView);
        }
    }
}