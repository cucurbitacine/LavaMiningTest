using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class PlayerInventoryView : InventoryView
    {
        [Space]
        public PlayerResourceView resourceViewPrefab = null;

        [Space]
        public Transform contentRoot = null;
        
        private readonly Dictionary<ResourceProfile, PlayerResourceView> _views =
            new Dictionary<ResourceProfile, PlayerResourceView>();

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

        private PlayerResourceView CreateView(ResourceProfile profile)
        {
            if (contentRoot == null) contentRoot = transform;
            
            var view = Instantiate(resourceViewPrefab, contentRoot);
            
            view.Initiate(profile);
            
            return view;
        }

        private PlayerResourceView GetView(ResourceProfile profile)
        {
            if (!_views.TryGetValue(profile, out var view))
            {
                view = CreateView(profile);
                _views.Add(profile, view);
            }

            return view;
        }
    }
}