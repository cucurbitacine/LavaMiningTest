using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile of source
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class SourceProfile : DropperProfile
    {
        private const string ProfileName = "Source profile";
        
        [Header("Source Settings")]
        [Tooltip("Max amount resources. Will be recovered that amount")]
        [Min(1)]
        public int maxAmountResources = 3; 
        [Tooltip("Max time duration of the mining process depends on the frequency of mining")]
        [Min(0.01f)]
        public float frequencyMining = 1f;
        [Tooltip("Time duration of the recovering process")]
        [Min(0f)]
        public float timeoutRecovery = 12f;

#if UNITY_EDITOR
        
        /*
         * Calculate and display information about source.
         * Useful for manage balance
         *
         * 1. Calculate amount drops = max amount resources / on drop amount per one time
         * 2. Calculate time duration of mining process = amount drops / frequency
         * 3. Calculate full time period of mining = mining time + recovering time
         * 4. Calculate RPS (resource per sec) = max amount resources / full time period of mining
         * 
         */
        
        [Header("Source information - read only")]
        [SerializeField]
        [Tooltip("Show total speed of mining")]
        private float resourcePerSecond = 0f;
        
        protected override void OnValidate()
        {
            base.OnValidate();

            var amountMiningCycle = Mathf.CeilToInt((float)maxAmountResources / dropAmountResources);
            var durationMiningCycle = amountMiningCycle / frequencyMining;
            var fullPeriodMiningCycle = durationMiningCycle + timeoutRecovery;
            resourcePerSecond = maxAmountResources / fullPeriodMiningCycle;
        }
#endif
    }
}