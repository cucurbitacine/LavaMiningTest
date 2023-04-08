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
        
        public void Initialize(SpotBehaviour spotBehaviour)
        {
            spot = spotBehaviour;

            spot.onProductionChanged.AddListener(ProductionChange);

            inventoryView.spot = spot;
            inventoryView.Subscribe(spot.inventory);
            
            progressSlider.gameObject.SetActive(false);
        }

        public void Deinitialize()
        {
            inventoryView.Unsubscribe();

            spot.onProductionChanged.RemoveListener(ProductionChange);

            spot = null;
        }

        private void ProductionChange(bool start)
        {
            inventoryView.contentRoot.gameObject.SetActive(!start);
            progressSlider.gameObject.SetActive(start);

            if (_production != null) StopCoroutine(_production);
            
            if (start)
            {
                _production = StartCoroutine(_Production());
            }
        }

        private IEnumerator _Production()
        {
            while (spot.producting)
            {
                progressSlider.value = spot.productionProgress;
                yield return null;
            }
        }
    }
}