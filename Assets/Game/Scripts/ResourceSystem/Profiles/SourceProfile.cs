using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class SourceProfile : DropperProfile
    {
        private const string ProfileName = "Source";
        
        [Header("Source Settings")]
        [Min(1)]
        public int maxAmountResources = 3; 
        [Min(0.01f)]
        public float frequencyMining = 1f;
        [Min(0f)]
        public float timeoutMining = 12f;

        [Header("Source information - read only")]
        [SerializeField]
        [Tooltip("Show total speed of mining")]
        private float resourcePerSecond = 0f;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            var durationMining = maxAmountResources / frequencyMining;
            var periodMining = durationMining + timeoutMining;
            resourcePerSecond = dropAmountResources * maxAmountResources / periodMining;
        }
    }
}