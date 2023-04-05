﻿using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.CreateResourceProfile + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceProfile : ScriptableObjectWithGuid
    {
        private const string ProfileName = nameof(ResourceProfile);
        
        [Space]
        public string nameResource = string.Empty;
        public Sprite icon = null;

        [Space]
        [Multiline]
        public string description = "Good Luck, Mr. Gorsky!";
    }
}