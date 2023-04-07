using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class RequiredResourceProfile : ScriptableObject
    {
        public List<RequiredResource> requiredResources = new List<RequiredResource>();

        private const string ProfileName = "Required Resources";

        private void OnValidate()
        {
            foreach (var input in requiredResources)
            {
                if (input != null) input.displayName = $"[{input.amount}] {input.profile?.name}";
            }
        }
    }
    
    [Serializable]
    public class RequiredResource
    {
        [HideInInspector]
        [SerializeField]
        internal string displayName = string.Empty;
            
        [Space]
        [Min(0)] public int amount = 1;
        public ResourceProfile profile = null;
    }
}