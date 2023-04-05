﻿using System;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Profiles
{
    [CreateAssetMenu(menuName = GameManager.CreateResourceSourceProfile + ProfileName, fileName = ProfileName, order = 0)]
    public class ResourceSourceProfile : ScriptableObjectWithGuid
    {
        private const string ProfileName = nameof(ResourceSourceProfile);
        
        [Header("Source Settings")]
        [Min(1)]
        public int amountMaxMining = 3; 
        [Min(0.01f)]
        public float frequencyMining = 1f;
        [Min(0f)]
        public float timeoutMining = 12f;

        [Header("Resource Settings")]
        public ResourceInfo outputResource = new ResourceInfo();
        [Space]
        public List<ResourceInfo> inputResources = new List<ResourceInfo>();

        [Header("Source information - read only")]
        [SerializeField]
        [Tooltip("Show total speed of mining")]
        private float resourcePerSecond = 0f;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            var durationMining = amountMaxMining / frequencyMining;
            var periodMining = durationMining + timeoutMining;
            resourcePerSecond = amountMaxMining / periodMining;

            foreach (var input in inputResources)
            {
                if (input != null) input.displayName = $"[{input.amount}] {input.resourcePrefab?.name}";
            }
        }

        [Serializable]
        public class ResourceInfo
        {
            [HideInInspector]
            [SerializeField]
            internal string displayName = string.Empty;
            
            [Space]
            [Min(0)] public int amount = 1;
            public ResourceEntity resourcePrefab = null;
        }
    }
}