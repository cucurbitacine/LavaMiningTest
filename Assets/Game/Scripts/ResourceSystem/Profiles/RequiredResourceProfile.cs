using System;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class RequiredResourceProfile : ScriptableObject
    {
        public RequiredResource[] requiredResources = Array.Empty<RequiredResource>();

        private const string ProfileName = "Required Resources";
        
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

        private void OnValidate()
        {
            foreach (var input in requiredResources)
            {
                if (input != null) input.displayName = $"[{input.amount}] {input.profile?.name}";
            }
        }
    }
}