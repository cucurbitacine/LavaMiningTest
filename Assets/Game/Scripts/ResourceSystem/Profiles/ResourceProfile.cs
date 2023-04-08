using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.ResourceSystem + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceProfile : ScriptableObjectWithGuid
    {
        private const string ProfileName = "Resource";
        
        [Space]
        public ResourceBehaviour prefab = null;
        public Sprite icon = null;
        
        [Space]
        public string nameResource = string.Empty;
        [Multiline]
        public string description = "Good Luck, Mr. Gorsky!";

        [Space]
        public ResourceEffectProfile effect = null;
    }
}