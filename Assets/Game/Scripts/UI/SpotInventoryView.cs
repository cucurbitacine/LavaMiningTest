using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class SpotInventoryView : InventoryView
    {
        public SpotBehaviour spot = null;

        [Space]
        public Vector3 spotPositionOffset = Vector3.zero;
        public Vector2 viewportOffset = Vector2.zero;
        public Vector2 screenOffset = Vector2.zero;
        
        [Space]
        public ResourceView resourceViewPrefab = null;
        
        [Space]
        public RectTransform rect = null;
        public Transform contentRoot = null;

        private bool _isOnScreen = false;
        private Vector2 _viewport = Vector2.zero;
        private Vector2 _screen = Vector2.zero;
        
        private Camera _cam = null;
        
        private readonly Dictionary<ResourceProfile, ResourceView> _views =
            new Dictionary<ResourceProfile, ResourceView>();
        
        protected override void Put(ResourceBehaviour resource)
        {
            var view = GetView(resource.profile);

            view.Decrease();
        }

        protected override void Pick(ResourceBehaviour resource)
        {
            var view = GetView(resource.profile);

            view.Increase();
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

        private void UpdateView()
        {
            _viewport = _cam.WorldToViewportPoint(spot.transform.position + spotPositionOffset);
            _screen = _cam.ViewportToScreenPoint(_viewport + viewportOffset);
            _isOnScreen = _viewport.x is > 0 and < 1 && _viewport.y is > 0 and < 1;

            rect.anchoredPosition = _screen + screenOffset;
        }
        
        private void Awake()
        {
            if (_cam == null) _cam = Camera.main;
            if (rect == null) rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (spot != null && spot.inventory != null) Subscribe(spot.inventory);
        }
        
        private void OnDisable()
        {
            Unsubscribe();
        }
        
        private void Start()
        {
            foreach (var resourceStack in spot.profile.inputRequiredProfile.requiredResources)
            {
                var view = GetView(resourceStack.profile);
                view.SetAmount(resourceStack.amount);
            }
        }

        private void Update()
        {
            UpdateView();
        }
    }
}