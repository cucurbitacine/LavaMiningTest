using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceSourceProfile : ScriptableObjectWithGuid
    {
        private const string ProfileName = "Source";
        
        [Header("Source Settings")]
        [Min(1)]
        public int amountMaxMining = 3; 
        [Min(0.01f)]
        public float frequencyMining = 1f;
        [Min(0f)]
        public float timeoutMining = 12f;

        [Header("Resource Settings")]
        [Min(0)]
        public int outputResourceAmount = 1;
        public ResourceEntity outputResourcePrefab = null;

        [Space]
        public bool inputRequired = false;
        public RequiredResourceProfile inputResourceProfile = null;
        
        [Header("Source information - read only")]
        [SerializeField]
        [Tooltip("Show total speed of mining")]
        private float resourcePerSecond = 0f;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            var durationMining = amountMaxMining / frequencyMining;
            var periodMining = durationMining + timeoutMining;
            resourcePerSecond = outputResourceAmount * amountMaxMining / periodMining;
        }
    }
}