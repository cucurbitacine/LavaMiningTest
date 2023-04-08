using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class SpotProfile : DropperProfile
    {
        private const string ProfileName = "Spot";
        
        [Header("Spot Settings")]
        [Min(0f)]
        public float durationProduction = 10f;
        [Min(0f)]
        public float timeoutProduction = 1f;

        [Space]
        public RequiredResourceProfile inputRequiredProfile = null;
    }
}