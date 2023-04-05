using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceInventory : MonoBehaviour
    {
        public List<InventoryItem> items = new List<InventoryItem>();

        public void Put(ResourceEntity resource)
        {
            var item = items.FirstOrDefault(i => i.profile == resource.profile);

            if (item == null)
            {
                item = new InventoryItem(resource.profile);
                items.Add(item);
            }

            item.resources.Add(resource);
            
            resource.gameObject.SetActive(false);
        }

        public bool TryPick(ResourceProfile profile, out ResourceEntity resource)
        {
            var item = items.FirstOrDefault(i => i.profile == profile);

            if (item != null && item.resources.Count > 0)
            {
                resource = item.resources[0];
                item.resources.RemoveAt(0);
                return true;
            }

            resource = null;
            return false;
        }

        private void OnValidate()
        {
            foreach (var item in items)
            {
                if (item != null) item.displayName = $"[{item.resources.Count}] {item.profile.name}";
            }
        }

        [Serializable]
        public class InventoryItem
        {
            [HideInInspector] [SerializeField] internal string displayName = string.Empty;
            
            public ResourceProfile profile = null;
            public List<ResourceEntity> resources = new List<ResourceEntity>();

            public InventoryItem(ResourceProfile profile)
            {
                this.profile = profile;
            }
        }
    }
}