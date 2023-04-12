using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    /// <summary>
    /// Profile of resource
    /// </summary>
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceProfile : ScriptableObjectWithGuid
    {
        private const string ProfileName = "Resource profile";
        
        [Space]
        [Tooltip("Resource prefab")]
        public ResourceBehaviour prefab = null;
        [Tooltip("Resource icon. Will be shown in inventory")]
        public Sprite icon = null;
        
        [Space]
        public string nameResource = string.Empty;
        [Multiline]
        public string description = "Good Luck, Mr. Gorsky!";

        [Space]
        [Tooltip("Effect's profile")]
        public ResourceEffectProfile effect = null;
    }
}