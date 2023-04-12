using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile which keeps all available resources in the game
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceDatabase : ScriptableObject
    {
        [Tooltip("Put there all resources")]
        public List<ResourceProfile> profiles = new List<ResourceProfile>();

        private const string ProfileName = "Database of Resources";
        
        public bool TryGetProfile(Guid guid, out ResourceProfile profile)
        {
            profile = profiles.FirstOrDefault(p => p.Guid == guid);

            return profile != null;
        }
#if UNITY_EDITOR
        [ContextMenu("Fill Database")]
        private void FillDatabase()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(ResourceProfile)}"); 
            profiles.Clear();
            for (var i = 0; i < guids.Length; i++) 
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var profile = AssetDatabase.LoadAssetAtPath<ResourceProfile>(path);
                if (profile != null) profiles.Add(profile);
            }
        }
#endif
    }
}