using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile of spot
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class SpotProfile : DropperProfile
    {
        private const string ProfileName = "Spot profile";
        
        [Header("Spot Settings")]
        [Tooltip("Time duration of producing")]
        [Min(0f)]
        public float durationProduction = 10f;
        [Tooltip("Time duration of recovering")]
        [Min(0f)]
        public float timeoutRecovering = 1f;

        [Space]
        [Tooltip("Profile with information of required resources")]
        public RequiredResourceProfile inputRequiredProfile = null;
    }
}