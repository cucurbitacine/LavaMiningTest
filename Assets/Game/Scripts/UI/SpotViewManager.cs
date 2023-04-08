using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class SpotViewManager : MonoBehaviour
    {
        public SpotBehaviour spot = null;

        [Space]
        public SpotViewController spotViewPrefab = null;

        private SpotViewController _spotView = null;

        private void Start()
        {
            _spotView = Instantiate(spotViewPrefab);
            _spotView.Initialize(spot);
        }

        private void OnDisable()
        {
            _spotView.Deinitialize();
        }
    }
}