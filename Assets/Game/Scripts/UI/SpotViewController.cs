using System.Collections;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpotViewController : MonoBehaviour
    {
        public SpotBehaviour spot = null;

        [Space]
        public SpotInventoryView inventoryView = null;
        public Slider progressSlider = null;

        private Coroutine _production = null;

        private CanvasGroup _inventoryGroup = null;
        private CanvasGroup _sliderGroup = null;
        
        public void Initialize(SpotBehaviour spotBehaviour)
        {
            spot = spotBehaviour;

            spot.onProductionChanged.AddListener(ProductionChange);

            inventoryView.spot = spot;
            inventoryView.Subscribe(spot.inventory);
            
            _sliderGroup.alpha = 0f;
        }

        public void Deinitialize()
        {
            inventoryView.Unsubscribe();

            spot.onProductionChanged.RemoveListener(ProductionChange);

            spot = null;
        }

        private void ProductionChange(bool start)
        {
            _inventoryGroup.alpha = start ? 0f : 1f;
            _sliderGroup.alpha = start ? 1f : 0f;
            
            if (_production != null) StopCoroutine(_production);
            
            if (start)
            {
                _production = StartCoroutine(_Production());
            }
        }

        private IEnumerator _Production()
        {
            while (spot.producing)
            {
                progressSlider.value = spot.productionProgress;
                yield return null;
            }
        }

        private void Awake()
        {
            _inventoryGroup = inventoryView.contentRoot.GetComponent<CanvasGroup>();
            _sliderGroup = progressSlider.GetComponent<CanvasGroup>();

            if (_inventoryGroup == null)
            {
                _inventoryGroup = inventoryView.contentRoot.gameObject.AddComponent<CanvasGroup>();
            }
            
            if (_sliderGroup == null)
            {
                _sliderGroup = progressSlider.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
}