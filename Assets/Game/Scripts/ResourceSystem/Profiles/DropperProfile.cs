using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile of base dropper entity
    /// </summary>
    public abstract class DropperProfile : ScriptableObjectWithGuid
    {
        [Header("Drop Settings")]
        [Tooltip("Amount of resources to be dropped at one time")]
        [Min(0)]
        public int dropAmountResources = 1;
        
        [Tooltip("Profile of resource which will be dropped")]
        public ResourceProfile dropResourceProfile = null;
        
        [Tooltip("Time between dropping resources if there are more than 1")]
        [Min(0f)]
        public float timeoutDropping = 0.2f;
    }
}