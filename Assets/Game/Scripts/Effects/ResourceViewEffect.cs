using DG.Tweening;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class ResourceViewEffect : MonoBehaviour
    {
        [Space]
        public ResourceView view = null;

        private void UpdateView(int value)
        {
            if (view.layoutElement != null) view.layoutElement.ignoreLayout = value == 0;
            if (view.canvasGroup != null) view.canvasGroup.alpha = value == 0 ? 0f : 1f;
            
            if (value > 0)
            {
                view.gameObject.SetActive(true);
                
                view.iconImage.rectTransform.DOComplete();
                view.iconImage.rectTransform.DOShakeScale(1f, 0.5f);
            
                view.amountText.rectTransform.DOComplete();
                view.amountText.rectTransform.DOShakeScale(1f, 0.25f); 
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