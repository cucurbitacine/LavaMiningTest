using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceInventory : MonoBehaviour
    {
        public UnityEvent<ResourceEntity> onPutted = new UnityEvent<ResourceEntity>();
        public UnityEvent<ResourceEntity> onPicked = new UnityEvent<ResourceEntity>();

        public readonly Dictionary<ResourceProfile, Queue<ResourceEntity>> items = new Dictionary<ResourceProfile, Queue<ResourceEntity>>();

        public bool Contains(List<RequiredResource> listRequired)
        {
            return listRequired.All(required => required.amount <= CountResource(required.profile));
        }
        
        public int CountResource(ResourceProfile profile)
        {
            return items.TryGetValue(profile, out var queue) ? queue.Count : 0;
        }
        
        public void Put(ResourceEntity resource)
        {
            if (!items.TryGetValue(resource.profile, out var queue))
            {
                queue = new Queue<ResourceEntity>();
                items.Add(resource.profile, queue);
            }

            if (queue.Contains(resource))
            {
                return;
            }
            
            queue.Enqueue(resource);
            
            onPutted.Invoke(resource);
        }

        public bool TryPick(ResourceProfile inputProfile, out ResourceEntity outputResource)
        {
            if (items.TryGetValue(inputProfile, out var queue))
            {
                if (queue.TryDequeue(out outputResource))
                {
                    onPicked.Invoke(outputResource);
                    return true;
                }
            }

            outputResource = null;
            return false;
        }

        public int Pick(List<RequiredResource> inputRequired, ref List<ResourceEntity> outputResources)
        {
            outputResources.Clear();

            foreach (var input in inputRequired)
            {
                for (var i = 0; i < input.amount; i++)
                {
                    if (TryPick(input.profile, out var output))
                    {
                        outputResources.Add(output);
                    }
                }
            }

            return outputResources.Count;
        }

#if UNITY_EDITOR
        [SerializeField] private List<InventoryItem> _displayItems = new List<InventoryItem>();

        private void Update()
        {
            _displayItems.Clear();
            _displayItems.AddRange(items.Keys.Select(k => new InventoryItem()
                { profileName = k.name, amount = items[k].Count }));
        }

        [Serializable]
        internal class InventoryItem
        {
            public string profileName = string.Empty;
            public int amount = 0;
        }
#endif
    }
}