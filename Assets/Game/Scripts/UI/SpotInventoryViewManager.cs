using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class SpotInventoryViewManager : MonoBehaviour
    {
        public ResourceSpotEntity spot = null;
        
        [Space]
        public SpotInventoryView viewPrefab = null;
        
        private void Start()
        {
            var view = Instantiate(viewPrefab);
            view.spot = spot;
            view.Subscribe(spot.inventory);
        }
    }
}