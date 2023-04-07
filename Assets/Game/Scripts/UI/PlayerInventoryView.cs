using System;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class PlayerInventoryView : InventoryView
    {
        [SerializeField] private ResourceInventory playerInventory = null;
        
        [Space]
        public ResourceView resourceViewPrefab = null;

        [Space]
        public Transform contentRoot = null;
        
        private readonly Dictionary<ResourceProfile, ResourceView> _views =
            new Dictionary<ResourceProfile, ResourceView>();

        protected override void Put(ResourceEntity resource)
        {
            var view = GetView(resource.profile);

            view.Increase();
        }

        protected override void Pick(ResourceEntity resource)
        {
            var view = GetView(resource.profile);

            view.Decrease();
        }

        private ResourceView CreateView(ResourceProfile profile)
        {
            if (contentRoot == null) contentRoot = transform;
            
            var view = Instantiate(resourceViewPrefab, contentRoot);
            
            view.Initiate(profile);
            
            return view;
        }

        private ResourceView GetView(ResourceProfile profile)
        {
            if (!_views.TryGetValue(profile, out var view))
            {
                view = CreateView(profile);
                _views.Add(profile, view);
            }

            return view;
        }

        private void OnEnable()
        {
            if (playerInventory != null) Subscribe(playerInventory);
        }
        
        private void OnDisable()
        {
            Unsubscribe();
        }
    }
}