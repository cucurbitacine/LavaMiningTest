using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = "Create ResourceDatabase", fileName = "ResourceDatabase", order = 0)]
    public class ResourceDatabase : ScriptableObject
    {
        public List<ResourceProfile> profiles = new List<ResourceProfile>();
    }
}