using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile with information of required resources
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class RequiredResourceProfile : ScriptableObject
    {
        [Tooltip("Resources and its amount")]
        public List<ResourceStack> requiredResources = new List<ResourceStack>();

        private const string ProfileName = "Requirements";

        private void OnValidate()
        {
            foreach (var input in requiredResources)
            {
                if (input != null)
                {
                    var nameResource = string.Empty;
                    if (input.profile != null)
                    {
                        nameResource = string.IsNullOrWhiteSpace(input.profile.nameResource)
                            ? input.profile.name
                            : input.profile.nameResource;
                    }
                    
                    input.displayName = $"[{input.amount}] {nameResource}";
                }
            }
        }
    }
    
    [Serializable]
    public class ResourceStack
    {
        [HideInInspector]
        [SerializeField]
        internal string displayName = string.Empty;
            
        [Space]
        [Min(0)] public int amount = 1;
        public ResourceProfile profile = null;
    }
}